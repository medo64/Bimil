namespace Bimil;

using System;
using System.Buffers.Binary;
using System.Globalization;
using System.Text;

/// <summary>
/// Timestamp header field.
/// </summary>
public sealed class TimestampHeader : Header {

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="type">Type.</param>
    /// <param name="data">Data.</param>
    /// <param name="caption">Caption.</param>
    internal TimestampHeader(HeaderType type, ProtectedBytes data, string caption)
       : base(type, data, caption) {
    }


    /// <summary>
    /// Gets or sets the timestamp.
    /// </summary>
    public DateTime Timestamp {
        get {
            var bytes = Data.GetBytes();
            try {
                if (bytes.Length == 4) {
                    var seconds = BinaryPrimitives.ReadUInt32LittleEndian(bytes);
                    return DateTime.UnixEpoch.AddSeconds(seconds);
                } else if (bytes.Length == 8) { //try hexadecimal
                    if (uint.TryParse(Encoding.UTF8.GetString(bytes), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var seconds)) {
                        return DateTime.UnixEpoch.AddSeconds(seconds);
                    } else {
                        return DateTime.MinValue;
                    }
                }
                return DateTime.MinValue;
            } finally {
                ProtectedBytes.ZeroMemory(bytes);
            }
        }
        set {
            Data.SetBytes(GetBytes(value), zeroBytes: true);
        }
    }


    /// <summary>
    /// Returns bytes based on the value provided.
    /// </summary>
    /// <param name="value">Value.</param>
    public static byte[] GetBytes(DateTime value) {
        if ((value < DateTime.UnixEpoch) || (value > DateTime.UnixEpoch.AddSeconds(uint.MaxValue))) { throw new ArgumentNullException(nameof(value), "Time outside of allowable range."); }
        var seconds = (uint)((value.ToUniversalTime() - DateTime.UnixEpoch).TotalSeconds);
        var bytes = new byte[4];
        BinaryPrimitives.WriteUInt32LittleEndian(bytes, seconds);
        return bytes;
    }

}
