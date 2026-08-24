namespace Bimil;

using System;

/// <summary>
/// Unknown recotd field.
/// </summary>
public sealed class UnknownField : Field {

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="type">Type.</param>
    /// <param name="data">Data.</param>
    /// <param name="caption">Caption.</param>
    internal UnknownField(FieldType type, ProtectedBytes data, string caption)
       : base(type, data, caption) {
    }

}
