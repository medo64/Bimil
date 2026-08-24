namespace Bimil;

using System;
using System.Text;

/// <summary>
/// Text record field.
/// </summary>
public sealed class TextField : Field {

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="type">Type.</param>
    /// <param name="data">Data.</param>
    /// <param name="caption">Caption.</param>
    internal TextField(FieldType type, ProtectedBytes data, string caption)
       : base(type, data, caption) {
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
            var bytes = Encoding.UTF8.GetBytes(value);
            Data.SetBytes(bytes, zeroBytes: true);
        }
    }

}
