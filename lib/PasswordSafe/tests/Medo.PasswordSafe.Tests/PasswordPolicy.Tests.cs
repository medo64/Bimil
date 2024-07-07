using Xunit;
using PwSafe = Medo.Security.Cryptography.PasswordSafe;

namespace Tests;

public class PasswordPolicy_Tests {

    [Fact(DisplayName = "PasswordSafe: PasswordPolicy: New")]
    public void PasswordPolicy_New() {
        var policy = new PwSafe.PasswordPolicy(10) {
            Style = PwSafe.PasswordPolicyStyle.MakePronounceable,
            MinimumLowercaseCount = 1,
            MinimumUppercaseCount = 2,
            MinimumDigitCount = 3,
            MinimumSymbolCount = 4
        };

        Assert.Equal(0x0200, (int)policy.Style);
        Assert.Equal(10, policy.TotalPasswordLength);
        Assert.Equal(1, policy.MinimumLowercaseCount);
        Assert.Equal(2, policy.MinimumUppercaseCount);
        Assert.Equal(3, policy.MinimumDigitCount);
        Assert.Equal(4, policy.MinimumSymbolCount);
        Assert.Equal("", new string(policy.GetSpecialSymbolSet()));
    }


    [Fact(DisplayName = "PasswordSafe: PasswordPolicy: Single special symbols")]
    public void PasswordPolicy_SingleSymbol() {
        var policy = new PwSafe.PasswordPolicy(10);
        policy.SetSpecialSymbolSet(new char[] { '!' });
        Assert.Equal("!", new string(policy.GetSpecialSymbolSet()));
    }

    [Fact(DisplayName = "PasswordSafe: PasswordPolicy: Filter duplicate symbols")]
    public void PasswordPolicy_DuplicateSymbols() {
        var policy = new PwSafe.PasswordPolicy(10);
        policy.SetSpecialSymbolSet(new char[] { 'A', 'B', 'B', 'A', 'a', 'b', 'b', 'a' });
        Assert.Equal("ABab", new string(policy.GetSpecialSymbolSet()));
    }

    [Fact(DisplayName = "PasswordSafe: PasswordPolicy: Empty special symbols")]
    public void PasswordPolicy_EmptySymbols() {
        var policy = new PwSafe.PasswordPolicy(10);
        policy.SetSpecialSymbolSet(new char[] { '!' });
        policy.SetSpecialSymbolSet();
        Assert.Equal("", new string(policy.GetSpecialSymbolSet()));
    }

}
