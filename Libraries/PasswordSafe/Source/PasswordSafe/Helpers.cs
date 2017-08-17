using System;
using System.IO;
using System.Security.Cryptography;

namespace PasswordSafe {
    internal static class Helpers {

        private static readonly Lazy<byte[]> lazyRandomKey = new Lazy<byte[]>(() => {
            var buffer = new byte[16];
            RandomNumberGenerator.Create().GetBytes(buffer);
            return buffer;
        });
        private static readonly Lazy<Aes> aes = new Lazy<Aes>(() => Aes.Create());

        //.NET Standard compatible replacement for ProtectedData.Unprotect(encryptedData, optionalEntropy, DataProtectionScope.CurrentUser);
        public static byte[] UnprotectData(byte[] encryptedData, byte[] optionalEntropy) {
            if (encryptedData == null) { throw new ArgumentNullException(nameof(encryptedData), "Data cannot be null."); }
            var iv = new byte[16]; //all 0s by default
            if (optionalEntropy != null) { Buffer.BlockCopy(optionalEntropy, 0, iv, 0, (optionalEntropy.Length < 16) ? optionalEntropy.Length : 16); } //just copy first 16 bytes of entropy to IV
            using (var ms = new MemoryStream(encryptedData)) {
                using (var decryptor = aes.Value.CreateDecryptor(lazyRandomKey.Value, iv))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read)) {
                    var decryptedBuffer = new byte[encryptedData.Length];
                    var decryptedLength = cs.Read(decryptedBuffer, 0, decryptedBuffer.Length);

                    var newBuffer = new byte[decryptedLength];
                    Buffer.BlockCopy(decryptedBuffer, 0, newBuffer, 0, newBuffer.Length);
                    Array.Clear(decryptedBuffer, 0, decryptedBuffer.Length);
                    return newBuffer;
                }
            }
        }

        //.NET Standard compatible replacement for ProtectedData.Protect(userData, optionalEntropy, DataProtectionScope.CurrentUser);
        public static byte[] ProtectData(byte[] userData, byte[] optionalEntropy) {
            if (userData == null) { throw new ArgumentNullException(nameof(userData), "Data cannot be null."); }
            var iv = new byte[16]; //all 0s by default
            if (optionalEntropy != null) { Buffer.BlockCopy(optionalEntropy, 0, iv, 0, (optionalEntropy.Length < 16) ? optionalEntropy.Length : 16); } //just copy first 16 bytes of entropy to IV
            using (var ms = new MemoryStream()) {
                using (var encryptor = aes.Value.CreateEncryptor(lazyRandomKey.Value, iv))
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write)) {
                    cs.Write(userData, 0, userData.Length);
                }
                return ms.ToArray();
            }
        }

    }
}
