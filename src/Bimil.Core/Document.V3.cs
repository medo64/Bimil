namespace Bimil;

using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

public sealed partial class Document {

    private static Document LoadCoreV3(byte[] bytes, byte[] passphrase) {
        var bytesSpan = bytes.AsSpan();

        var salt = bytes[4..36];
        var iter = BinaryPrimitives.ReadUInt32LittleEndian(bytesSpan[36..40]);

        byte[]? stretchedKey = null, keyK = null, keyL = null, data = null;
        try {
            stretchedKey = GetStretchedKey(passphrase, salt, iter);
            if (!AreBytesTheSame(GetSha256Hash(stretchedKey), bytes, 40)) {
                throw new CryptographicException("Password mismatch.");
            }
            keyK = DecryptKey(stretchedKey, bytes, 72);
            keyL = DecryptKey(stretchedKey, bytes, 104);

            var keyBlock = KeyBlock.Create(salt, iter, keyK, keyL, passphrase, zeroBytes: false);

            var iv = bytes[136..152];
            data = DecryptData(keyK, iv, bytes, 152, bytes.Length - 200);
            var dataSpan = data.AsSpan();

            using var dataHash = new HMACSHA256(keyL);
            var dataOffset = 0;

            var headerFields = new List<Header>();
            while (dataOffset < data.Length) {
                var fieldLength = BinaryPrimitives.ReadInt32LittleEndian(dataSpan[dataOffset..(dataOffset + 4)]);
                var fieldLengthFull = ((fieldLength + 5 - 1) / 16 + 1) * 16;
                var fieldType = (HeaderType)data[dataOffset + 4];
                var fieldData = new byte[fieldLength];
                try {
                    Buffer.BlockCopy(data, dataOffset + 5, fieldData, 0, fieldLength);
                    dataOffset += fieldLengthFull; // there is ALWAYS some random bytes added, thus extra block if 16 bytes

                    dataHash.TransformBlock(fieldData, 0, fieldData.Length, null, 0);  // not hashing length nor type - wtf?
                    if (fieldType == HeaderType.EndOfEntry) { break; }

                    var field = Header.Create(fieldType, fieldData, zeroBytes: true);
                    headerFields.Add(field);
                } finally {
                    CryptographicOperations.ZeroMemory(fieldData);
                }
            }

            if ((headerFields.Count > 0) && (headerFields[0] is VersionHeader versionHeader)) {
                if (versionHeader.Version.Major != 3) { throw new FormatException($"Unknown file format version ({versionHeader.Version})."); }
            } else {
                throw new FormatException("Unrecognized file format version.");
            }

            var records = new List<Record>();
            var recordFields = new List<Field>();
            while (dataOffset < data.Length) {
                var fieldLength = BinaryPrimitives.ReadInt32LittleEndian(dataSpan[dataOffset..(dataOffset + 4)]);
                var fieldLengthFull = ((fieldLength + 5 - 1) / 16 + 1) * 16;
                var fieldType = (FieldType)data[dataOffset + 4];
                var fieldData = new byte[fieldLength];
                try {
                    Buffer.BlockCopy(data, dataOffset + 5, fieldData, 0, fieldLength);
                    dataOffset += fieldLengthFull; //there is ALWAYS some random bytes added, thus extra block if 16 bytes

                    dataHash.TransformBlock(fieldData, 0, fieldData.Length, null, 0); //not hashing length nor type - wtf?
                    if (fieldType == FieldType.EndOfEntry) {
                        if (recordFields.Count > 0) {
                            records.Add(Record.Create(recordFields));
                        }
                        recordFields = [];
                        continue;
                    }

                    var field = Field.Create(fieldType, new ProtectedBytes(fieldData, zeroBytes: true));
                    recordFields.Add(field);
                } finally {
                    CryptographicOperations.ZeroMemory(fieldData);
                }
            }

            dataHash.TransformFinalBlock([], 0, 0);

            if (!AreBytesTheSame(dataHash.Hash, bytes, bytes.Length - 32)) {
                throw new CryptographicException("Authentication mismatch.");
            }

            //return new Document(passphraseBuffer, (int)iter, headerFields, [.. recordFields]);
            var doc = new Document(DatabaseVersion.V3, keyBlock, [keyBlock], headerFields, records);
            return doc;
        } catch (CryptographicException ex) {
            throw new FormatException(ex.Message, ex);
        } finally { //best effort to sanitize memory
            if (stretchedKey != null) { CryptographicOperations.ZeroMemory(stretchedKey); }
            if (keyK != null) { CryptographicOperations.ZeroMemory(keyK); }
            if (keyL != null) { CryptographicOperations.ZeroMemory(keyL); }
            if (data != null) { CryptographicOperations.ZeroMemory(data); }
            if (bytes != null) { CryptographicOperations.ZeroMemory(bytes); }
        }
    }

