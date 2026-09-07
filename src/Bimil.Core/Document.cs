namespace Bimil;

using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Password Safe document.
/// </summary>
public sealed partial class Document {

    /// <summary>
    /// Creates a new document using database V3 format.
    /// </summary>
    public Document()
        : this(DatabaseVersion.V3) {
    }

    /// <summary>
    /// Creates a new document.
    /// </summary>
    /// <param name="databaseVersion">Database version.</param>
    /// <exception cref="ArgumentOutOfRangeException">Invalid database version.</exception>
    public Document(DatabaseVersion databaseVersion) {
        DatabaseVersion = databaseVersion;

        var salt = new byte[32]; RandomNumberGenerator.Fill(salt);
        var keyK = new byte[32]; RandomNumberGenerator.Fill(keyK);
        var keyL = new byte[32]; RandomNumberGenerator.Fill(keyL);
        ActiveKeyBlock = KeyBlock.Create(salt, KeyBlock.DefaultIterationCount, keyK, keyL, passphrase: null, zeroBytes: true);

        var versionField = databaseVersion switch {
            DatabaseVersion.V3 => Header.Create(HeaderType.Version, VersionHeader.GetBytes(DefaultVersion3), zeroBytes: true),
            DatabaseVersion.V4 => Header.Create(HeaderType.Version, VersionHeader.GetBytes(DefaultVersion4), zeroBytes: true),
            _ => throw new ArgumentOutOfRangeException(nameof(databaseVersion), "Invalid database version."),
        };
        var uuidField = Header.Create(HeaderType.Uuid, UuidHeader.GetBytes(Guid.CreateVersion7()));

        KeyBlocks = new KeyBlockCollection(this, [ActiveKeyBlock]);
        Headers = new HeaderCollection(databaseVersion, [versionField, uuidField]);
        Records = new RecordCollection([]);
    }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="databaseVersion">Database version.</param>
    /// <param name="keyBlocks">Key blocks.</param>
    /// <param name="headers">Header fields.</param>
    /// <param name="records">Records.</param>
    private Document(DatabaseVersion databaseVersion, KeyBlock activeKeyBlock, ICollection<KeyBlock> keyBlocks, ICollection<Header> headers, ICollection<Record> records) {
        DatabaseVersion = databaseVersion;
        ActiveKeyBlock = activeKeyBlock;
        KeyBlocks = new KeyBlockCollection(this, keyBlocks);
        Headers = new HeaderCollection(databaseVersion, headers);
        Records = new RecordCollection(records);
        IsReadOnly = !ActiveKeyBlock.HasPassphrase || !ActiveKeyBlock.HasAllKeys;
    }


    private static readonly Version DefaultVersion3 = new(3, 17, 0, 0);
    private static readonly Version DefaultVersion4 = new(4, 2, 0, 0);


    #region Properties

    /// <summary>
    /// Gets the document version..
    /// </summary>
    public DatabaseVersion DatabaseVersion { get; private set; }

    /// <summary>
    /// Gets active key.
    /// </summary>
    public KeyBlock ActiveKeyBlock { get; private set; }

