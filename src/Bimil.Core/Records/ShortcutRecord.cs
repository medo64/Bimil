namespace Bimil;

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

/// <summary>
/// Entry record.
/// </summary>
public sealed class ShortcutRecord : Record {

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="fields">Fields.</param>
    internal ShortcutRecord(ICollection<Field> fields)
        : base(new FieldCollection(fields, FieldType.ShortcutUuid)) {
    }


    /// <summary>
    /// Gets/sets UUID.
    /// </summary>
    public Guid Uuid {
        get {
            if (Fields[FieldType.ShortcutUuid] is UuidField uuidField) {
                return uuidField.Uuid;
            }
            return Guid.Empty;
        }
        set {
            var @field = Fields[FieldType.ShortcutUuid];
            if (@field is not UuidField uuidField || @field.Type is not FieldType.ShortcutUuid) { throw new InvalidOperationException("Missing ShortcutUuid field"); }
            uuidField.Uuid = value;
        }
    }

}
