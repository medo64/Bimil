namespace Bimil;

using System;

/// <summary>
/// Interface for readable sensitivity.
/// </summary>
public interface IReadableIsSensitive {

    /// <summary>
    /// Gets if field is sensitive.
    /// </summary>
    public bool IsSensitive { get; }

}
