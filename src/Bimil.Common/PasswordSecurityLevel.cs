namespace Bimil;

/// <summary>
/// Suggested password security level.
/// </summary>
public enum PasswordSecurityLevel {

    /// <summary>
    /// Low security level.
    /// Takes less than a day to crack.
    /// </summary>
    Low = 0,

    /// <summary>
    /// Medium security level.
    /// Takes more than a day to crack.
    /// </summary>
    Medium = 1,

    /// <summary>
    /// High security level.
    /// Takes more than a year to crack.
    /// </summary>
    High = 2,

}
