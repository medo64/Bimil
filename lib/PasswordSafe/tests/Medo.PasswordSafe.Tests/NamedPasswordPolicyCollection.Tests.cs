using Medo.Security.Cryptography.PasswordSafe;
using System;
using System.Collections.Generic;
using Xunit;
using PwSafe = Medo.Security.Cryptography.PasswordSafe;

namespace Tests;

public class NamedPasswordPolicyCollection_Tests {

    [Fact(DisplayName = "PasswordSafe: NamedPasswordPolicyCollection: New")]
    public void NamedPasswordPolicyCollection_New_Empty() {
        var doc = new PwSafe.Document("Password");
        doc.Headers.Add(new PwSafe.Header(PwSafe.HeaderType.NamedPasswordPolicies) { Text = "" });

        PwSafe.NamedPasswordPolicyCollection passwordPolicies = new PwSafe.NamedPasswordPolicyCollection(doc);
        Assert.Equal(0, passwordPolicies.Count);
    }

    [Fact(DisplayName = "PasswordSafe: NamedPasswordPolicyCollection: New a non-empty collection")]
    public void NamedPasswordPolicyCollection_New_NonEmpty() {
        var doc = new PwSafe.Document("Password");
        doc.Headers.Add(new PwSafe.Header(PwSafe.HeaderType.NamedPasswordPolicies) { Text = "0104Test111101200100200300404@#$%" });

        PwSafe.NamedPasswordPolicyCollection passwordPolicies = new PwSafe.NamedPasswordPolicyCollection(doc);
        Assert.Equal(1, passwordPolicies.Count);

        PwSafe.NamedPasswordPolicy policy = passwordPolicies[0];
        Assert.Equal("Test", policy.Name);
        Assert.Equal(0x1111, (int)policy.Style);
        Assert.Equal(18, policy.TotalPasswordLength);
        Assert.Equal(1, policy.MinimumLowercaseCount);
        Assert.Equal(2, policy.MinimumUppercaseCount);
        Assert.Equal(3, policy.MinimumDigitCount);
        Assert.Equal(4, policy.MinimumSymbolCount);
        Assert.Equal(4, policy.GetSpecialSymbolSet().Length);
    }

    [Fact(DisplayName = "PasswordSafe: NamedPasswordPolicyCollection: Clear")]
    public void NamedPasswordPolicyCollection_Clear() {
        var doc = new PwSafe.Document("Password");
        doc.Headers.Add(new PwSafe.Header(PwSafe.HeaderType.NamedPasswordPolicies) { Text = "0104Test111101200100200300404@#$%" });

        PwSafe.NamedPasswordPolicyCollection passwordPolicies = new PwSafe.NamedPasswordPolicyCollection(doc);
        Assert.Equal(1, passwordPolicies.Count);

        passwordPolicies.Clear();

        Assert.Equal(0, passwordPolicies.Count);
    }

    [Fact(DisplayName = "PasswordSafe: NamedPasswordPolicyCollection: Add")]
    public void NamedPasswordPolicyCollection_Add() {
        var doc = new PwSafe.Document("Password");
        doc.Headers.Add(new PwSafe.Header(PwSafe.HeaderType.NamedPasswordPolicies) { Text = "" });

        PwSafe.NamedPasswordPolicyCollection passwordPolicies = new PwSafe.NamedPasswordPolicyCollection(doc);
        Assert.Equal(0, passwordPolicies.Count);

        NamedPasswordPolicy policy = new PwSafe.NamedPasswordPolicy("Test", 10) {
            Style = (PwSafe.PasswordPolicyStyle)0x111,
            MinimumLowercaseCount = 1,
            MinimumUppercaseCount = 1,
            MinimumDigitCount = 1,
            MinimumSymbolCount = 1,

        };
        policy.SetSpecialSymbolSet(['@']);
        passwordPolicies.Add(policy);
        Assert.Equal(1, passwordPolicies.Count);
        Assert.Equal("0104Test011100A00100100100101@", doc.Headers[PwSafe.HeaderType.NamedPasswordPolicies].Text);
    }

