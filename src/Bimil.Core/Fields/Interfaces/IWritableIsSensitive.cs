namespace Bimil;

using System;

/// <summary>
/// Interface for writable sensitivity.
/// </summary>
public interface IWritableIsSensitive {

    /// <summary>
    /// Gets/sets if field is sensitive.
    /// </summary>
    public bool IsSensitive { get; set; }

}