    private void SaveCoreV3(Stream stream) {
        if (!ActiveKeyBlock.HasPassphrase) { throw new InvalidOperationException("Active key block contains no passphrase."); }
        if (!ActiveKeyBlock.HasKeys) { throw new InvalidOperationException("Active key block contains no keys."); }

        var passphrase = Array.Empty<byte>();
        var stretchedKey = Array.Empty<byte>();
        var keyK = ActiveKeyBlock.KeyK.GetBytes();
        var keyL = ActiveKeyBlock.KeyL.GetBytes();
        var salt = ActiveKeyBlock.Salt.GetBytes();
        try {
            var tag = new byte[4];
            BinaryPrimitives.WriteUInt32BigEndian(tag, Tag);
            stream.Write(tag);

            stream.Write(salt);

            var iter = new byte[4];
            BinaryPrimitives.WriteUInt32LittleEndian(iter, ActiveKeyBlock.IterationCount);
            stream.Write(iter, 0, 4);

            passphrase = ActiveKeyBlock.Passphrase.GetBytes();
            stretchedKey = GetStretchedKey(passphrase, salt, ActiveKeyBlock.IterationCount);
            stream.Write(GetSha256Hash(stretchedKey), 0, 32);

            stream.Write(EncryptKey(stretchedKey, keyK, 0));
            stream.Write(EncryptKey(stretchedKey, keyL, 0));

            var iv = new byte[16];
            RandomNumberGenerator.Fill(iv);
            stream.Write(iv);

            using var dataHash = new HMACSHA256(keyL);
            using var twofish = new Twofish() {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.None,
                KeySize = 256,
                Key = keyK,
                IV = iv,
            };
            using (var dataEncryptor = twofish.CreateEncryptor()) {
                foreach (var header in Headers) {
                    WriteBlock(stream, dataHash, dataEncryptor, (byte)header.Type, header.Data.GetBytes());
                }
                WriteBlock(stream, dataHash, dataEncryptor, (byte)HeaderType.EndOfEntry, []);

                foreach (var record in Records) {
                    foreach (var field in record.Fields) {
                        WriteBlock(stream, dataHash, dataEncryptor, (byte)field.Type, field.Data.GetBytes());
                    }
                    WriteBlock(stream, dataHash, dataEncryptor, (byte)FieldType.EndOfEntry, []);
                }
            }

            dataHash.TransformFinalBlock([], 0, 0);

            var tagEof = new byte[16];
            BinaryPrimitives.WriteUInt128BigEndian(tagEof, TagEof);
            stream.Write(tagEof);

            if (dataHash.Hash == null) { throw new InvalidOperationException("Cannot compute hash."); }  // newer happens actually
            stream.Write(dataHash.Hash);
        } finally {
            CryptographicOperations.ZeroMemory(passphrase);
            CryptographicOperations.ZeroMemory(stretchedKey);
            CryptographicOperations.ZeroMemory(keyK);
            CryptographicOperations.ZeroMemory(keyL);
            CryptographicOperations.ZeroMemory(salt);
        }
    }


