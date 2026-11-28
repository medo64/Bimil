namespace Bimil;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

/// <summary>
/// Base32 encoder/decoder.
/// RUDE stands for: Readable User-friendly Data Encoding
/// </summary>
internal static class Rude32 {

    /// <summary>
    /// Returns base 32 encoded text.
    /// </summary>
    /// <param name="source">Bytes to encode.</param>
    public static string ToString(ReadOnlySpan<byte> source) {
        return ToString(source, includeChecksumInPadding: false);
    }

    /// <summary>
    /// Returns base 32 encoded text.
    /// Please note that checksum included in padding is not the same checksum as 
    /// </summary>
    /// <param name="source">Bytes to encode.</param>
    /// <param name="includeChecksumInPadding">If true, checksum will be included in padding but only if 2 bits or more of padding is available.</param>
    public static string ToString(ReadOnlySpan<byte> source, bool includeChecksumInPadding) {
        if (TryToString(source, out var result, includeChecksumInPadding)) {
            return result;
        } else {
            throw new FormatException("Cannot convert to destination.");
        }
    }

    /// <summary>
    /// Returns true if base 32 source can be parsed.
    /// Any character that is not digit or letter will be ignored.
    /// </summary>
    /// <param name="source">Bytes to encode.</param>
    /// <param name="result">Encoded output.</param>
    public static bool TryToString(ReadOnlySpan<byte> source, [NotNullWhen(true)] out string result) {
        return TryToString(source, out result, includeChecksumInPadding: false);
    }

    /// <summary>
    /// Returns true if base 32 source can be parsed.
    /// Any character that is not digit or letter will be ignored.
    /// </summary>
    /// <param name="source">Bytes to encode.</param>
    /// <param name="result">Encoded output.</param>
    /// <param name="includeChecksumInPadding">If true, checksum will be included in padding but only if 2 bits or more of padding is available.</param>
    public static bool TryToString(ReadOnlySpan<byte> source, [NotNullWhen(true)] out string result, bool includeChecksumInPadding) {
        (var quotient, var remainder) = Math.DivRem(source.Length * 8, 5);
        var charCount = remainder == 0 ? quotient : quotient + 1;
        var destination = new char[charCount];
        if (TryToString(source, destination, out var charsWritten, includeChecksumInPadding)) {
            result = new string(destination);
            return true;
        } else {
            result = string.Empty;
            return false;
        }
    }

    /// <summary>
    /// Returns base 32 encoded text.
    /// Please note that checksum included in padding is not the same checksum as 
    /// </summary>
    /// <param name="source">Bytes to encode.</param>
    /// <param name="destination">Encoding destination.</param>
    public static bool TryToString(ReadOnlySpan<byte> source, Span<char> destination) {
        return TryToString(source, destination, out _, includeChecksumInPadding: false);
    }

    /// <summary>
    /// Returns base 32 encoded text.
    /// Please note that checksum included in padding is not the same checksum as 
    /// </summary>
    /// <param name="source">Bytes to encode.</param>
    /// <param name="destination">Encoding destination.</param>
    /// <param name="includeChecksumInPadding">If true, checksum will be included in padding but only if 2 bits or more of padding is available.</param>
    public static bool TryToString(ReadOnlySpan<byte> source, Span<char> destination, bool includeChecksumInPadding) {
        return TryToString(source, destination, out _, includeChecksumInPadding);
    }

    /// <summary>
    /// Returns base 32 encoded text.
    /// Please note that checksum included in padding is not the same checksum as 
    /// </summary>
    /// <param name="source">Bytes to encode.</param>
    /// <param name="destination">Encoding destination.</param>
    /// <param name="charsWritten">Number of characters written to destination.</param>
    /// <param name="includeChecksumInPadding">If true, checksum will be included in padding but only if 2 bits or more of padding is available.</param>
    public static bool TryToString(ReadOnlySpan<byte> source, Span<char> destination, out int charsWritten) {
        return TryToString(source, destination, out charsWritten, includeChecksumInPadding: false);
    }

    /// <summary>
    /// Returns base 32 encoded text.
    /// Please note that checksum included in padding is not the same checksum as 
    /// </summary>
    /// <param name="source">Bytes to encode.</param>
    /// <param name="destination">Encoding destination.</param>
    /// <param name="charsWritten">Number of characters written to destination.</param>
    /// <param name="includeChecksumInPadding">If true, checksum will be included in padding but only if 2 bits or more of padding is available.</param>
    public static bool TryToString(ReadOnlySpan<byte> source, Span<char> destination, out int charsWritten, bool includeChecksumInPadding) {
        if (includeChecksumInPadding) {
            return TryWriteWithChecksum(source, destination, out charsWritten);
        } else {
            return TryWriteWithoutChecksum(source, destination, out charsWritten);
        }
    }


