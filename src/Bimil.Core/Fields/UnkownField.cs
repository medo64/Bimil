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
    internal UnknownField(FieldType type, ProtectedBytes data)
       : base(type, data) {
    }


    /// <summary>
    /// Returns clone of the field.
    /// </summary>
    public override Field Clone() {
        return new UnknownField(Type, Data.Clone());
    }

}