    [Fact(DisplayName = "PasswordSafe: NamedPasswordPolicyCollection: Add (duplicate)")]

    public void NamedPasswordPolicyCollection_Add_Duplicate() {
        var doc = new PwSafe.Document("Password");
        doc.Headers.Add(new PwSafe.Header(PwSafe.HeaderType.NamedPasswordPolicies) { Text = "0104Test111101200100200300404@#$%" });

        PwSafe.NamedPasswordPolicyCollection passwordPolicies = new PwSafe.NamedPasswordPolicyCollection(doc);
        Assert.Equal(1, passwordPolicies.Count);

        Exception ex = Assert.Throws<ArgumentException>(() => {
            NamedPasswordPolicy policy = new PwSafe.NamedPasswordPolicy("Test", 10);
            passwordPolicies.Add(policy);
        });

        Assert.Equal("Password policy with the name 'Test' already existed in collection. (Parameter 'policy')", ex.Message);
    }

    [Fact(DisplayName = "PasswordSafe: NamedPasswordPolicyCollection: AddRange")]
    public void NamedPasswordPolicyCollection_AddRange() {
        var doc = new PwSafe.Document("Password");
        doc.Headers.Add(new PwSafe.Header(PwSafe.HeaderType.NamedPasswordPolicies) { Text = "" });

        PwSafe.NamedPasswordPolicyCollection passwordPolicies = new PwSafe.NamedPasswordPolicyCollection(doc);
        Assert.Equal(0, passwordPolicies.Count);

        NamedPasswordPolicy policy = new PwSafe.NamedPasswordPolicy("Test", 10) {
            Style = (PwSafe.PasswordPolicyStyle)0x111,
            MinimumLowercaseCount = 1,
            MinimumUppercaseCount = 1,
            MinimumDigitCount = 1,
            MinimumSymbolCount = 1,

        };
        policy.SetSpecialSymbolSet(['@']);
        passwordPolicies.AddRange(new List<NamedPasswordPolicy>() { policy });
        Assert.Equal(1, passwordPolicies.Count);
        Assert.Equal("0104Test011100A00100100100101@", doc.Headers[PwSafe.HeaderType.NamedPasswordPolicies].Text);
    }

    [Fact(DisplayName = "PasswordSafe: NamedPasswordPolicyCollection: AddRange (duplicate)")]
    public void NamedPasswordPolicyCollection_AddRange_Duplicate() {
        var doc = new PwSafe.Document("Password");
        doc.Headers.Add(new PwSafe.Header(PwSafe.HeaderType.NamedPasswordPolicies) { Text = "0104Test111101200100200300404@#$%" });

        PwSafe.NamedPasswordPolicyCollection passwordPolicies = new PwSafe.NamedPasswordPolicyCollection(doc);
        Assert.Equal(1, passwordPolicies.Count);

        Exception ex = Assert.Throws<ArgumentException>(() => {
            NamedPasswordPolicy policy = new PwSafe.NamedPasswordPolicy("Test", 10);
            passwordPolicies.AddRange(new List<NamedPasswordPolicy>() { policy });
        });

        Assert.Equal("Password policy with the name 'Test' already existed in collection. (Parameter 'policies')", ex.Message);
    }

    [Fact(DisplayName = "PasswordSafe: NamedPasswordPolicyCollection: Remove")]
    public void NamedPasswordPolicyCollection_Remove() {
        var doc = new PwSafe.Document("Password");
        doc.Headers.Add(new PwSafe.Header(PwSafe.HeaderType.NamedPasswordPolicies) { Text = "0104Test111101200100200300404@#$%" });

        PwSafe.NamedPasswordPolicyCollection passwordPolicies = new PwSafe.NamedPasswordPolicyCollection(doc);
        Assert.Equal(1, passwordPolicies.Count);

        passwordPolicies.Remove(passwordPolicies[0]);
        Assert.Equal(0, passwordPolicies.Count);
    }
}

