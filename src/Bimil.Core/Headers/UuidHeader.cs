namespace Bimil;

using System;

/// <summary>
/// UUID header field.
/// </summary>
public sealed class UuidHeader : Header {

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="type">Type.</param>
    /// <param name="data">Data.</param>
    /// <param name="caption">Caption.</param>
    internal UuidHeader(HeaderType type, ProtectedBytes data, string caption)
       : base(type, data, caption) {
    }


    /// <summary>
    /// Gets or sets the UUID.
    /// </summary>
    public Guid Uuid {
        get {
            var bytes = Data.GetBytes();
            try {
                if (bytes.Length == 16) {
                    return new Guid(bytes);
                }
                return Guid.Empty;
            } finally {
                ProtectedBytes.ZeroMemory(bytes);
            }
        }
        set {
            var bytes = value.ToByteArray();
            Data.SetBytes(bytes, zeroBytes: true);
        }
    }

}
