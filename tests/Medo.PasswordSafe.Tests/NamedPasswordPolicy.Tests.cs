using Xunit;
using PwSafe = Medo.Security.Cryptography.PasswordSafe;

namespace Tests;

public class NamedPasswordPolicy_Tests {

    [Fact(DisplayName = "PasswordSafe: NamedPasswordPolicy: New")]
    public void NamedPasswordPolicy_New() {
        var policy = new PwSafe.NamedPasswordPolicy("Test", 10) {
            Style = PwSafe.PasswordPolicyStyle.MakePronounceable,
            MinimumLowercaseCount = 1,
            MinimumUppercaseCount = 2,
            MinimumDigitCount = 3,
            MinimumSymbolCount = 4
        };

        Assert.Equal("Test", policy.Name);
        Assert.Equal(0x0200, (int)policy.Style);
        Assert.Equal(10, policy.TotalPasswordLength);
        Assert.Equal(1, policy.MinimumLowercaseCount);
        Assert.Equal(2, policy.MinimumUppercaseCount);
        Assert.Equal(3, policy.MinimumDigitCount);
        Assert.Equal(4, policy.MinimumSymbolCount);
        Assert.Equal("", new string(policy.GetSpecialSymbolSet()));
        Assert.Equal("Test", policy.ToString());
    }


    [Fact(DisplayName = "PasswordSafe: NamedPasswordPolicy: Single special symbols")]
    public void NamedPasswordPolicy_SingleSymbol() {
        var policy = new PwSafe.NamedPasswordPolicy("Test", 10);
        policy.SetSpecialSymbolSet(new char[] { '!' });
        Assert.Equal("!", new string(policy.GetSpecialSymbolSet()));
    }

    [Fact(DisplayName = "PasswordSafe: NamedPasswordPolicy: Filter duplicate symbols")]
    public void NamedPasswordPolicy_DuplicateSymbols() {
        var policy = new PwSafe.NamedPasswordPolicy("Test", 10);
        policy.SetSpecialSymbolSet(new char[] { 'A', 'B', 'B', 'A', 'a', 'b', 'b', 'a' });
        Assert.Equal("ABab", new string(policy.GetSpecialSymbolSet()));
    }

    [Fact(DisplayName = "PasswordSafe: NamedPasswordPolicy: Empty special symbols")]
    public void NamedPasswordPolicy_EmptySymbols() {
        var policy = new PwSafe.NamedPasswordPolicy("Test", 10);
        policy.SetSpecialSymbolSet(new char[] { '!' });
        policy.SetSpecialSymbolSet();
        Assert.Equal("", new string(policy.GetSpecialSymbolSet()));
    }

}
