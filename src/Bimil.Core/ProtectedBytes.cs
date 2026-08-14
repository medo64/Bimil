namespace Bimil;

using System;
using System.IO;
using System.Security.Cryptography;

/// <summary>
/// Handling obfuscation.
/// Not really secure but it does minimize plain text flowing around.
/// </summary>
internal static class ProtectedBytes {

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
    /// <param name="optionalEntropy">An optional additional byte array used to increase the complexity of the encryption, or null for no additional complexity.</param>
    /// <param name="zeroUserData">If true, userData will be zeroed.</param>
    /// <exception cref="ArgumentNullException">Data cannot be null.</exception>
    public static byte[] ProtectData(byte[] userData, byte[]? optionalEntropy = null, bool zeroUserData = false) {
        if (userData == null) { throw new ArgumentNullException(nameof(userData), "Data cannot be null."); }

        var iv = LazyRandomIV.Value;
        try {
            if (optionalEntropy != null) { Buffer.BlockCopy(optionalEntropy, 0, iv, 0, (optionalEntropy.Length < 16) ? optionalEntropy.Length : 16); }  // just copy first 16 bytes of entropy to IV

            using var encryptor = CryptoAlgorithm.Value.CreateEncryptor(LazyRandomKey.Value, iv);
            return encryptor.TransformFinalBlock(userData, 0, userData.Length);
        } finally {
            if (zeroUserData)  { CryptographicOperations.ZeroMemory(userData); }
        }
    }

    /// <summary>
    /// Decrypts the data in a specified buffer and writes the decrypted data to a destination buffer.
    /// A portable replacement for ProtectedData.Unprotect(encryptedData, optionalEntropy, DataProtectionScope.CurrentUser);
    /// </summary>
    /// <param name="encryptedData">A byte array containing data encrypted using the Protect(Byte[], Byte[], DataProtectionScope) method.</param>
    /// <param name="optionalEntropy">An optional additional byte array that was used to encrypt the data, or null if the additional byte array was not used.</param>
    /// <param name="zeroEncryptedData">If true, encryptedData will be zeroed.</param>
    /// <exception cref="ArgumentNullException">Data cannot be null.</exception>
    public static byte[] UnprotectData(byte[] encryptedData, byte[]? optionalEntropy = null, bool zeroEncryptedData = false) {
        if (encryptedData == null) { throw new ArgumentNullException(nameof(encryptedData), "Data cannot be null."); }

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
            if (zeroEncryptedData)  { CryptographicOperations.ZeroMemory(encryptedData); }
        }
    }

}
