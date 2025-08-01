/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2021-11-25: Refactored to use pattern matching
//2021-11-01: Added support for DateTimeOffset
//2021-03-04: Refactored for .NET 5
//2017-09-17: Refactored for .NET Standard 2.0
//            Allowing custom DateTime for GetCode
//            Removing GetCode overload for various digit lengths - use Digits instead
//2015-02-12: Initial version

namespace Medo.Security.Cryptography;

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

/// <summary>
/// Implementation of HOTP (RFC 4226) and TOTP (RFC 6238) one-time password algorithms.
/// </summary>
/// <example>
/// <code>
/// var otp = new OneTimePassword("MZxw6\tyTboI=");
/// // or otp = new OneTimePassword();
/// // var secret = otp.GetBase32Secret(SecretFormatFlags.None));
/// if (otp.IsCodeValid(755224)) {
///     // do something
/// }
/// </code>
/// </example>
public sealed class OneTimePassword : IDisposable {

    /// <summary>
    /// Create new instance with random 160-bit secret.
    /// </summary>
    public OneTimePassword()
        : this(randomizeBuffer: true) {
        _secretLength = 20; //160 bits
        ProtectSecret();
    }

    /// <summary>
    /// Create new instance with predefined secret.
    /// </summary>
    /// <param name="secret">Secret. It should not be shorter than 128 bits (16 bytes). Minimum of 160 bits (20 bytes) is strongly recommended.</param>
    /// <exception cref="ArgumentNullException">Secret cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Secret cannot be longer than 8192 bits (1024 bytes).</exception>
    public OneTimePassword(byte[] secret)
        : this(false) {
        if (secret == null) { throw new ArgumentNullException(nameof(secret), "Secret cannot be null."); }
        if (secret.Length > MaxSecretLength) { throw new ArgumentOutOfRangeException(nameof(secret), "Secret cannot be longer than 8192 bits (1024 bytes)."); }

        Buffer.BlockCopy(secret, 0, _secretBuffer, 0, secret.Length);
        _secretLength = secret.Length;

        ProtectSecret();
    }

    /// <summary>
    /// Create new instance with predefined secret.
    /// </summary>
    /// <param name="secret">Secret in Base32 encoding. It should not be shorter than 128 bits (16 bytes). Minimum of 160 bits (20 bytes) is strongly recommended.</param>
    /// <exception cref="ArgumentNullException">Secret cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Secret is not a valid Base32 string. -or- Secret cannot be longer than 8192 bits (1024 bytes).</exception>
    public OneTimePassword(string secret)
        : this(false) {
        if (secret == null) { throw new ArgumentNullException(nameof(secret), "Secret cannot be null."); }

        try {
            FromBase32(secret, _secretBuffer, out _secretLength);
        } catch (IndexOutOfRangeException) {
            throw new ArgumentOutOfRangeException(nameof(secret), "Secret cannot be longer than 8192 bits (1024 bytes).");
        } catch (Exception) {
            throw new ArgumentOutOfRangeException(nameof(secret), "Secret is not a valid Base32 string.");
        }

        ProtectSecret();
    }

    private OneTimePassword(bool randomizeBuffer) {
        _secretBuffer = GC.AllocateUninitializedArray<byte>(MaxSecretLength, pinned: true);
        using var rng = RandomNumberGenerator.Create();
        if (randomizeBuffer) { rng.GetBytes(_secretBuffer); }

        _randomIV = new byte[16];
        RandomNumberGenerator.Create().GetBytes(_randomIV);

        _randomKey = new byte[16];
        RandomNumberGenerator.Create().GetBytes(_randomKey);
    }



    #region Setup

    private int _digits = 6;
    /// <summary>
    /// Gets/Sets number of digits to return.
    /// Number of digits should be kept between 6 and 8 for best results.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Number of digits to return must be between 4 and 9.</exception>
    public int Digits {
        get { return _digits; }
        set {
            if (value is < 4 or > 9) { throw new ArgumentOutOfRangeException(nameof(value), "Number of digits to return must be between 4 and 9."); }
            _digits = value;
        }
    }

    private int _timeStep = 30;
    /// <summary>
    /// Gets/sets time step in seconds for TOTP algorithm.
    /// Value must be between 15 and 300 seconds.
    /// If value is zero, time step won't be used and HOTP will be resulting protocol.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Time step must be between 15 and 300 seconds.</exception>
    public int TimeStep {
        get { return _timeStep; }
        set {
            if (value == 0) {
                _timeStep = 0;
                Counter = 0;
            } else {
                if (value is < 15 or > 300) { throw new ArgumentOutOfRangeException(nameof(value), "Time step must be between 15 and 300 seconds."); }
                _timeStep = value;
            }
        }
    }

