namespace Bimil;

using System;
using System.Collections.Generic;

/// <summary>
/// Record.
/// </summary>
public abstract class Record {

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="fields">Fields.</param>
    private protected Record(FieldCollection fields) {
        ArgumentNullException.ThrowIfNull(fields);
        Fields = fields;
    }


    /// <summary>
    /// Returns record based on type.
    /// </summary>    
    public static Record Create(ICollection<Field> fields) {
        foreach (var field in fields) {
            if (field.Type == FieldType.Uuid) {
                return new EntryRecord(fields);
            } else if (field.Type == FieldType.AliasUuid) {
                return new AliasRecord(fields);
            } else if (field.Type == FieldType.ShortcutUuid) {
                return new AliasRecord(fields);
            } else if (field.Type == FieldType.Attachment4Uuid) {
                return new AttachmentRecord(fields);
            }
        }
        return new UnknownRecord(fields);
    }


    /// <summary>
    /// Gets field collection.
    /// </summary>
    public FieldCollection Fields { get; }

}
