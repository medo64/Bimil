namespace Bimil.Core;

using System;

/// <summary>
/// Base class for password generation.
/// </summary>
public abstract class PasswordGenerator {


    /// <summary>
    /// Gets number of combinations based on the current settings.
    /// Assumption is made that the potential attacker knows exactly which
    /// method and dictionary were used to generate password (i.e. the worst
    /// case scenario).
    /// </summary>
    public virtual double Combinations { get; }


    /// <summary>
    /// Returns total number of combinations for the given character count and password length.
    /// </summary>
    /// <param name="entries">Character count and password length tuple.</param>
    public static double GetNumberOfCombinations(params (int characterCount, int passwordLength)[] entries) {
        var total = 1.0;
        foreach (var (characterCount, passwordLength) in entries) {
            total *= Math.Pow(characterCount, passwordLength);
        }
        return total;
    }

    /// <summary>
    /// Returns duration for cracking the given password.
    /// </summary>
    /// <param name="combinations">Number of combinations for a password.</param>
    public static TimeSpan GetCrackDuration(double combinations) {
        CalculateCombinationStats(combinations, out var crackDuration, out _, out _, DateTimeOffset.UtcNow);
        return crackDuration;
    }

    /// <summary>
    /// Returns the suggested password security level for the given password.
    /// </summary>
    /// <param name="combinations">Number of combinations for a password.</param>
    public static PasswordSecurityLevel GetSecurityLeve(double combinations) {
        CalculateCombinationStats(combinations, out _, out var securityLevel, out _, DateTimeOffset.UtcNow);
        return securityLevel;
    }


    /// <summary>
    /// Returns how many bits of entropy this number of combinations translates to.
    /// </summary>
    /// <param name="combinations">Number of combinations for a password.</param>
    public static int GetEntropyInBits(double combinations) {
        CalculateCombinationStats(combinations, out _, out _, out var entropyBits, DateTimeOffset.UtcNow);
        return entropyBits;
    }


    #region Helpers

    private static readonly DateTimeOffset CracksPerSecondEpoch = new(2016, 1, 1, 0, 0, 0, TimeSpan.Zero);
    private const double CracksPerSecond = 100_000_000_000_000;  //1000 trillion is as good guess as any

    internal static void CalculateCombinationStats(double combinations, out TimeSpan crackDuration, out PasswordSecurityLevel securityLevel, out int entropyBits, DateTimeOffset testTime) {  // internal so that it can be tested
        crackDuration = GetCrackDuration(combinations, testTime);
        securityLevel = crackDuration.TotalDays switch {
            > 365 => PasswordSecurityLevel.High,
            > 1 => PasswordSecurityLevel.Medium,
            _ => PasswordSecurityLevel.Low,
        };
        entropyBits = (int)Math.Floor(Math.Log(combinations, 2));
    }

    internal static double GetCracksPerSecond(DateTimeOffset testTime) {  // internal so that it can be tested
        if (testTime < CracksPerSecondEpoch) { return CracksPerSecond; }
        return CracksPerSecond * Math.Pow(2, ((testTime - CracksPerSecondEpoch).TotalDays / 365) / 2);  // double every 2 years (Moore's law)
    }

    private static TimeSpan GetCrackDuration(double combinations, DateTimeOffset testTime) {
        var cps = GetCracksPerSecond(testTime);
        var seconds = combinations / cps;
        return TimeSpan.FromSeconds(seconds);
    }

    #endregion
}
