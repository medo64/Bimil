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
    /// <param name="caption">Caption.</param>
    /// <param name="isSensitive">If true, field is considered sensitive.</param>
    internal BinaryField(FieldType type, ProtectedBytes data, string caption, bool isSensitive)
       : base(type, data, caption, isSensitive) {
    }

}