    /// <summary>
    /// Gets file name used for the last save.
    /// If document contains no information about the file used, the value will be null.
    /// </summary>
    public FileInfo? File { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the document is read-only.
    /// </summary>
    public bool IsReadOnly {
        get;
        internal set {
            field = value;
            Headers.IsReadOnly = value;
            Records.IsReadOnly = value;
        }
    }

    #endregion Properties

    #region Hash

    /// <inheritdoc />
    public override int GetHashCode() {
        var hashCode = HashCode.Combine((int)DatabaseVersion, File?.GetHashCode(), ActiveKeyBlock.Passphrase.GetHashCode());

        foreach (var field in Headers) {
            hashCode = HashCode.Combine(hashCode, (int)field.Type, field.Data.GetHashCode());
        }

        foreach (var record in Records) {
            foreach (var field in record.Fields) {
                hashCode = HashCode.Combine(hashCode, (int)field.Type, field.Data.GetHashCode());
            }
        }

        return hashCode;
    }

    private int LastHashCode;

    /// <summary>
    /// Gets if file has been changed since the last time it was saved.
    /// Notice that operation might take some time since access will traverse all records.
    /// This functionality depends on GetHashCode in each record.
    /// </summary>
    public bool? HasChanged {
        get {
            var currHashCode = GetHashCode();
            return (LastHashCode != currHashCode);
        }
    }

    /// <summary>
    /// Clears HasChanged property.
    /// </summary>
    private void ResetChanges() {
        LastHashCode = GetHashCode();
    }

    #endregion

    #region Load

    /// <summary>
    /// Returns document loaded based on file name and passphrase.
    /// </summary>
    /// <param name="file">File.</param>
    /// <param name="passphrase">Passphrase.</param>
    public static Document Load(FileInfo file, string passphrase) {
        ArgumentNullException.ThrowIfNull(file);
        ArgumentNullException.ThrowIfNull(passphrase);

        var passphraseBytes = Encoding.UTF8.GetBytes(passphrase);
        try {
            return Load(file, passphraseBytes);
        } finally {
            CryptographicOperations.ZeroMemory(passphraseBytes);
        }
    }

    /// <summary>
    /// Returns document loaded based on file name and passphrase bytes.
    /// </summary>
    /// <param name="file">File.</param>
    /// <param name="passphrase">Passphrase bytes.</param>
    public static Document Load(FileInfo file, byte[] passphrase) {
        ArgumentNullException.ThrowIfNull(file);
        ArgumentNullException.ThrowIfNull(passphrase);

        using var stream = file.OpenRead();
        return LoadCore(stream, passphrase, file);
    }

    /// <summary>
    /// Returns document loaded based on file name and passphrase.
    /// </summary>
    /// <param name="file">File.</param>
    /// <param name="passphrase">Passphrase bytes.</param>
    public static Document Load(Stream stream, byte[] passphrase) {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(passphrase);

        return LoadCore(stream, passphrase, originalFile: null);
    }


    private static readonly UInt32 Tag = 0x50575333;  // PWS3
    private static readonly UInt128 TagEof = new(0x505753332D454F46, 0x505753332D454F46);

    private static Document LoadCore(Stream stream, byte[] passphrase, FileInfo? originalFile) {
        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        if (ms.Length < 200) { throw new FormatException("File content too short."); }

        var bytes = ms.ToArray();

        var v3Tag = BinaryPrimitives.ReadUInt32BigEndian(bytes[0..4]);
        var v3EofTag = BinaryPrimitives.ReadUInt128BigEndian(bytes[(bytes.Length - 48)..(bytes.Length - 32)]);

        Document? doc = null;
        try {
            if ((v3Tag == Tag) && (v3EofTag == TagEof)) {
                doc = LoadCoreV3(bytes, passphrase);
            } else {  // anything not v3 is assumed to be v4
                var nonce = new byte[32];
                Buffer.BlockCopy(bytes, 0, nonce, 0, 32);
                var nonceSha = SHA256.HashData(nonce);

                for (var i = 32; i < bytes.Length - 116; i += 116) {
                    var isSame = true;
                    for (var j = 0; j < 32; j++) {
                        if (bytes[i + j] != nonceSha[j]) { isSame = false; break; }
                    }
                    if (isSame) { doc = LoadCoreV4(bytes, passphrase); break; }
                }
            }
        } catch (CryptographicException ex) {
            throw new FormatException(ex.Message, ex);
        } catch (FormatException) {
            throw;
        }
        if (doc == null) { throw new FormatException("Unrecognized file format."); }

        // Update properties
        doc.File = originalFile;
        doc.ResetChanges();

        return doc;
    }

    #endregion Load

    #region Save

    /// <summary>
    /// Saves document using previous file name and passphrase.
    /// </summary>
    public void Save() {
        if (File == null) { throw new InvalidOperationException("File not specified."); }
        if (!ActiveKeyBlock.HasPassphrase) { throw new InvalidOperationException("Passphrase not specified."); }

        Save(File);
    }

    /// <summary>
    /// Saves document to provided file using provided passphrase.
    /// </summary>
    /// <param name="file">File.</param>
    /// <param name="passphrase">Passphrase bytes.</param>
    public void Save(FileInfo file) {
        ArgumentNullException.ThrowIfNull(file);

        using var stream = file.OpenWrite();
        stream.SetLength(0);
        SaveCore(stream, file);
    }

    /// <summary>
    /// Saves document to the provided stream using provided passphrase.
    /// </summary>
    /// <param name="stream">Stream.</param>
    public void Save(Stream stream) {
        ArgumentNullException.ThrowIfNull(stream);

        SaveCore(stream, originalFile: null);
    }

    private void SaveCore(Stream stream, FileInfo? originalFile) {
        if (!ActiveKeyBlock.HasPassphrase) { throw new InvalidOperationException("Active key block contains no passphrase."); }
        if (!ActiveKeyBlock.HasAllKeys) { throw new InvalidOperationException("Active key block contains no keys."); }

        // update headers - but only ones that exist
        var insertUser = false;
        for (var i = Headers.Count - 1; i >= 0; i--) {
            var header = Headers[i];
#pragma warning disable CS0612 // Type or member is obsolete
            if (header.Type is HeaderType.WhoPerformedLastSave) {
#pragma warning restore CS0612 // Type or member is obsolete
                Headers.RemoveAt(i);
                insertUser = true;
            } else if (header.Type == HeaderType.TimestampOfLastSave && header is TimestampHeader saveHeader) {
                saveHeader.Timestamp = DateTime.UtcNow;
            } else if (header.Type == HeaderType.WhatPerformedLastSave && header is TextHeader appHeader) {
                appHeader.Text = "Bimil V1.0.0";
            } else if (header.Type == HeaderType.LastSavedByUser && header is TextHeader userHeader) {
                userHeader.Text = Environment.UserName;
                insertUser = false;
            } else if (header.Type == HeaderType.LastSavedOnHost && header is TextHeader hostHeader) {
                hostHeader.Text = Environment.MachineName;
            }
        }
        if (insertUser) {
            var userHeader = (TextHeader)Header.Create(HeaderType.LastSavedByUser);
            userHeader.Text = Environment.UserName;
            Headers.Add(userHeader);
        }

        if (DatabaseVersion == DatabaseVersion.V3) {
            SaveCoreV3(stream);
        } else if (DatabaseVersion == DatabaseVersion.V4) {
            SaveCoreV4(stream);
        } else {
            throw new InvalidOperationException("Unknown version.");
        }

        // Update properties
        File = originalFile;
        ResetChanges();

    }

    #endregion Save

}
