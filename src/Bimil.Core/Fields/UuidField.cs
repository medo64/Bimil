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
    internal UuidField(FieldType type, ProtectedBytes data)
       : base(type, data) {
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
    /// Returns clone of the field.
    /// </summary>
    public override Field Clone() {
        return new UuidField(Type, Data.Clone());
    }


    /// <summary>
    /// Returns bytes based on the value provided.
    /// </summary>
    /// <param name="value">Value.</param>
    public static byte[] GetBytes(Guid value) {
        return value.ToByteArray(); ;
    }

}
