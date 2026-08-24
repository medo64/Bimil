namespace Bimil;

using System;

/// <summary>
/// Unknown header field.
/// </summary>
public sealed class UnknownHeader : Header {

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="type">Type.</param>
    /// <param name="data">Data.</param>
    /// <param name="caption">Caption.</param>
    internal UnknownHeader(HeaderType type, ProtectedBytes data, string caption)
       : base(type, data, caption) {
    }

}
