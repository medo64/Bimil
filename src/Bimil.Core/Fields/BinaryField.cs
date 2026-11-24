namespace Bimil;

using System;

/// <summary>
/// Text record field.
/// </summary>
public sealed class BinaryField : Field {

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="type">Type.</param>
    /// <param name="data">Data.</param>
    internal BinaryField(FieldType type, ProtectedBytes data)
       : base(type, data) {
    }

}
