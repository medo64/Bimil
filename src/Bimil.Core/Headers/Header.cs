namespace Bimil;

using System;

/// <summary>
/// Header field.
/// </summary>
public abstract class Header {

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="type">Type.</param>
    /// <param name="data">Data.</param>
    /// <param name="caption">Caption.</param>
    private protected Header(HeaderType type, ProtectedBytes data, string caption) {
        ArgumentNullException.ThrowIfNull(data);
        ArgumentNullException.ThrowIfNull(caption);

        Type = type;
        Data = data;
        Caption = caption;
    }


    /// <summary>
    /// Gets header type.
    /// </summary>
    public HeaderType Type { get; }

    /// <summary>
    /// Gets data.
    /// </summary>
    public ProtectedBytes Data { get; }

    /// <summary>
    /// Gets caption.
    /// Caption will be static and in English.
    /// </summary>
    public string Caption { get; }


    /// <summary>
    /// Returns header field based on type.
    /// </summary>
    /// <param name="type">Type.</param>
    public static Header Create(HeaderType type) {
        return Create(type, new ProtectedBytes());
    }

    /// <summary>
    /// Returns header field based on type.
    /// </summary>
    /// <param name="type">Type.</param>
    /// <param name="bytes">Bytes.</param>
    public static Header Create(HeaderType type, byte[] bytes) {
        return Create(type, new ProtectedBytes(bytes));
    }

    /// <summary>
    /// Returns header field based on type.
    /// </summary>
    /// <param name="type">Type.</param>
    /// <param name="bytes">Bytes.</param>
    /// <param name="zeroBytes">If true, input bytes will be zeroed after protection.</param>
    public static Header Create(HeaderType type, byte[] bytes, bool zeroBytes) {
        return Create(type, new ProtectedBytes(bytes, zeroBytes));
    }


    /// <summary>
    /// Returns header field based on type.
    /// </summary>
    /// <param name="type">Type.</param>
    /// <param name="data">Data.</param>
    internal static Header Create(HeaderType type, ProtectedBytes data) {
        return type switch {
            HeaderType.Version => new VersionHeader(type, data, "Version"),
            HeaderType.Uuid => new UuidHeader(type, data, "UUID"),
            HeaderType.NonDefaultPreferences => new TextHeader(type, data, "Non-default preferences"),
            HeaderType.TreeDisplayStatus => new TextHeader(type, data, "Tree Display Status"),
            HeaderType.TimestampOfLastSave => new TimestampHeader(type, data, "Timestamp of last save"),
#pragma warning disable CS0612 // Type or member is obsolete
            HeaderType.WhoPerformedLastSave => new TextHeader(type, data, "Who performed last save"),
#pragma warning restore CS0612 // Type or member is obsolete
            HeaderType.WhatPerformedLastSave => new TextHeader(type, data, "What performed last save"),
            HeaderType.LastSavedByUser => new TextHeader(type, data, "Last saved by user"),
            HeaderType.LastSavedOnHost => new TextHeader(type, data, "Last saved on host"),
            HeaderType.DatabaseName => new TextHeader(type, data, "Database name"),
            HeaderType.DatabaseDescription => new TextHeader(type, data, "Database description"),
            HeaderType.DatabaseFilters => new TextHeader(type, data, "Database filters"),
            HeaderType.RecentlyUsedEntries => new TextHeader(type, data, "Recently used entries"),
            HeaderType.NamedPasswordPolicies => new TextHeader(type, data, "Named password policies"),
            HeaderType.EmptyGroups => new TextHeader(type, data, "Empty groups"),
            HeaderType.Yubico => new UnknownHeader(type, data, "Yubico"),
            HeaderType.TimestampOfLastMasterPasswordChange => new TimestampHeader(type, data, "Timestamp of last master password change"),
            HeaderType.EndOfEntry => throw new NotSupportedException("Cannot create EndOfEntry field."),
            _ => new UnknownHeader(type, data, string.Empty),
        };
    }

}
