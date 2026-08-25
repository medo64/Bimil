namespace Bimil;

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

/// <summary>
/// Entry record.
/// </summary>
public sealed class AliasRecord : Record {

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="fields">Fields.</param>
    internal AliasRecord(ICollection<Field> fields)
        : base(new FieldCollection(fields, FieldType.AliasUuid)) {
    }


    /// <summary>
    /// Gets/sets UUID.
    /// </summary>
    public Guid Uuid {
        get {
            if (Fields[FieldType.AliasUuid] is UuidField uuidField) {
                return uuidField.Uuid;
            }
            return Guid.Empty;
        }
        set {
            var @field = Fields[FieldType.AliasUuid];
            if (@field is not UuidField uuidField || @field.Type is not FieldType.AliasUuid) { throw new InvalidOperationException("Missing AliasUuid field"); }
            uuidField.Uuid = value;
        }
    }

}
