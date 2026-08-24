namespace Bimil;

using System;

public sealed partial class Document {

    /// <summary>
    /// Gets all headers.
    /// </summary>
    public HeaderCollection Headers;

    /// <summary>
    /// Gets all records.
    /// </summary>
    public RecordCollection Records;


    /// <summary>
    /// Gets/sets document version.
    /// </summary>
    public Version Version {
        get {
            if (Headers[HeaderType.Version] is VersionHeader versionField) {
                return versionField.Version;
            }
            return new Version(0, 0, 0, 0);
        }
        set {
            var headerField = Headers[HeaderType.Version];
            if (headerField is not VersionHeader versionField) {
                versionField = (VersionHeader)Header.Create(HeaderType.Version);
                Headers.Insert(0, versionField);
            }
            versionField.Version = value;
        }
    }

    /// <summary>
    /// Gets/sets document UUID.
    /// </summary>
    public Guid Uuid {
        get {
            if (Headers[HeaderType.Uuid] is UuidHeader uuidField) {
                return uuidField.Uuid;
            }
            return Guid.Empty;
        }
        set {
            var headerField = Headers[HeaderType.Uuid];
            if (headerField is not UuidHeader uuidField) {
                uuidField = (UuidHeader)Header.Create(HeaderType.Uuid);
                Headers.Insert(1, uuidField);
            }
            uuidField.Uuid = value;
        }
    }

    /// <summary>
    /// Gets/sets last save timestamp.
    /// </summary>
    public DateTime LastSaveTime {
        get {
            if (Headers[HeaderType.TimestampOfLastSave] is not null and TimestampHeader timestampField) {
                return timestampField.Timestamp;
            }
            return DateTime.MinValue;
        }
        set {
            var @field = Headers[HeaderType.TimestampOfLastSave];
            if (@field is not TimestampHeader timeField) {
                timeField = (TimestampHeader)Header.Create(HeaderType.TimestampOfLastSave);
                Headers.Add(timeField);
            }
            timeField.Timestamp = value;
        }
    }

    /// <summary>
    /// Gets/sets last password change timestamp.
    /// </summary>
    public DateTime LastPasswordChangeTime {
        get {
            if (Headers[HeaderType.TimestampOfLastMasterPasswordChange] is not null and TimestampHeader timestampField) {
                return timestampField.Timestamp;
            }
            return DateTime.MinValue;
        }
        set {
            var @field = Headers[HeaderType.TimestampOfLastMasterPasswordChange];
            if (@field is not TimestampHeader timeField) {
                timeField = (TimestampHeader)Header.Create(HeaderType.TimestampOfLastMasterPasswordChange);
                Headers.Add(timeField);
            }
            timeField.Timestamp = value;
        }
    }

    /// <summary>
    /// Gets/sets which user performed last save.
    /// </summary>
    public string LastSaveUser {
        get {
            if (Headers[HeaderType.LastSavedByUser] is not null and TextHeader textField) {
                return textField.Text;
            }
            return string.Empty;
        }
        set {
            ArgumentNullException.ThrowIfNull(value);
            var @field = Headers[HeaderType.LastSavedByUser];
            if (@field is not TextHeader textField) {
                textField = (TextHeader)Header.Create(HeaderType.LastSavedByUser);
                Headers.Add(textField);
            }
            textField.Text = value;
        }
    }

    /// <summary>
    /// Gets/sets on which host the last save was performed.
    /// </summary>
    public string LastSaveHost {
        get {
            if (Headers[HeaderType.LastSavedOnHost] is not null and TextHeader textField) {
                return textField.Text;
            }
            return string.Empty;
        }
        set {
            ArgumentNullException.ThrowIfNull(value);
            var @field = Headers[HeaderType.LastSavedOnHost];
            if (@field is not TextHeader textField) {
                textField = (TextHeader)Header.Create(HeaderType.LastSavedOnHost);
                Headers.Add(textField);
            }
            textField.Text = value;
        }
    }

    /// <summary>
    /// Gets/sets what performed last save.
    /// </summary>
    public string LastSaveApplication {
        get {
            if (Headers[HeaderType.WhatPerformedLastSave] is not null and TextHeader textField) {
                return textField.Text;
            }
            return string.Empty;
        }
        set {
            ArgumentNullException.ThrowIfNull(value);
            var @field = Headers[HeaderType.WhatPerformedLastSave];
            if (@field is not TextHeader textField) {
                textField = (TextHeader)Header.Create(HeaderType.WhatPerformedLastSave);
                Headers.Add(textField);
            }
            textField.Text = value;
        }
    }


    /// <summary>
    /// Updates last save timestamp
    /// </summary>
    public void ClearIdentityData() {
        for (var i = Headers.Count - 1; i >= 0; i--) {
            var header = Headers[i];
#pragma warning disable CS0612 // Type or member is obsolete
            if (header.Type is HeaderType.WhoPerformedLastSave or HeaderType.WhatPerformedLastSave or HeaderType.LastSavedByUser or HeaderType.LastSavedOnHost) {
                Headers.RemoveAt(i);
            }
#pragma warning restore CS0612 // Type or member is obsolete
        }
    }

}
