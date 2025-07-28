namespace Bimil;

using System;
using System.Security.Cryptography;

/// <summary>
/// Base class for password generation.
/// </summary>
public abstract class PasswordGenerator {

    /// <summary>
    /// Returns newly generated password.
    /// </summary>
    public abstract string GetNewPassword();

    /// <summary>
    /// REturns number of combinations based on the current settings.
    /// Assumption is made that the potential attacker knows exactly which
    /// method and dictionary were used to generate password (i.e. the worst
    /// case scenario).
    /// </summary>
    public abstract double GetEstimatedCombinationCount();


    /// <summary>
    /// Returns estimated time to crack the password assuming all the best for attacker.
    /// </summary>
    public TimeSpan GetEstimatedCrackDuration() {
        EstimateStats(GetEstimatedCombinationCount(), out var crackDuration, out _, out _, DateTimeOffset.UtcNow);
        return crackDuration;
    }

    /// <summary>
    /// Returns estimated security level for the password assuming all the best for attacker.
    /// </summary>
    /// <returns></returns>
    public PasswordSecurityLevel GetEstimatedSecurityLevel() {
        EstimateStats(GetEstimatedCombinationCount(), out _, out var securityLevel, out _, DateTimeOffset.UtcNow);
        return securityLevel;
    }

    /// <summary>
    /// Returns how many bits of entropy this number of combinations translates to.
    /// </summary>
    public int GetEstimatedEntropyInBits() {
        var combinations = GetEstimatedCombinationCount();
        if (combinations == 0) { return 0; }
        EstimateStats(combinations, out _, out _, out var entropyBits, DateTimeOffset.UtcNow);
        return entropyBits;
    }


    #region Helpers

    private static readonly DateTimeOffset CracksPerSecondEpoch = new(2016, 1, 1, 0, 0, 0, TimeSpan.Zero);
    private const double CracksPerSecond = 100_000_000_000_000;  //1000 trillion is as good guess as any

    internal static void EstimateStats(double combinations, out TimeSpan crackDuration, out PasswordSecurityLevel securityLevel, out int entropyBits, DateTimeOffset testTime) {  // internal so that it can be tested
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
        var seconds = Math.Round(combinations / cps, 0, MidpointRounding.ToZero);
        return (seconds > TimeSpan.MaxValue.TotalSeconds) ? TimeSpan.MaxValue : TimeSpan.FromSeconds(seconds);
    }

    #endregion Helpers
}
