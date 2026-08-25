namespace Bimil;

using System;
using System.Buffers.Binary;
using System.Security.Cryptography;

/// <summary>
/// Handling obfuscation.
/// Not really secure but it does minimize plain text flowing around.
/// </summary>
public sealed class ProtectedBytes {

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    internal ProtectedBytes()
        : this([], zeroBytes: false) {
    }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="bytes">Bytes.</param>
    internal ProtectedBytes(byte[] bytes)
        : this(bytes, zeroBytes: false) {
    }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="bytes">Bytes.</param>
    /// <param name="zeroBytes">If true, input bytes will be zeroed after protection.</param>
    internal ProtectedBytes(byte[] bytes, bool zeroBytes) {
        SetBytes(bytes, zeroBytes);
    }


    private readonly Lazy<byte[]> RandomIV = new(() => {
        var buffer = GC.AllocateArray<byte>(16, pinned: true);
        RandomNumberGenerator.Create().GetBytes(buffer);
        return buffer;
    });

    private byte[] Bytes = [];
    private int BytesLength;
    private int BytesHashCode;


    /// <summary>
    /// Sets bytes.
    /// </summary>
    /// <param name="bytes">Bytes.</param>
    public void SetBytes(byte[] bytes) {
        SetBytes(bytes, zeroBytes: false);
    }

    /// <summary>
    /// Sets bytes.
    /// </summary>
    /// <param name="bytes">Bytes.</param>
    /// <param name="zeroBytes">If true, input bytes will be zeroed after protection.</param>
    public void SetBytes(byte[] bytes, bool zeroBytes) {
        ArgumentNullException.ThrowIfNull(bytes);

        try {
            Bytes = ProtectData(bytes, RandomIV.Value);
            Length = bytes.Length;

            var newHashCode = bytes.Length;
            foreach (var b in Bytes) {
                newHashCode = HashCode.Combine(newHashCode, b);
            }
            BytesHashCode = newHashCode;
        } finally {
            if (zeroBytes) { CryptographicOperations.ZeroMemory(bytes); }
        }
    }

    /// <summary>
    /// Returns bytes.
    /// </summary>
    public byte[] GetBytes() {
        return UnprotectData(Bytes, RandomIV.Value);
    }


    /// <summary>
    /// Gets length of stored bytes.
    /// </summary>
    public int Length {
        get {
            return BytesLength ^ BinaryPrimitives.ReadInt32BigEndian(RandomIV.Value);
        }
        private set {
            BytesLength = value ^ BinaryPrimitives.ReadInt32BigEndian(RandomIV.Value);  // just a bit of obfuscation
        }
    }


    /// <inheritdoc />
    public override int GetHashCode() {
        var iv = RandomIV.Value.AsSpan();
        return HashCode.Combine(
            BinaryPrimitives.ReadInt32BigEndian(iv[0..4]),
            BinaryPrimitives.ReadInt32BigEndian(iv[4..8]),
            BinaryPrimitives.ReadInt32BigEndian(iv[8..12]),
            BinaryPrimitives.ReadInt32BigEndian(iv[12..16]),
            BytesHashCode
        );
    }


    #region Static

    private static readonly Lazy<byte[]> LazyRandomKey = new(() => {
        var buffer = GC.AllocateArray<byte>(32, pinned: true);
        RandomNumberGenerator.Create().GetBytes(buffer);
        return buffer;
    });

    private static readonly Lazy<byte[]> LazyRandomIV = new(() => {
        var buffer = GC.AllocateArray<byte>(16, pinned: true);
        RandomNumberGenerator.Create().GetBytes(buffer);
        return buffer;
    });


    private static readonly Lazy<Aes> CryptoAlgorithm = new(() => {
        var alg = Aes.Create();
        alg.KeySize = 256;
        alg.Mode = CipherMode.CBC;
        alg.Padding = PaddingMode.PKCS7;
        return alg;
    });


    /// <summary>
    /// Encrypts the data in a specified byte array and returns a byte array that contains the encrypted data.
    /// A portable replacement for ProtectedData.Protect(userData, optionalEntropy, DataProtectionScope.CurrentUser);
    /// </summary>
    /// <param name="userData">A byte array that contains data to encrypt.</param>
    /// <exception cref="ArgumentNullException">Data cannot be null.</exception>
    public static byte[] ProtectData(byte[] userData) {
        return ProtectData(userData, optionalEntropy: null);
    }