    private long _counter = 0;
    /// <summary>
    /// Gets/sets counter value.
    /// Value can only be set in HOTP mode (if time step is zero).
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Counter value must be a positive number.</exception>
    /// <exception cref="NotSupportedException">Counter value can only be set in HOTP mode (time step is zero).</exception>
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
                if (value < 0) { throw new ArgumentOutOfRangeException(nameof(value), "Counter value must be a positive number."); }
                _counter = value;
            } else {
                throw new NotSupportedException("Counter value can only be set in HOTP mode (time step is zero).");
            }
        }
    }

    private static long GetTimeBasedCounter(DateTimeOffset time, int timeStep) {
        var seconds = time.ToUnixTimeSeconds();
        return (seconds / timeStep);
    }

    private OneTimePasswordAlgorithm _algorithm = OneTimePasswordAlgorithm.Sha1;
    /// <summary>
    /// Gets/sets crypto algorithm.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Unknown algorithm.</exception>
    public OneTimePasswordAlgorithm Algorithm {
        get { return _algorithm; }
        set {
            switch (value) {
                case OneTimePasswordAlgorithm.Sha1:
                case OneTimePasswordAlgorithm.Sha256:
                case OneTimePasswordAlgorithm.Sha512: break;
                default: throw new ArgumentOutOfRangeException(nameof(value), "Unknown algorithm.");
            }
            _algorithm = value;
        }
    }

    #endregion Setup


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
    /// <param name="time">UTC or Local time for code retrieval.</param>
    /// <exception cref="ArgumentOutOfRangeException">Time must be either UTC or Local.</exception>
    /// <exception cref="NotSupportedException">Cannot specify time in HOTP mode (time step is zero).</exception>
    public int GetCode(DateTime time) {
        if (time.Kind is not DateTimeKind.Utc and not DateTimeKind.Local) { throw new ArgumentOutOfRangeException(nameof(time), "Time must be either UTC or Local."); }
        return GetCode(new DateTimeOffset(time));
    }

    /// <summary>
    /// Returns code.
    /// Number of digits should be kept between 6 and 8 for best results.
    /// </summary>
    /// <param name="utcTime">UTC time for code retrieval.</param>
    /// <exception cref="NotSupportedException">Cannot specify time in HOTP mode (time step is zero).</exception>
    public int GetCode(DateTimeOffset time) {
        if (TimeStep == 0) { throw new NotSupportedException("Cannot specify time in HOTP mode (time step is zero)."); }
        return GetCode(Digits, GetTimeBasedCounter(time, TimeStep));
    }


    private int cachedDigits;
    private long cachedCounter = -1;
    private int cachedCode;

    private int GetCode(int digits, long counter) {
        if ((cachedCounter == counter) && (cachedDigits == digits)) { return cachedCode; } //to avoid recalculation if all is the same

        var counterBytes = BitConverter.GetBytes(counter);
        if (BitConverter.IsLittleEndian) { Array.Reverse(counterBytes, 0, 8); }

        byte[] hash;
        var secret = GetSecret();
        try {
            using HMAC hmac = Algorithm switch {
                OneTimePasswordAlgorithm.Sha512 => new HMACSHA512(secret),
                OneTimePasswordAlgorithm.Sha256 => new HMACSHA256(secret),
                _ => new HMACSHA1(secret),
            };
            hash = hmac.ComputeHash(counterBytes);
        } finally {
            ClearSecret(secret);
        }

        var offset = hash[^1] & 0x0F;
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

    #endregion Code


    #region Validate

    /// <summary>
    /// Returns true if code has been validated.
    /// </summary>
    /// <param name="code">Code to validate.</param>
    /// <exception cref="ArgumentOutOfRangeException">Code must contain only numbers and whitespace.</exception>
    /// <exception cref="ArgumentNullException">Code cannot be null.</exception>
    public bool IsCodeValid(string code) {
        if (code == null) { throw new ArgumentNullException(nameof(code), "Code cannot be null."); }
        var number = 0;
        foreach (var ch in code) {
            if (char.IsWhiteSpace(ch)) { continue; }
            if (!char.IsDigit(ch)) { throw new ArgumentOutOfRangeException(nameof(code), "Code must contain only numbers and whitespace."); }
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
    /// <exception cref="ArgumentOutOfRangeException">Time must be either UTC or Local.</exception>
    /// <exception cref="NotSupportedException">Cannot specify time in HOTP mode (time step is zero).</exception>
    public bool IsCodeValid(int code, DateTime time) {
        if (time.Kind is not DateTimeKind.Utc and not DateTimeKind.Local) { throw new ArgumentOutOfRangeException(nameof(time), "Time must be either UTC or Local."); }
        return IsCodeValid(code, new DateTimeOffset(time));
    }

    /// <summary>
    /// Returns true if code has been validated.
    /// In HOTP mode (time step is zero) counter will increased if code is valid.
    /// </summary>
    /// <param name="code">Code to validate.</param>
    /// <param name="utcTime">UTC time.</param>
    /// <exception cref="NotSupportedException">Cannot specify time in HOTP mode (time step is zero).</exception>
    public bool IsCodeValid(int code, DateTimeOffset time) {
        if (TimeStep == 0) { throw new NotSupportedException("Cannot specify time in HOTP mode (time step is zero)."); }

        var counter = GetTimeBasedCounter(time, TimeStep);
        var currCode = GetCode(Digits, counter);
        var prevCode = GetCode(Digits, counter - 1);

        var isCurrValid = (code == currCode);
        var isPrevValid = (code == prevCode) && (Counter > 0); //don't check previous code if counter is zero; but calculate it anyhow (to keep timing)
        var isValid = isCurrValid || isPrevValid;
        return isValid;
    }

    #endregion Validate


    #region Secret buffer

    /// <summary>
    /// Returns secret in byte array.
    /// It is up to the caller to secure the given byte array.
    /// </summary>
    public byte[] GetSecret() {
        var buffer = GC.AllocateUninitializedArray<byte>(_secretLength, pinned: true);

        UnprotectSecret();
        try {
            Buffer.BlockCopy(_secretBuffer, 0, buffer, 0, buffer.Length);
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
            return ToBase32(_secretBuffer, _secretLength, format);
        } finally {
            ProtectSecret();
        }
    }


    private const int MaxSecretLength = 1024;  // must be multiple of 8 for AES
    private readonly byte[] _secretBuffer;
    private readonly int _secretLength;

    private readonly byte[] _randomIV;
    private readonly byte[] _randomKey;
    private readonly Lazy<Aes> _aesAlgorithm = new(delegate {
        var aes = Aes.Create();
        aes.Padding = PaddingMode.None;
        return aes;
    });

    private void ProtectSecret() {  // essentially obfuscation as ProtectedData is not really portable
        var aes = _aesAlgorithm.Value;

        using var encryptor = aes.CreateEncryptor(_randomKey, _randomIV);
        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);

        cs.Write(_secretBuffer, 0, _secretBuffer.Length);
        Buffer.BlockCopy(ms.ToArray(), 0, _secretBuffer, 0, _secretBuffer.Length);
    }

    private void UnprotectSecret() {
        var aes = _aesAlgorithm.Value;

        using var decryptor = aes.CreateDecryptor(_randomKey, _randomIV);
        using var ms = new MemoryStream(_secretBuffer);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);

        cs.Read(_secretBuffer, 0, _secretBuffer.Length);
    }

    private static void ClearSecret(byte[] array) {
        for (var i = 0; i < array.Length; i++) {
            array[i] = 0;
        }
    }

    #endregion Secret buffer


    #region Base32

    private static readonly IList<char> Base32Alphabet = new List<char>("ABCDEFGHIJKLMNOPQRSTUVWXYZ234567").AsReadOnly();
    private static readonly byte[] Base32Bitmask = new byte[] { 0x00, 0x01, 0x03, 0x07, 0x0F, 0x1F };

    private static void FromBase32(string text, byte[] buffer, out int length) {
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

        if (bitPosition is > (-1) and >= 5) {
            partialByte <<= (8 - bitPosition);
            buffer[index] = partialByte;
            index++;
        }

        length = index;
    }

    private static string ToBase32(byte[] bytes, int length, SecretFormatFlags format) {
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

    #endregion Base32


    #region IDispose

    private bool disposedValue;

    private void Dispose(bool disposing) {
        if (!disposedValue) {
            ClearSecret(_secretBuffer);  // not unmanaged resource, but we want to get rid of data as soon as possible

            if (disposing) {
                ClearSecret(_randomKey);
                ClearSecret(_randomIV);
                if (_aesAlgorithm.IsValueCreated) { _aesAlgorithm.Value.Dispose(); }
            }

            disposedValue = true;
        }
    }

    public void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion IDispose

}



/// <summary>
/// Enumerates formatting option for secret.
/// </summary>
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
