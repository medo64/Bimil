namespace Bimil;

using System;
using System.Text;

/// <summary>
/// Text record field.
/// </summary>
public sealed class TextField : Field, IReadableCaption, IReadableText, IWritableText, IReadableIsSensitive {

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="type">Type.</param>
    /// <param name="data">Data.</param>
    internal TextField(FieldType type, ProtectedBytes data, bool isSensitive)
       : base(type, data) {
        IsSensitive = isSensitive;
    }


    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    public string Text {
        get {
            var bytes = Data.GetBytes();
            try {
                return Encoding.UTF8.GetString(bytes);
            } finally {
                ProtectedBytes.ZeroMemory(bytes);
            }
        }
        set {
            Data.SetBytes(GetBytes(value), zeroBytes: true);
        }
    }

    /// <summary>
    /// Gets if field is considered sensitive.
    /// </summary>
    public bool IsSensitive { get; }


    /// <summary>
    /// Returns clone of the field.
    /// </summary>
    public override Field Clone() {
        return new TextField(Type, Data.Clone(), IsSensitive);
    }


    /// <summary>
    /// Returns bytes based on the value provided.
    /// </summary>
    /// <param name="value">Value.</param>
    public static byte[] GetBytes(string value) {
        return Encoding.UTF8.GetBytes(value ?? "");
    }

}