    /// <summary>
    /// Returns byte array from base 32 source.
    /// Any bytes higher than 127 will stop parsing.
    /// Any character that is not digit or letter will be ignored.
    /// </summary>
    /// <param name="source">Base 32 encoded text.</param>
    public static byte[] FromString(string source) {
        if (TryFromString(source, out var result)) { return result; }
        throw new FormatException("Cannot convert from source.");
    }

    /// <summary>
    /// Returns true if base 32 source can be parsed.
    /// Any character that is not digit or letter will be ignored.
    /// </summary>
    /// <param name="source">Base 32 encoded text.</param>
    /// <param name="result">Output bytes.</param>
    public static bool TryFromString(string source, [NotNullWhen(true)] out byte[] result) {
        return TryFromString(source.AsSpan(), out result);
    }

    /// <summary>
    /// Returns true if base 32 source can be parsed.
    /// Any character that is not digit or letter will be ignored.
    /// </summary>
    /// <param name="source">Base 32 encoded text.</param>
    /// <param name="result">Output bytes.</param>
    public static bool TryFromString(ReadOnlySpan<char> source, [NotNullWhen(true)] out byte[] result) {
        var buffer = new byte[source.Length * 5 / 8 + 1];
        if (TryFromString(source, buffer.AsSpan(), out var bytesWritten)) {
            result = new byte[bytesWritten];
            Buffer.BlockCopy(buffer, 0, result, 0, result.Length);
            return true;
        } else {
            result = [];
            return false;
        }
    }

    /// <summary>
    /// Returns true if base 32 source can be parsed.
    /// Any character that is not digit or letter will be ignored.
    /// </summary>
    /// <param name="source">Base 32 encoded characters.</param>
    /// <param name="destination">Output bytes.</param>
    public static bool TryFromString(ReadOnlySpan<char> source, Span<byte> destination) {
        return TryRead(source, destination, out _);
    }

    /// <summary>
    /// Returns true if base 32 source can be parsed.
    /// Any character that is not digit or letter will be ignored.
    /// </summary>
    /// <param name="source">Base 32 encoded characters.</param>
    /// <param name="destination">Output bytes.</param>
    /// <param name="bytesWritten">Number of bytes writtend to destination.</param>
    public static bool TryFromString(ReadOnlySpan<char> source, Span<byte> destination, out int bytesWritten) {
        return TryRead(source, destination, out bytesWritten);
    }

    /// <summary>
    /// Returns true if base 32 source can be parsed.
    /// Any character that is not ASCII digit or letter will be ignored.
    /// </summary>
    /// <param name="source">Base 32 encoded characters.</param>
    /// <param name="destination">Output bytes.</param>
    public static bool TryFromString(ReadOnlySpan<byte> source, Span<byte> destination) {
        return TryRead(source, destination, out _);
    }

    /// <summary>
    /// Returns true if base 32 source can be parsed.
    /// Any character that is not ASCII digit or letter will be ignored.
    /// </summary>
    /// <param name="source">Base 32 encoded characters.</param>
    /// <param name="destination">Output bytes.</param>
    /// <param name="bytesWritten">Number of bytes writtend to destination.</param>
    public static bool TryFromString(ReadOnlySpan<byte> source, Span<byte> destination, out int bytesWritten) {
        return TryRead(source, destination, out bytesWritten);
    }


