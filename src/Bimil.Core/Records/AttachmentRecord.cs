namespace Bimil;

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

/// <summary>
/// Entry record.
/// </summary>
public sealed class AttachmentRecord : Record {

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="fields">Fields.</param>
    internal AttachmentRecord(ICollection<Field> fields)
        : base(new FieldCollection(fields, FieldType.Attachment4Uuid)) {
    }


    /// <summary>
    /// Gets/sets UUID.
    /// </summary>
    public Guid Uuid {
        get {
            if (Fields[FieldType.Attachment4Uuid] is UuidField uuidField) {
                return uuidField.Uuid;
            }
            return Guid.Empty;
        }
        set {
            var @field = Fields[FieldType.Attachment4Uuid];
            if (@field is not UuidField uuidField || @field.Type is not FieldType.Attachment4Uuid) { throw new InvalidOperationException("Missing ShortcutUuid field"); }
            uuidField.Uuid = value;
        }
    }

}
