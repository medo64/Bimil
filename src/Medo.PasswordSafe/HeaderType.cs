using System;

namespace Medo.Security.Cryptography.PasswordSafe;

/// <summary>
/// Header field types.
/// </summary>
public enum HeaderType {
    /// <summary>
    /// Version.
    /// </summary>
    Version = 0x00,
    /// <summary>
    /// UUID.
    /// </summary>
    Uuid = 0x01,
    /// <summary>
    /// Non-default preferences.
    /// </summary>
    NonDefaultPreferences = 0x02,
    /// <summary>
    /// Tree display status.
    /// </summary>
    TreeDisplayStatus = 0x03,
    /// <summary>
    /// Timestamp of last save.
    /// </summary>
    TimestampOfLastSave = 0x04,
    /// <summary>
    /// Who performed last save.
    /// </summary>
    WhoPerformedLastSave = 0x05,
    /// <summary>
    /// What performed last save.
    /// </summary>
    WhatPerformedLastSave = 0x06,
    /// <summary>
    /// Last saved by user.
    /// </summary>
    LastSavedByUser = 0x07,
    /// <summary>
    /// Last saved on host.
    /// </summary>
    LastSavedOnHost = 0x08,
    /// <summary>
    /// Database name.
    /// </summary>
    DatabaseName = 0x09,
    /// <summary>
    /// Database description.
    /// </summary>
    DatabaseDescription = 0x0A,
    /// <summary>
    /// Database filters.
    /// </summary>
    DatabaseFilters = 0x0B,
    /// <summary>
    /// Recently used entries.
    /// </summary>
    RecentlyUsedEntries = 0x0F,
    /// <summary>
    /// Named password policies.
    /// </summary>
    NamedPasswordPolicies = 0x10,
    /// <summary>
    /// Empty groups.
    /// </summary>
    EmptyGroups = 0x11,
    /// <summary>
    /// Yubico.
    /// </summary>
    Yubico = 0x12,
    /// <summary>
    /// End of entries.
    /// </summary>
    EndOfEntry = 0xFF,
}
