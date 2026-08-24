namespace Bimil;

using System;
using System.Collections.Generic;

/// <summary>
/// Entry record.
/// </summary>
public sealed class UnknownRecord : Record {

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="fields">Fields.</param>
    internal UnknownRecord(ICollection<Field> fields)
        : base(fields) {
    }

}
