namespace Bimil;

using System;

/// <summary>
/// UUID header field.
/// </summary>
public sealed class UuidField : Field {

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="type">Type.</param>
    /// <param name="data">Data.</param>
    /// <param name="caption">Caption.</param>
    internal UuidField(FieldType type, ProtectedBytes data, string caption)
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
            Data.SetBytes(GetBytes(value), zeroBytes: true);
        }
    }


    /// <summary>
    /// Returns bytes based on the value provided.
    /// </summary>
    /// <param name="value">Value.</param>
    public static byte[] GetBytes(Guid value) {
        return value.ToByteArray(); ;
    }

}