    #region Implementation

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryWriteWithoutChecksum(ReadOnlySpan<byte> source, Span<char> destination, out int charsWritten) {
        var reservoir = 0;
        var level = 0;
        var srcIndex = 0;
        var done = false;
        for (var dstIndex = 0; dstIndex < destination.Length; dstIndex++) {
            if (level < 5) {  // load new byte
                byte data;
                if (srcIndex < source.Length) {
                    data = source[srcIndex++];
                } else {
                    data = 0;
                    done = true;
                }
                level += 8;
                reservoir = (reservoir << 8) | data;
            }
            level -= 5;
            var alphabetIndex = (reservoir >> level) & 0b11111;
            destination[dstIndex] = alphabetIndex switch {
                0b00000 => '0',
                0b00001 => '1',
                0b00010 => '2',
                0b00011 => '3',
                0b00100 => '4',
                0b00101 => '5',
                0b00110 => '6',
                0b00111 => '7',
                0b01000 => '8',
                0b01001 => '9',
                0b01010 => 'A',
                0b01011 => 'B',
                0b01100 => 'C',
                0b01101 => 'D',
                0b01110 => 'E',
                0b01111 => 'F',
                0b10000 => 'G',
                0b10001 => 'H',
                0b10010 => 'J',
                0b10011 => 'K',
                0b10100 => 'M',
                0b10101 => 'N',
                0b10110 => 'P',
                0b10111 => 'Q',
                0b11000 => 'R',
                0b11001 => 'T',
                0b11010 => 'U',
                0b11011 => 'V',
                0b11100 => 'W',
                0b11101 => 'X',
                0b11110 => 'Y',
                0b11111 => 'Z',
                _ => ' ',  // will not happen
            };

            if (done) {
                charsWritten = dstIndex + 1;
                return true;
            }
        }
        charsWritten = destination.Length;
        return (level == 0);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryWriteWithChecksum(ReadOnlySpan<byte> source, Span<char> destination, out int charsWritten) {
        var reservoir = 0;
        var level = 0;
        var sumA = 1;
        var sumB = 0;
        var srcIndex = 0;
        var done = false;
        for (var dstIndex = 0; dstIndex < destination.Length; dstIndex++) {
            if (level < 5) {  // load new byte
                byte data;
                if (srcIndex < source.Length) {
                    data = source[srcIndex++];
                    sumA += data;
                    sumB += sumA;
                } else {
                    data = level switch {
                        1 => (byte)((1 + sumB % 15) << 4),
                        2 => (byte)((1 + sumB % 7) << 5),
                        3 => (byte)((1 + sumB % 3) << 6),
                        4 => 0,
                        _ => 0,
                    };
                    done = true;
                }
                level += 8;
                reservoir = (reservoir << 8) | data;
            }
            level -= 5;
            var alphabetIndex = (reservoir >> level) & 0b11111;
            destination[dstIndex] = alphabetIndex switch {
                0b00000 => '0',
                0b00001 => '1',
                0b00010 => '2',
                0b00011 => '3',
                0b00100 => '4',
                0b00101 => '5',
                0b00110 => '6',
                0b00111 => '7',
                0b01000 => '8',
                0b01001 => '9',
                0b01010 => 'A',
                0b01011 => 'B',
                0b01100 => 'C',
                0b01101 => 'D',
                0b01110 => 'E',
                0b01111 => 'F',
                0b10000 => 'G',
                0b10001 => 'H',
                0b10010 => 'J',
                0b10011 => 'K',
                0b10100 => 'M',
                0b10101 => 'N',
                0b10110 => 'P',
                0b10111 => 'Q',
                0b11000 => 'R',
                0b11001 => 'T',
                0b11010 => 'U',
                0b11011 => 'V',
                0b11100 => 'W',
                0b11101 => 'X',
                0b11110 => 'Y',
                0b11111 => 'Z',
                _ => ' ',  // will not happen
            };

            if (done) {
                charsWritten = dstIndex + 1;
                return true;
            }
        }
        charsWritten = destination.Length;
        return (level == 0);
    }

    private static bool TryRead(ReadOnlySpan<char> source, Span<byte> destination, out int bytesWritten) {
        var dstIndex = 0;
        var sumA = 1;
        var sumB = 0;
        var reservoir = 0;
        var level = 0;
        for (var srcIndex = 0; srcIndex < source.Length; srcIndex++) {
            var alphabetIndex = source[srcIndex] switch {
                '0' or 'O' or 'o' => 0b00000,
                '1' or 'I' or 'i' or 'L' or 'l' => 0b00001,
                '2' => 0b00010,
                '3' => 0b00011,
                '4' => 0b00100,
                '5' or 'S' or 's' => 0b00101,
                '6' => 0b00110,
                '7' => 0b00111,
                '8' => 0b01000,
                '9' => 0b01001,
                'A' or 'a' => 0b01010,
                'B' or 'b' => 0b01011,
                'C' or 'c' => 0b01100,
                'D' or 'd' => 0b01101,
                'E' or 'e' => 0b01110,
                'F' or 'f' => 0b01111,
                'G' or 'g' => 0b10000,
                'H' or 'h' => 0b10001,
                'J' or 'j' => 0b10010,
                'K' or 'k' => 0b10011,
                'M' or 'm' => 0b10100,
                'N' or 'n' => 0b10101,
                'P' or 'p' => 0b10110,
                'Q' or 'q' => 0b10111,
                'R' or 'r' => 0b11000,
                'T' or 't' => 0b11001,
                'U' or 'u' => 0b11010,
                'V' or 'v' => 0b11011,
                'W' or 'w' => 0b11100,
                'X' or 'x' => 0b11101,
                'Y' or 'y' => 0b11110,
                'Z' or 'z' => 0b11111,
                _ => -1,
            };
            if (alphabetIndex == -1) { continue; }

            reservoir = (reservoir << 5) | alphabetIndex;
            level += 5;
            if (level >= 8) {  // enough for a byte
                level -= 8;
                var data = (byte)(reservoir >> level);
                if (dstIndex == destination.Length) {
                    bytesWritten = dstIndex;
                    return false;
                }
                destination[dstIndex++] = (byte)data;
                sumA += data;
                sumB += sumA;
            }
        }
        bytesWritten = dstIndex;
        var checksum = level switch {
            0 => 0,
            1 => 0,
            2 => reservoir & 0b11,
            3 => reservoir & 0b111,
            4 => reservoir & 0b1111,
            _ => 0,
        };
        if (checksum > 0) {
            var expected = level switch {
                2 => (byte)(1 + sumB % 3),
                3 => (byte)(1 + sumB % 7),
                4 => (byte)(1 + sumB % 15),
                _ => 0,
            };
            return (checksum == expected);
        }
        return true;
    }

    private static bool TryRead(ReadOnlySpan<byte> source, Span<byte> destination, out int bytesWritten) {
        var dstIndex = 0;
        var sumA = 1;
        var sumB = 0;
        var reservoir = 0;
        var level = 0;
        for (var srcIndex = 0; srcIndex < source.Length; srcIndex++) {
            var alphabetIndex = source[srcIndex] switch {
                0x30 or 0x4F or 0x6F => 0b00000,
                0x31 or 0x49 or 0x69 or 0x4C or 0x6C => 0b00001,
                0x32 => 0b00010,
                0x33 => 0b00011,
                0x34 => 0b00100,
                0x35 or 0x53 or 0x73 => 0b00101,
                0x36 => 0b00110,
                0x37 => 0b00111,
                0x38 => 0b01000,
                0x39 => 0b01001,
                0x41 or 0x61 => 0b01010,
                0x42 or 0x62 => 0b01011,
                0x43 or 0x63 => 0b01100,
                0x44 or 0x64 => 0b01101,
                0x45 or 0x65 => 0b01110,
                0x46 or 0x66 => 0b01111,
                0x47 or 0x67 => 0b10000,
                0x48 or 0x68 => 0b10001,
                0x4A or 0x6A => 0b10010,
                0x4B or 0x6B => 0b10011,
                0x4D or 0x6D => 0b10100,
                0x4E or 0x6E => 0b10101,
                0x50 or 0x70 => 0b10110,
                0x51 or 0x71 => 0b10111,
                0x52 or 0x72 => 0b11000,
                0x54 or 0x74 => 0b11001,
                0x55 or 0x75 => 0b11010,
                0x56 or 0x76 => 0b11011,
                0x57 or 0x77 => 0b11100,
                0x58 or 0x78 => 0b11101,
                0x59 or 0x79 => 0b11110,
                0x5A or 0x7A => 0b11111,
                _ => -1,
            };
            if (alphabetIndex == -1) { continue; }

            reservoir = (reservoir << 5) | alphabetIndex;
            level += 5;
            if (level >= 8) {  // enough for a byte
                level -= 8;
                var data = (byte)(reservoir >> level);
                if (dstIndex == destination.Length) {
                    bytesWritten = dstIndex;
                    return false;
                }
                destination[dstIndex++] = (byte)data;
                sumA += data;
                sumB += sumA;
            }
        }
        bytesWritten = dstIndex;
        var checksum = level switch {
            0 => 0,
            1 => 0,
            2 => reservoir & 0b11,
            3 => reservoir & 0b111,
            4 => reservoir & 0b1111,
            _ => 0,
        };
        if (checksum > 0) {
            var expected = level switch {
                2 => (byte)(1 + sumB % 3),
                3 => (byte)(1 + sumB % 7),
                4 => (byte)(1 + sumB % 15),
                _ => 0,
            };
            return (checksum == expected);
        }
        return true;
    }

    #endregion Implementation

}
