namespace Bimil;

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

/// <summary>
/// Entry record.
/// </summary>
public sealed class EntryRecord : Record {

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="fields">Fields.</param>
    internal EntryRecord(ICollection<Field> fields)
        : base(new FieldCollection(fields, FieldType.Uuid)) {
    }


    /// <summary>
    /// Gets/sets UUID.
    /// </summary>
    public Guid Uuid {
        get {
            if (Fields[FieldType.Uuid] is UuidField uuidField) {
                return uuidField.Uuid;
            }
            return Guid.Empty;
        }
        set {
            var @field = Fields[FieldType.Uuid];
            if (@field is not UuidField uuidField || @field.Type is not FieldType.Uuid) { throw new InvalidOperationException("Missing Uuid field"); }
            uuidField.Uuid = value;
        }
    }

    /// <summary>
    /// Gets/sets title.
    /// </summary>
    public string Title {
        get {
            if (Fields[FieldType.Title] is TextField textField) {
                return textField.Text;
            }
            return string.Empty;
        }
        set {
            var @field = Fields[FieldType.Title];
            if (@field is not TextField textField) {
                textField = (TextField)Field.Create(FieldType.Title);
                Fields.Add(textField);
            }
            textField.Text = value;
        }
    }

    /// <summary>
    /// Gets/sets title.
    /// </summary>
    public string Group {
        get {
            if (Fields[FieldType.Group] is TextField textField) {
                return textField.Text;
            }
            return string.Empty;
        }
        set {
            var @field = Fields[FieldType.Group];
            if (@field is not TextField textField) {
                textField = (TextField)Field.Create(FieldType.Group);
                Fields.Add(textField);
            }
            textField.Text = value;
        }
    }

    /// <summary>
    /// Gets/sets creation time.
    /// </summary>
    public DateTime CreationTime {
        get {
            if (Fields[FieldType.CreationTime] is TimestampField timeField) {
                return timeField.Timestamp;
            }
            return DateTime.MinValue;
        }
        set {
            var @field = Fields[FieldType.CreationTime];
            if (@field is not TimestampField timeField) {
                timeField = (TimestampField)Field.Create(FieldType.CreationTime);
                Fields.Add(timeField);
            }
            timeField.Timestamp = value;
        }
    }

    /// <summary>
    /// Gets/sets modification time.
    /// </summary>
    public DateTime ModificationTime {
        get {
            if (Fields[FieldType.LastModificationTime] is TimestampField timeField) {
                return timeField.Timestamp;
            }
            return DateTime.MinValue;
        }
        set {
            var @field = Fields[FieldType.LastModificationTime];
            if (@field is not TimestampField timeField) {
                timeField = (TimestampField)Field.Create(FieldType.LastModificationTime);
                Fields.Add(timeField);
            }
            timeField.Timestamp = value;
        }
    }

    /// <summary>
    /// Gets/sets access time.
    /// </summary>
    public DateTime AccessTime {
        get {
            if (Fields[FieldType.LastAccessTime] is TimestampField timeField) {
                return timeField.Timestamp;
            }
            return DateTime.MinValue;
        }
        set {
            var @field = Fields[FieldType.LastAccessTime];
            if (@field is not TimestampField timeField) {
                timeField = (TimestampField)Field.Create(FieldType.LastAccessTime);
                Fields.Add(timeField);
            }
            timeField.Timestamp = value;
        }
    }


    /// <summary>
    /// Updates modification time for the record.
    /// If creation time is not present, it will be created.
    /// </summary>
    public void UpdateAccessTime() {
        AccessTime = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates modification time for the record.
    /// If creation time is not present, it will be created.
    /// </summary>
    public void UpdateModificationTime() {
        ModificationTime = DateTime.UtcNow;
        if (CreationTime == DateTime.MinValue) { CreationTime = ModificationTime; }
    }


    /// <summary>
    /// Returns a new entry record.
    /// </summary>
    public static EntryRecord Create() {
        var uuid = (UuidField)Field.Create(FieldType.Uuid);
        uuid.Uuid = Guid.CreateVersion7();

        var fields = new FieldCollection([uuid]);
        return new EntryRecord(fields);
    }

}