    #region Helpers

    private static byte[] GetStretchedKey(byte[] passphrase, byte[] salt, uint iterations) {
        var hash = GetSha256Hash(passphrase, salt);
        for (var i = 0; i < iterations; i++) {
            hash = GetSha256Hash(hash);
        }
        return hash;
    }

    private static byte[] GetSha256Hash(params byte[][] buffers) {
        using var hash = SHA256.Create();
        foreach (var buffer in buffers) {
            hash.TransformBlock(buffer, 0, buffer.Length, buffer, 0);
        }
        hash.TransformFinalBlock([], 0, 0);
        return hash.Hash ?? [];
    }

    private static bool AreBytesTheSame(byte[]? buffer1, byte[]? buffer2, int buffer2Offset) {
        if ((buffer1 == null) || (buffer2 == null)) { return false; }  // always different if null
        if (buffer1.Length == 0) { return false; }
        if (buffer2Offset + buffer1.Length > buffer2.Length) { return false; }
        for (var i = 0; i < buffer1.Length; i++) {
            if (buffer1[i] != buffer2[buffer2Offset + i]) { return false; }
        }
        return true;
    }

    private static byte[] DecryptData(byte[] key, byte[] iv, byte[] buffer, int offset, int length) {
        using var twofish = new Twofish();
        twofish.Mode = CipherMode.CBC;
        twofish.Padding = PaddingMode.None;
        twofish.KeySize = 256;
        twofish.Key = key;
        twofish.IV = iv;

        using var dataDecryptor = twofish.CreateDecryptor();
        return dataDecryptor.TransformFinalBlock(buffer, offset, length);
    }

    private static byte[] DecryptKey(byte[] stretchedKey, byte[] buffer, int offset) {
        using var twofish = new Twofish();
        twofish.Mode = CipherMode.ECB;
        twofish.Padding = PaddingMode.None;
        twofish.KeySize = 256;
        twofish.Key = stretchedKey;

        using var transform = twofish.CreateDecryptor();
        return transform.TransformFinalBlock(buffer, offset, 32);
    }

    private static byte[] EncryptKey(byte[] stretchedKey, byte[] buffer, int offset) {
        using var twofish = new Twofish();
        twofish.Mode = CipherMode.ECB;
        twofish.Padding = PaddingMode.None;
        twofish.KeySize = 256;
        twofish.Key = stretchedKey;

        using var transform = twofish.CreateEncryptor();
        return transform.TransformFinalBlock(buffer, offset, 32);
    }

    private static void WriteBlock(Stream stream, HashAlgorithm dataHash, ICryptoTransform dataEncryptor, byte type, byte[] fieldData) {
        dataHash.TransformBlock(fieldData, 0, fieldData.Length, null, 0);

        byte[]? fieldBlock = null;
        try {
            var fieldLengthPadded = ((fieldData.Length + 5 - 1) / 16 + 1) * 16;
            fieldBlock = new byte[fieldLengthPadded];

            RandomNumberGenerator.Fill(fieldBlock);
            var dataLen = new byte[4];
            BinaryPrimitives.WriteInt32LittleEndian(dataLen, fieldData.Length);
            Buffer.BlockCopy(dataLen, 0, fieldBlock, 0, 4);
            fieldBlock[4] = type;
            Buffer.BlockCopy(fieldData, 0, fieldBlock, 5, fieldData.Length);

            dataEncryptor.TransformBlock(fieldBlock, 0, fieldBlock.Length, fieldBlock, 0);
            stream.Write(fieldBlock, 0, fieldBlock.Length);
        } finally {
            CryptographicOperations.ZeroMemory(fieldData);
            if (fieldBlock != null) { CryptographicOperations.ZeroMemory(fieldBlock); }
        }
    }

    #endregion Helpers

}