    /// <summary>
    /// Encrypts the data in a specified byte array and returns a byte array that contains the encrypted data.
    /// A portable replacement for ProtectedData.Protect(userData, optionalEntropy, DataProtectionScope.CurrentUser);
    /// </summary>
    /// <param name="userData">A byte array that contains data to encrypt.</param>
    /// <param name="optionalEntropy">An optional additional byte array used to increase the complexity of the encryption, or null for no additional complexity.</param>
    /// <exception cref="ArgumentNullException">Data cannot be null.</exception>
    public static byte[] ProtectData(byte[] userData, byte[]? optionalEntropy) {
        ArgumentNullException.ThrowIfNull(userData);

        var iv = LazyRandomIV.Value;
        try {
            if (optionalEntropy != null) { Buffer.BlockCopy(optionalEntropy, 0, iv, 0, (optionalEntropy.Length < 16) ? optionalEntropy.Length : 16); }  // just copy first 16 bytes of entropy to IV

            using var encryptor = CryptoAlgorithm.Value.CreateEncryptor(LazyRandomKey.Value, iv);
            return encryptor.TransformFinalBlock(userData, 0, userData.Length);
        } finally {
            CryptographicOperations.ZeroMemory(iv);
        }
    }

    /// <summary>
    /// Decrypts the data in a specified buffer and writes the decrypted data to a destination buffer.
    /// A portable replacement for ProtectedData.Unprotect(encryptedData, optionalEntropy, DataProtectionScope.CurrentUser);
    /// </summary>
    /// <param name="encryptedData">A byte array containing data encrypted using the Protect(Byte[], Byte[], DataProtectionScope) method.</param>
    /// <exception cref="ArgumentNullException">Data cannot be null.</exception>
    public static byte[] UnprotectData(byte[] encryptedData) {
        return UnprotectData(encryptedData, optionalEntropy: null);
    }

    /// <summary>
    /// Decrypts the data in a specified buffer and writes the decrypted data to a destination buffer.
    /// A portable replacement for ProtectedData.Unprotect(encryptedData, optionalEntropy, DataProtectionScope.CurrentUser);
    /// </summary>
    /// <param name="encryptedData">A byte array containing data encrypted using the Protect(Byte[], Byte[], DataProtectionScope) method.</param>
    /// <param name="optionalEntropy">An optional additional byte array that was used to encrypt the data, or null if the additional byte array was not used.</param>
    /// <exception cref="ArgumentNullException">Data cannot be null.</exception>
    public static byte[] UnprotectData(byte[] encryptedData, byte[]? optionalEntropy) {
        ArgumentNullException.ThrowIfNull(encryptedData);

        var decryptedBuffer = Array.Empty<byte>();
        var iv = LazyRandomIV.Value;
        try {
            if (optionalEntropy != null) { Buffer.BlockCopy(optionalEntropy, 0, iv, 0, (optionalEntropy.Length < 16) ? optionalEntropy.Length : 16); }  // just copy first 16 bytes of entropy to IV

            using var decryptor = CryptoAlgorithm.Value.CreateDecryptor(LazyRandomKey.Value, iv);
            decryptedBuffer = decryptor.TransformFinalBlock(encryptedData, 0, encryptedData.Length);  // not pinned but we'll survive

            var newBuffer = GC.AllocateArray<byte>(decryptedBuffer.Length, pinned: true);
            Buffer.BlockCopy(decryptedBuffer, 0, newBuffer, 0, newBuffer.Length);
            return newBuffer;
        } finally {
            CryptographicOperations.ZeroMemory(decryptedBuffer);
            CryptographicOperations.ZeroMemory(iv);
        }
    }

    /// <summary>
    /// Clears all bytes in the array.
    /// </summary>
    /// <param name="bytes">Bytes.</param>
    public static void ZeroMemory(byte[] bytes) {
        CryptographicOperations.ZeroMemory(bytes);
    }

    #endregion Static

}
