/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2017-09-17: Adjusting for .NET Standard 2.0.
//            Allowing custom DateTime for GetCode.
//            Removing GetCode overload for various digit lengths - use Digits instead.
//2015-02-12: Initial version.


using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace Medo.Security.Cryptography {

    /// <summary>
    /// Implementation of HOTP (RFC 4226) and TOTP (RFC 6238) one-time password algorithms.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "OneTime", Justification = "One time is more commonly written as two words.")]
    public class OneTimePassword {

        /// <summary>
        /// Create new instance with random 160-bit secret.
        /// </summary>
        public OneTimePassword() {
            using (var rng = RandomNumberGenerator.Create()) {
                rng.GetBytes(SecretBuffer);
            }
            SecretLength = 20; //160 bits
            ProtectSecret();
        }

        /// <summary>
        /// Create new instance with predefined secret.
        /// </summary>
        /// <param name="secret">Secret. It should not be shorter than 128 bits (16 bytes). Minimum of 160 bits (20 bytes) is strongly recommended.</param>
        /// <exception cref="System.ArgumentNullException">Secret cannot be null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Secret cannot be longer than 8192 bits (1024 bytes).</exception>
        public OneTimePassword(byte[] secret) {
            if (secret == null) { throw new ArgumentNullException("secret", "Secret cannot be null."); }
            if (secret.Length > SecretBuffer.Length) { throw new ArgumentOutOfRangeException("secret", "Secret cannot be longer than 8192 bits (1024 bytes)."); }

            Buffer.BlockCopy(secret, 0, SecretBuffer, 0, secret.Length);
            SecretLength = secret.Length;
            ProtectSecret();
        }

        /// <summary>
        /// Create new instance with predefined secret.
        /// </summary>
        /// <param name="secret">Secret in Base32 encoding. It should not be shorter than 128 bits (16 bytes). Minimum of 160 bits (20 bytes) is strongly recommended.</param>
        /// <exception cref="System.ArgumentNullException">Secret cannot be null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Secret is not valid Base32 string. -or- Secret cannot be longer than 8192 bits (1024 bytes).</exception>
        public OneTimePassword(string secret) {
            if (secret == null) { throw new ArgumentNullException("secret", "Secret cannot be null."); }

            try {
                FromBase32(secret, SecretBuffer, out var length);
                SecretLength = length;
            } catch (IndexOutOfRangeException) {
                throw new ArgumentOutOfRangeException("secret", "Secret cannot be longer than 8192 bits (1024 bytes).");
            } catch (Exception) {
                throw new ArgumentOutOfRangeException("secret", "Secret is not valid Base32 string.");
            }
            ProtectSecret();
        }


        #region Secret buffer

        private readonly byte[] SecretBuffer = new byte[1024]; //ProtectedMemory requires length of the data to be a multiple of 16 bytes.
        private readonly int SecretLength;

        private static readonly Lazy<byte[]> lazyRandomKey = new Lazy<byte[]>(() => {
            var buffer = new byte[16];
            RandomNumberGenerator.Create().GetBytes(buffer);
            return buffer;
        });
        private static readonly Lazy<Aes> aes = new Lazy<Aes>(() => {
            var aes = Aes.Create();
            aes.Padding = PaddingMode.None;
            return aes;
        });

        private void ProtectSecret() {
            using (var ms = new MemoryStream()) {
                using (var encryptor = aes.Value.CreateEncryptor(lazyRandomKey.Value, new byte[16]))
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write)) {
                    cs.Write(SecretBuffer, 0, SecretBuffer.Length);
                }
                Buffer.BlockCopy(ms.ToArray(), 0, SecretBuffer, 0, SecretBuffer.Length);
            }
        }

        private void UnprotectSecret() {
            using (var ms = new MemoryStream(SecretBuffer)) {
                using (var decryptor = aes.Value.CreateDecryptor(lazyRandomKey.Value, new byte[16]))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read)) {
                    var decryptedBuffer = new byte[SecretBuffer.Length];
                    var decryptedLength = cs.Read(decryptedBuffer, 0, decryptedBuffer.Length);
                    Buffer.BlockCopy(decryptedBuffer, 0, SecretBuffer, 0, SecretBuffer.Length);
                    Array.Clear(decryptedBuffer, 0, decryptedBuffer.Length);
                }
            }
        }


        /// <summary>
        /// Returns secret in byte array.
        /// It is up to the caller to secure given byte array.
        /// </summary>
        public byte[] GetSecret() {
            var buffer = new byte[SecretLength];

            UnprotectSecret();
            try {
                Buffer.BlockCopy(SecretBuffer, 0, buffer, 0, buffer.Length);
            } finally {
                ProtectSecret();
            }

            return buffer;
        }

        /// <summary>
        /// Returns secret as a Base32 string.
        /// String will be shown in quads and without padding.
        /// It is up to the caller to secure given string.
        /// </summary>
        public string GetBase32Secret() {
            return GetBase32Secret(SecretFormatFlags.Spacing);
        }

        /// <summary>
        /// Returns secret as a Base32 string with custom formatting.
        /// It is up to the caller to secure given string.
        /// </summary>
        /// <param name="format">Format of Base32 string.</param>
        public string GetBase32Secret(SecretFormatFlags format) {
            UnprotectSecret();
            try {
                return ToBase32(SecretBuffer, SecretLength, format);
            } finally {
                ProtectSecret();
            }
        }

        #endregion


        #region Setup

        private int _digits = 6;
        /// <summary>
        /// Gets/Sets number of digits to return.
        /// Number of digits should be kept between 6 and 8 for best results.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">Number of digits to return must be between 4 and 9.</exception>
        public int Digits {
            get { return _digits; }
            set {
                if ((value < 4) || (value > 9)) { throw new ArgumentOutOfRangeException("value", "Number of digits to return must be between 4 and 9."); }
                _digits = value;
            }
        }

        private int _timeStep = 30;
        /// <summary>
        /// Gets/sets time step in seconds for TOTP algorithm.
        /// Value must be between 15 and 300 seconds.
        /// If value is zero, time step won't be used and HOTP will be resulting protocol.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">Time step must be between 15 and 300 seconds.</exception>
        public int TimeStep {
            get { return _timeStep; }
            set {
                if (value == 0) {
                    _timeStep = 0;
                    Counter = 0;
                } else {
                    if ((value < 15) || (value > 300)) { throw new ArgumentOutOfRangeException("value", "Time step must be between 15 and 300 seconds."); }
                    _timeStep = value;
                }
            }
        }

        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private long _counter = 0;
        /// <summary>
        /// Gets/sets counter value.
        /// Value can only be set in HOTP mode (if time step is zero).
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">Counter value must be a positive number.</exception>
        /// <exception cref="System.NotSupportedException">Counter value can only be set in HOTP mode (time step is zero).</exception>
        public long Counter {
            get {
                if (TimeStep == 0) {
                    return _counter;
                } else {
                    return GetTimeBasedCounter(DateTime.UtcNow, TimeStep);
                }
            }
            set {
                if (TimeStep == 0) {
                    if (value < 0) { throw new ArgumentOutOfRangeException("value", "Counter value must be a positive number."); }
                    _counter = value;
                } else {
                    throw new NotSupportedException("Counter value can only be set in HOTP mode (time step is zero).");
                }
            }
        }

        private static long GetTimeBasedCounter(DateTime utcTime, int timeStep) {
            var seconds = (utcTime.Ticks - Epoch.Ticks) / 10000000;
            return (seconds / timeStep);
        }

        private OneTimePasswordAlgorithm _algorithm = OneTimePasswordAlgorithm.Sha1;
        /// <summary>
        /// Gets/sets crypto algorithm.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">Unknown algorithm.</exception>
        public OneTimePasswordAlgorithm Algorithm {
            get { return _algorithm; }
            set {
                switch (value) {
                    case OneTimePasswordAlgorithm.Sha1:
                    case OneTimePasswordAlgorithm.Sha256:
                    case OneTimePasswordAlgorithm.Sha512: break;
                    default: throw new ArgumentOutOfRangeException("value", "Unknown algorithm.");
                }
                _algorithm = value;
            }
        }

        #endregion


        #region Code

        /// <summary>
        /// Returns code.
        /// In HOTP mode (time step is zero), counter will be automatically increased. 
        /// </summary>
        public int GetCode() {
            var code = GetCode(Digits, Counter);
            if (TimeStep == 0) { Counter += 1; }
            return code;
        }

        /// <summary>
        /// Returns code.
        /// Number of digits should be kept between 6 and 8 for best results.
        /// </summary>
        /// <param name="utcTime">UTC time for code retrieval.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Time must be in UTC.</exception>
        /// <exception cref="System.NotSupportedException">Cannot specify time in HOTP mode (time step is zero).</exception>
        public int GetCode(DateTime utcTime) {
            if (utcTime.Kind != DateTimeKind.Utc) { throw new ArgumentOutOfRangeException("utcTime", "Time must be in UTC."); }
            if (TimeStep == 0) { throw new NotSupportedException("Cannot specify time in HOTP mode (time step is zero)."); }

            return GetCode(Digits, OneTimePassword.GetTimeBasedCounter(utcTime, TimeStep));
        }


        private int cachedDigits;
        private long cachedCounter = -1;
        private int cachedCode;

        private int GetCode(int digits, long counter) {
            if ((cachedCounter == counter) && (cachedDigits == digits)) { return cachedCode; } //to avoid recalculation if all is the same

            byte[] hash;
            var secret = GetSecret();
            try {
                var counterBytes = BitConverter.GetBytes(counter);
                if (BitConverter.IsLittleEndian) { Array.Reverse(counterBytes, 0, 8); }
                HMAC hmac = null;
                try {
                    switch (Algorithm) {
                        case OneTimePasswordAlgorithm.Sha1: hmac = new HMACSHA1(secret); break;
                        case OneTimePasswordAlgorithm.Sha256: hmac = new HMACSHA256(secret); break;
                        case OneTimePasswordAlgorithm.Sha512: hmac = new HMACSHA512(secret); break;
                    }
                    hash = hmac.ComputeHash(counterBytes);
                } finally {
                    if (hmac != null) { hmac.Dispose(); }
                }
            } finally {
                Array.Clear(secret, 0, secret.Length);
            }

            var offset = hash[hash.Length - 1] & 0x0F;
            var truncatedHash = new byte[] { (byte)(hash[offset + 0] & 0x7F), hash[offset + 1], hash[offset + 2], hash[offset + 3] };
            if (BitConverter.IsLittleEndian) { Array.Reverse(truncatedHash, 0, 4); }
            var number = BitConverter.ToInt32(truncatedHash, 0);
            var code = number % DigitsDivisor[digits];

            cachedCounter = counter;
            cachedDigits = digits;
            cachedCode = code;

            return code;
        }

        private static readonly int[] DigitsDivisor = new int[] { 0, 0, 0, 0, 10000, 100000, 1000000, 10000000, 100000000, 1000000000 };

        #endregion


        #region Validate

        /// <summary>
        /// Returns true if code has been validated.
        /// </summary>
        /// <param name="code">Code to validate.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Code must contain only numbers and whitespace.</exception>
        /// <exception cref="System.ArgumentNullException">Code cannot be null.</exception>
        public bool IsCodeValid(string code) {
            if (code == null) { throw new ArgumentNullException("code", "Code cannot be null."); }
            var number = 0;
            foreach (var ch in code) {
                if (char.IsWhiteSpace(ch)) { continue; }
                if (!char.IsDigit(ch)) { throw new ArgumentOutOfRangeException("code", "Code must contain only numbers and whitespace."); }
                if (number >= 100000000) { return false; } //number cannot be more than 9 digits
                number *= 10;
                number += (ch - 0x30);
            }
            return IsCodeValid(number);
        }

        /// <summary>
        /// Returns true if code has been validated.
        /// In HOTP mode (time step is zero) counter will increased if code is valid.
        /// </summary>
        /// <param name="code">Code to validate.</param>
        public bool IsCodeValid(int code) {
            var currCode = GetCode(Digits, Counter);
            var prevCode = GetCode(Digits, Counter - 1);

            var isCurrValid = (code == currCode);
            var isPrevValid = (code == prevCode) && (Counter > 0); //don't check previous code if counter is zero; but calculate it anyhow (to keep timing)
            var isValid = isCurrValid || isPrevValid;
            if ((TimeStep == 0) && isValid) {
                Counter++;
            }
            return isValid;
        }

        /// <summary>
        /// Returns true if code has been validated.
        /// In HOTP mode (time step is zero) counter will increased if code is valid.
        /// </summary>
        /// <param name="code">Code to validate.</param>
        /// <param name="utcTime">UTC time.</param>
        /// <exception cref="ArgumentOutOfRangeException">Time must be in UTC.</exception>
        /// <exception cref="NotSupportedException">Cannot specify time in HOTP mode (time step is zero).</exception>
        public bool IsCodeValid(int code, DateTime utcTime) {
            if (utcTime.Kind != DateTimeKind.Utc) { throw new ArgumentOutOfRangeException("utcTime", "Time must be in UTC."); }
            if (TimeStep == 0) { throw new NotSupportedException("Cannot specify time in HOTP mode (time step is zero)."); }

            var counter = GetTimeBasedCounter(utcTime, TimeStep);

            var currCode = GetCode(Digits, counter);
            var prevCode = GetCode(Digits, counter - 1);

            var isCurrValid = (code == currCode);
            var isPrevValid = (code == prevCode) && (Counter > 0); //don't check previous code if counter is zero; but calculate it anyhow (to keep timing)
            var isValid = isCurrValid || isPrevValid;
            return isValid;
        }

        #endregion


        #region Base32

        private static readonly IList<char> Base32Alphabet = new List<char>("ABCDEFGHIJKLMNOPQRSTUVWXYZ234567").AsReadOnly();
        private static readonly byte[] Base32Bitmask = new byte[] { 0x00, 0x01, 0x03, 0x07, 0x0F, 0x1F };

        internal static void FromBase32(string text, byte[] buffer, out int length) {
            var index = 0;

            var bitPosition = 0;
            byte partialByte = 0;
            foreach (var ch in text) { //always assume padding - easier to code than actually checking
                if (char.IsWhiteSpace(ch)) { continue; } //ignore whitespaces
                if (ch == '=') { // finish up
                    bitPosition = -1;
                    continue;
                } else if (bitPosition == -1) { throw new FormatException("Character '" + ch + "' found after padding ."); }

                var bits = Base32Alphabet.IndexOf(char.ToUpperInvariant(ch));
                if (bits < 0) { throw new FormatException("Unknown character '" + ch + "'."); }

                var bitCount1 = (bitPosition < 3) ? 5 : 8 - bitPosition; //how many bits go in current partial byte
                var bitCount2 = 5 - bitCount1; //how many bits are for next byte

                partialByte <<= bitCount1;
                partialByte |= (byte)(bits >> (5 - bitCount1));
                bitPosition += bitCount1;

                if (bitPosition >= 8) {
                    buffer[index] = partialByte;
                    index++;
                    bitPosition = bitCount2;
                    partialByte = (byte)(bits & Base32Bitmask[bitCount2]);
                }
            }

            if ((bitPosition > -1) && (bitPosition >= 5)) {
                partialByte <<= (8 - bitPosition);
                buffer[index] = partialByte;
                index++;
            }

            length = index;
        }

        internal static string ToBase32(byte[] bytes, int length, SecretFormatFlags format) {
            if (length == 0) { return string.Empty; }

            var hasSpacing = (format & SecretFormatFlags.Spacing) == SecretFormatFlags.Spacing;
            var hasPadding = (format & SecretFormatFlags.Padding) == SecretFormatFlags.Padding;
            var isUpper = (format & SecretFormatFlags.Uppercase) == SecretFormatFlags.Uppercase;

            var bitLength = (length * 8);
            var textLength = bitLength / 5 + ((bitLength % 5) == 0 ? 0 : 1);
            var totalLength = textLength;

            var padLength = (textLength % 8 == 0) ? 0 : 8 - textLength % 8;
            totalLength += (hasPadding ? padLength : 0);

            var spaceLength = totalLength / 4 + ((totalLength % 4 == 0) ? -1 : 0);
            totalLength += (hasSpacing ? spaceLength : 0);


            var chars = new char[totalLength];
            var index = 0;

            var bits = 0;
            var bitsRemaining = 0;
            for (var i = 0; i < length; i++) {
                bits = (bits << 8) | bytes[i];
                bitsRemaining += 8;
                while (bitsRemaining >= 5) {
                    var bitsIndex = (bits >> (bitsRemaining - 5)) & 0x1F;
                    bitsRemaining -= 5;
                    chars[index] = isUpper ? Base32Alphabet[bitsIndex] : char.ToLowerInvariant(Base32Alphabet[bitsIndex]);
                    index++;

                    if (hasSpacing && (index < chars.Length) && (bitsRemaining % 4 == 0)) {
                        chars[index] = ' ';
                        index++;
                    }
                }
            }
            if (bitsRemaining > 0) {
                var bitsIndex = (bits & Base32Bitmask[bitsRemaining]) << (5 - bitsRemaining);
                chars[index] = isUpper ? Base32Alphabet[bitsIndex] : char.ToLowerInvariant(Base32Alphabet[bitsIndex]);
                index++;
            }

            if (hasPadding) {
                for (var i = 0; i < padLength; i++) {
                    if (hasSpacing && (i % 4 == padLength % 4)) {
                        chars[index] = ' ';
                        index++;
                    }
                    chars[index] = '=';
                    index++;
                }
            }

            return new string(chars);
        }

        #endregion

    }



    /// <summary>
    /// Enumerates formatting option for secret.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Identifier name is intentional.")]
    [Flags()]
    public enum SecretFormatFlags {
        /// <summary>
        /// Secret will be returned as a minimal Base32 string.
        /// </summary>
        None = 0,
        /// <summary>
        /// Secret will have space every four characters.
        /// </summary>
        Spacing = 1,
        /// <summary>
        /// Secret will be properly padded to full Base32 length.
        /// </summary>
        Padding = 2,
        /// <summary>
        /// Secret will be returned in upper case characters.
        /// </summary>
        Uppercase = 4,
    }



    /// <summary>
    /// Algorithm for generating one time password.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "OneTime", Justification = "One time is more commonly written as two words.")]
    public enum OneTimePasswordAlgorithm {
        /// <summary>
        /// SHA-1.
        /// </summary>
        Sha1 = 0,
        /// <summary>
        /// SHA-256.
        /// </summary>
        Sha256 = 1,
        /// <summary>
        /// SHA-512.
        /// </summary>
        Sha512 = 2,
    }

}
