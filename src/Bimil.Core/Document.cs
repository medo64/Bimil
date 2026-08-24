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
    /// Creates a new document.
    /// </summary>
    public Document()
        : this(DatabaseVersion.V3) {
    }

    /// <summary>
    /// Creates a new document.
    /// </summary>
    /// <param name="databaseVersion">Database version.</param>
    public Document(DatabaseVersion databaseVersion) {
        DatabaseVersion = databaseVersion;
        IterationCount = 2 * 262144;
        Headers = new HeaderCollection([]);
        Records = new RecordCollection([]);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="databaseVersion">Database version.</param>
    /// <param name="iterationCount">Iteration count.</param>
    /// <param name="headers">Header fields.</param>
    /// <param name="records">Records.</param>
    private Document(DatabaseVersion databaseVersion, uint iterationCount, ICollection<Header> headers, ICollection<Record> records) {
        DatabaseVersion = databaseVersion;
        IterationCount = iterationCount;
        Headers = new HeaderCollection(headers);
        Records = new RecordCollection(records);
    }


    #region Properties

    /// <summary>
    /// Gets the document version..
    /// </summary>
    public DatabaseVersion DatabaseVersion { get; private set; }

    /// <summary>
    /// Gets/sets iteration count.
    /// </summary>
    public uint IterationCount {
        get;
        set {
            if (value < 262144) { value = 262144; }
            field = value;
        }
    }

    /// <summary>
    /// Gets file name used for the last save.
    /// If document contains no information about the file used, the value will be null.
    /// </summary>
    public FileInfo? File { get; private set; }

    private ProtectedBytes? PassphraseBytes;
    /// <summary>
    /// Returns last used passphrase.
    /// </summary>
    public byte[]? GetPassphrase() {
        if (PassphraseBytes == null) { return null; }
        return PassphraseBytes.GetBytes();
    }

    /// <summary>
    /// Sets passphrase.
    /// </summary>
    /// <param name="passphrase">Passphrase bytes.</param>
    public void SetPassphrase(byte[] passphrase) {
        ArgumentNullException.ThrowIfNull(passphrase);
        PassphraseBytes = new ProtectedBytes(passphrase);
    }

    /// <summary>
    /// Sets passphrase.
    /// </summary>
    /// <param name="passphrase">Passphrase.</param>
    public void SetPassphrase(string passphrase) {
        ArgumentNullException.ThrowIfNull(passphrase);
        var passphraseBytes = Encoding.UTF8.GetBytes(passphrase);
        PassphraseBytes = new ProtectedBytes(passphraseBytes, zeroBytes: true);
    }

    #endregion Properties

    #region Hash

    /// <inheritdoc />
    public override int GetHashCode() {
        var hashCode = HashCode.Combine((int)DatabaseVersion, File?.GetHashCode(), PassphraseBytes?.GetHashCode());

        foreach (var field in Headers) {
            hashCode = HashCode.Combine(hashCode, (int)field.Type, field.Data.GetHashCode());
        }

        //TODO: include other fields

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

        Document doc;
        if ((v3Tag == Tag) && (v3EofTag == TagEof)) {
            doc = LoadCoreV3(bytes, passphrase);
        } else {  // anything not v3 is assumed to be v4
            doc = LoadCoreV4(bytes, passphrase);
        }

        // Update properties
        doc.File = originalFile;
        doc.PassphraseBytes = new ProtectedBytes(passphrase);
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
        if (PassphraseBytes == null) { throw new InvalidOperationException("Passphrase not specified."); }

        var passphraseBytes = GetPassphrase()!;  // passphrase is not null due to internal check above
        try {
            Save(File, passphraseBytes);
        } finally {
            CryptographicOperations.ZeroMemory(passphraseBytes);
        }
    }

    /// <summary>
    /// Saves document to provided file using previous passphrase.
    /// </summary>
    /// <param name="file">File.</param>
    /// <param name="passphrase">Passphrase.</param>
    public void Save(FileInfo file) {
        ArgumentNullException.ThrowIfNull(file);
        if (PassphraseBytes == null) { throw new InvalidOperationException("Passphrase not specified."); }

        var passphraseBytes = GetPassphrase()!;  // passphrase is not null due to internal check above
        try {
            Save(file, passphraseBytes);
        } finally {
            CryptographicOperations.ZeroMemory(passphraseBytes);
        }
    }

    /// <summary>
    /// Saves document to provided file using provided passphrase.
    /// </summary>
    /// <param name="file">File.</param>
    /// <param name="passphrase">Passphrase.</param>
    public void Save(FileInfo file, string passphrase) {
        ArgumentNullException.ThrowIfNull(file);
        ArgumentNullException.ThrowIfNull(passphrase);

        var passphraseBytes = Encoding.UTF8.GetBytes(passphrase);
        try {
            Save(file, passphraseBytes);
        } finally {
            CryptographicOperations.ZeroMemory(passphraseBytes);
        }
    }

    /// <summary>
    /// Saves document to provided file using provided passphrase.
    /// </summary>
    /// <param name="file">File.</param>
    /// <param name="passphrase">Passphrase bytes.</param>
    public void Save(FileInfo file, byte[] passphrase) {
        ArgumentNullException.ThrowIfNull(file);
        ArgumentNullException.ThrowIfNull(passphrase);

        using var stream = file.OpenWrite();
        stream.SetLength(0);
        SaveCore(stream, passphrase, file);
    }

    /// <summary>
    /// Saves document to provided file using provided passphrase.
    /// </summary>
    /// <param name="stream">Stream.</param>
    public void Save(Stream stream) {
        ArgumentNullException.ThrowIfNull(stream);
        if (PassphraseBytes == null) { throw new InvalidOperationException("Passphrase not specified."); }

        var passphraseBytes = GetPassphrase()!;  // passphrase is not null due to internal check above
        try {
            Save(stream, passphraseBytes);
        } finally {
            CryptographicOperations.ZeroMemory(passphraseBytes);
        }
    }

    /// <summary>
    /// Saves document to the provided stream using provided passphrase.
    /// </summary>
    /// <param name="stream">Stream.</param>
    /// <param name="passphrase">Passphrase bytes.</param>
    public void Save(Stream stream, byte[] passphrase) {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(passphrase);

        SaveCore(stream, passphrase, originalFile: null);
    }

    private void SaveCore(Stream stream, byte[] passphrase, FileInfo? originalFile) {
        // update headers - but only ones that exist
        var insertUser = false;
        for (var i = Headers.Count - 1; i >= 0; i--) {
            var header = Headers[i];
#pragma warning disable CS0612 // Type or member is obsolete
            if (header.Type is HeaderType.WhoPerformedLastSave) {
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
#pragma warning restore CS0612 // Type or member is obsolete
        }
        if (insertUser) {
            var userHeader = (TextHeader)Header.Create(HeaderType.LastSavedByUser);
            userHeader.Text = Environment.UserName;
            Headers.Add(userHeader);
        }

        if (DatabaseVersion == DatabaseVersion.V3) {
            SaveCoreV3(stream, passphrase);
        } else if (DatabaseVersion == DatabaseVersion.V4) {
            SaveCoreV4(stream, passphrase);
        } else {
            throw new InvalidOperationException("Unknown version.");
        }

        // Update properties
        File = originalFile;
        PassphraseBytes = new ProtectedBytes(passphrase);
        ResetChanges();

    }

    #endregion Save

}
