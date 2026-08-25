namespace Bimil;

using System;

/// <summary>
/// Version header field.
/// </summary>
public sealed class VersionHeader : Header {

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="type">Type.</param>
    /// <param name="data">Data.</param>
    /// <param name="caption">Caption.</param>
    internal VersionHeader(HeaderType type, ProtectedBytes data, string caption)
        : base(type, data, caption) {
    }


    /// <summary>
    /// Gets or sets the version.
    /// </summary>
    public Version Version {
        get {
            var bytes = Data.GetBytes();
            try {
                if (bytes.Length == 2) {
                    return new Version(bytes[1], bytes[0], 0, 0);
                }
                return new Version(0, 0, 0, 0);
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
    public static byte[] GetBytes(Version value) {
        if (value.Major is < 0 or > 255) { throw new ArgumentOutOfRangeException(nameof(value), "Major version must be between 0 and 255."); }
        if (value.Minor is < 0 or > 255) { throw new ArgumentOutOfRangeException(nameof(value), "Minor version must be between 0 and 255."); }
        //if (value.Build is not 0) { throw new ArgumentOutOfRangeException(nameof(value), "Build version must be 0."); }
        //if (value.Revision is not 0) { throw new ArgumentOutOfRangeException(nameof(value), "Revision version must be 0."); }
        return [(byte)(value.Minor), (byte)(value.Major)];
    }

}
