namespace Bimil;

using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using global::Medo.Security.Cryptography.PasswordSafe;

/// <summary>
/// Handling static key.
/// </summary>
public static class StaticKey {

    private const HeaderType StaticKeyHeaderType = (HeaderType)0x6F;  // NOT compatible with PasswordSafe

    /// <summary>
    /// Returns static key if one is defined.
    /// </summary>
    /// <param name="document">Document.</param>
    public static byte[]? GetStaticKey(Document? document) {
        if (document == null) { return null; }

        foreach (var header in document.Headers) {
            if (header.HeaderType == StaticKeyHeaderType) {
                var bytes = header.GetBytes();
                if (bytes.Length == 64) {
                    return bytes;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Returns static key if one is defined.
    /// </summary>
    /// <param name="document">Document.</param>
    public static string? GetStaticKeyAsBase58(Document? document) {
        if (document == null) { return null; }

        byte[]? bytes = null;
        try {
            bytes = GetStaticKey(document);
            if (bytes != null) { return Base58.ToString(bytes); }
        } finally {
            if (bytes != null) { Array.Clear(bytes, 0, bytes.Length); }
        }
        return null;
    }

    /// <summary>
    /// Returns new random static key in Base58 format.
    /// </summary>
    public static string? GetNewStaticKeyAsBase58() {
        var keyBytes = new byte[64];
        try {
            var random = RandomNumberGenerator.Create();
            random.GetBytes(keyBytes);
            return Base58.ToString(keyBytes);
        } finally {
            Array.Clear(keyBytes, 0, keyBytes.Length);
        }
    }

    /// <summary>
    /// Removes static key from document.
    /// </summary>
    /// <param name="document">Document.</param>
    public static void RemoveStaticKey(Document? document) {
        if (document == null) { return; }

        for (var i = document.Headers.Count - 1; i >= 0; i--) {
            var header = document.Headers[i];
            if (header.HeaderType == StaticKeyHeaderType) {
                var bytes = header.GetBytes();
                try {
                    if (bytes.Length == 64) {
                        document.Headers.RemoveAt(i);
                    }
                } finally {
                    Array.Clear(bytes, 0, bytes.Length);
                }
            }
        }
    }

    /// <summary>
    /// Sets or replaces static key in the document.
    /// </summary>
    /// <param name="document">Document.</param>
    public static void SetStaticKey(Document? document, string? base58Key) {
        if (document == null) { return; }

        RemoveStaticKey(document);
        if (base58Key == null) { return; }

        var bytes = Base58.AsBytes(base58Key);
        try {
            var header = new Header(StaticKeyHeaderType);
            header.SetBytes(bytes);
            document.Headers.Add(header);
        } finally {
            Array.Clear(bytes, 0, bytes.Length);
        }
    }

    /// <summary>
    /// Saves document taking the static key into account.
    /// </summary>
    /// <param name="document">Document</param>
    public static void SaveDocumentWithStaticKey(Document? document, Stream? stream) {
        if (document == null) { return; }
        if (stream == null) { return; }

        byte[]? keyBytes = null;
        try {
            keyBytes = GetStaticKey(document);
            if (keyBytes != null) {
                document.SaveWithKey(stream, keyBytes);
            } else {
                document.Save(stream);
            }
        } finally {
            if (keyBytes != null) { Array.Clear(keyBytes, 0, keyBytes.Length); }
        }
    }


    /// <summary>
    /// Base58 encoder/decoder with leading-zero preservation.
    /// </summary>
    private static class Base58 {

        private static readonly char[] Map = new char[] {
            '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F',
            'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W',
            'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'm',
            'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
        };

        private static readonly Encoding Utf8 = new UTF8Encoding(false);


        #region ToString

        /// <summary>
        /// Returns Base58 string.
        /// </summary>
        /// <param name="value">Bytes to convert.</param>
        /// <exception cref="NullReferenceException">Bytes cannot be null.</exception>
        public static string ToString(byte[] value) {
            if (value == null) { throw new ArgumentNullException(nameof(value), "Bytes cannot be null."); }
            if (TryToString(value, out var result)) { return result; }
            throw new FormatException("Cannot convert.");
        }

        /// <summary>
        /// Returns Base58 string.
        /// </summary>
        /// <param name="value">UTF-8 text to convert.</param>
        /// <exception cref="NullReferenceException">Text cannot be null.</exception>
        public static string ToString(string value) {
            if (value == null) { throw new ArgumentNullException(nameof(value), "Text cannot be null."); }
            return ToString(Utf8.GetBytes(value));
        }


        /// <summary>
        /// Returns true of conversion succeeded.
        /// </summary>
        /// <param name="value">Bytes to convert.</param>
        /// <param name="result">Converstion result as Base58 string.</param>
        /// <exception cref="NullReferenceException">Bytes cannot be null.</exception>
        public static bool TryToString(byte[] value, out string result) {
            if (value == null) { throw new ArgumentNullException(nameof(value), "Bytes cannot be null."); }

            if (value.Length == 0) { //don't bother if there is nothing in array
                result = "";
                return true;
            }

            BigInteger input;
            var buffer = new byte[value.Length + 1]; //extra byte is leading 0
            try {
                Buffer.BlockCopy(value, 0, buffer, 1, value.Length);
                if (BitConverter.IsLittleEndian) { Array.Reverse(buffer); }
                input = new BigInteger(buffer);
            } finally {
                if (buffer != null) { Array.Clear(buffer, 0, buffer.Length); }
            }

            var remainders = new List<int>();
            while (input > 0) {
                var remainder = input % 58;
                remainders.Add((int)remainder);
                input /= 58;
            }

            //preserver leading zeros
            foreach (var b in value) {
                if (b == 0) { remainders.Add(0); } else { break; }
            }

            remainders.Reverse();

            var sbOutput = new StringBuilder();
            foreach (var remainder in remainders) {
                sbOutput.Append(Map[remainder]);
            }
            result = sbOutput.ToString();

            return true;
        }

        /// <summary>
        /// Returns true of conversion succeeded.
        /// </summary>
        /// <param name="value">Text to convert.</param>
        /// <param name="result">Converstion result as Base58 string.</param>
        /// <exception cref="NullReferenceException">Text cannot be null.</exception>
        public static bool TryToString(string value, out string result) {
            if (value == null) { throw new ArgumentNullException(nameof(value), "Text cannot be null."); }
            return TryToString(Utf8.GetBytes(value), out result);
        }

        #endregion ToString


        #region AsBytes

        /// <summary>
        /// Returns bytes based on their Base58 encoding.
        /// </summary>
        /// <param name="base58">Base58 encoded string.</param>
        /// <exception cref="NullReferenceException">Base58 string cannot be null.</exception>
        /// <exception cref="FormatException">Unknown character.</exception>
        public static byte[] AsBytes(string base58) {
            if (base58 == null) { throw new ArgumentNullException(nameof(base58), "Base58 string cannot be null."); }
            if (TryAsBytes(base58, out var result)) { return result; }
            throw new FormatException("Cannot convert.");
        }

        /// <summary>
        /// Returns true of conversion succeeded.
        /// </summary>
        /// <param name="base58">Base58 encoded string.</param>
        /// <param name="result">Conversion result as bytes.</param>
        /// <exception cref="NullReferenceException">Base58 string cannot be null.</exception>
        /// <exception cref="FormatException">Unknown character.</exception>
        public static bool TryAsBytes(string base58, out byte[] result) {
            if (base58 == null) { throw new ArgumentNullException(nameof(base58), "Base58 string cannot be null."); }

            if (string.IsNullOrEmpty(base58)) {
                result = new byte[0];
                return true;
            }

            var inStarting = true;
            var startingZeros = 0;
            var indices = new List<int>();
            foreach (var c in base58) {
                var index = Array.IndexOf(Map, c);
                if (index >= 0) {
                    if (inStarting && (index == 0)) {
                        startingZeros += 1;
                    } else {
                        inStarting = false;
                    }
                    indices.Add(index);
                } else {
                    throw new FormatException("Unknown character.");
                }
            }

            var output = new BigInteger();
            foreach (var index in indices) {
                output *= 58;
                output += index;
            }
            var outputBytes = output.ToByteArray();

            if (BitConverter.IsLittleEndian) { Array.Reverse(outputBytes); }

            try {
                var extraZeros = (outputBytes[0] == 0x00) ? startingZeros - 1 : startingZeros;
                var buffer = new byte[outputBytes.Length + extraZeros];
                if (extraZeros >= 0) {
                    Buffer.BlockCopy(outputBytes, 0, buffer, extraZeros, outputBytes.Length);
                } else {
                    Buffer.BlockCopy(outputBytes, -extraZeros, buffer, 0, buffer.Length);
                }
                result = buffer;
                return true;
            } finally {
                Array.Clear(outputBytes, 0, outputBytes.Length);
            }
        }

        #endregion AsBytes

        #region AsString

        /// <summary>
        /// Returns UTF-8 string based on it's Base58 encoding.
        /// </summary>
        /// <param name="base58">Base58 encoded string.</param>
        /// <exception cref="NullReferenceException">Base58 string cannot be null.</exception>
        /// <exception cref="FormatException">Unknown character.</exception>
        public static string AsString(string base58) {
            if (base58 == null) { throw new ArgumentNullException(nameof(base58), "Base58 string cannot be null."); }
            if (TryAsString(base58, out var result)) { return result; }
            throw new FormatException("Cannot convert.");
        }

        /// <summary>
        /// Returns true of conversion succeeded.
        /// </summary>
        /// <param name="base58">Base58 encoded string.</param>
        /// <param name="result">Conversion result as UTF-8 string.</param>
        /// <exception cref="NullReferenceException">Base58 string cannot be null.</exception>
        /// <exception cref="FormatException">Unknown character.</exception>
        public static bool TryAsString(string base58, out string result) {
            if (base58 == null) { throw new ArgumentNullException(nameof(base58), "Base58 string cannot be null."); }
            if (TryAsBytes(base58, out var bytes)) {
                result = Utf8.GetString(bytes);
                return true;
            } else {
                result = null!;
                return false;
            }
        }

        #endregion AsString

    }
}
