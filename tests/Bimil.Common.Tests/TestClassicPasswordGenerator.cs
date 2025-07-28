namespace Tests;

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bimil;

[TestClass]
public class TestClassicPasswordGenerator {

    [TestMethod]
    public void Generate8() {
        var gen = new ClassicPasswordGenerator() {
            PasswordLength = 8,
        };
        var dict = new Dictionary<string, object>();
        for (var i = 0; i < 10000; i++) {
            var password = gen.GetNewPassword();
            Assert.AreEqual(8, password.Length);
            Assert.IsFalse(dict.ContainsKey(password));
            dict.Add(password, null);
        }
    }

    [TestMethod]
    public void Generate14() {
        var gen = new ClassicPasswordGenerator() {
            PasswordLength = 14,
        };
        var dict = new Dictionary<string, object>();
        for (var i = 0; i < 10000; i++) {
            var password = gen.GetNewPassword();
            Assert.AreEqual(14, password.Length);
            Assert.IsFalse(dict.ContainsKey(password));
            dict.Add(password, null);
        }
    }

    [TestMethod]
    public void Generate14None() {
        var gen = new ClassicPasswordGenerator() {
            PasswordLength = 14,
            IncludeLowercaseLetters = false,
            IncludeUppercaseLetters = false,
            IncludeDigits = false,
            IncludeSpecialCharacters = false,
            ExcludeSimilarCharacters = false,
            ExcludeMovableCharacters = false,
            ExcludeUnpronounceable = false,
            ExcludeRepeatedCharacters = false,
        };
        Assert.AreEqual(14, gen.PasswordLength);
        Assert.AreEqual(0, gen.GetEstimatedCombinationCount());
        Assert.AreEqual(0, gen.GetEstimatedEntropyInBits());
        Assert.ThrowsException<InvalidOperationException>(() => {
            _ = gen.GetNewPassword();
        });

    }

    [TestMethod]
    public void Generate14IncludeLowercase() {
        var gen = new ClassicPasswordGenerator() {
            PasswordLength = 14,
            IncludeLowercaseLetters = true,
            IncludeUppercaseLetters = false,
            IncludeDigits = false,
            IncludeSpecialCharacters = false,
            ExcludeSimilarCharacters = false,
            ExcludeMovableCharacters = false,
            ExcludeUnpronounceable = false,
            ExcludeRepeatedCharacters = false,
        };
        Assert.AreEqual(14, gen.PasswordLength);
        Assert.AreEqual(6.450997470329715E+19, gen.GetEstimatedCombinationCount());
        Assert.AreEqual(65, gen.GetEstimatedEntropyInBits());
    }

    [TestMethod]
    public void Generate14Letters() {
        var gen = new ClassicPasswordGenerator() {
            PasswordLength = 14,
            IncludeLowercaseLetters = true,
            IncludeUppercaseLetters = true,
            IncludeDigits = false,
            IncludeSpecialCharacters = false,
            ExcludeSimilarCharacters = false,
            ExcludeMovableCharacters = false,
            ExcludeUnpronounceable = false,
            ExcludeRepeatedCharacters = false,
        };
        Assert.AreEqual(14, gen.PasswordLength);
        Assert.AreEqual(1.0569314255388205E+24, gen.GetEstimatedCombinationCount());
        Assert.AreEqual(79, gen.GetEstimatedEntropyInBits());
    }

    [TestMethod]
    public void Generate14LettersAndDigits() {
        var gen = new ClassicPasswordGenerator() {
            PasswordLength = 14,
            IncludeLowercaseLetters = true,
            IncludeUppercaseLetters = true,
            IncludeDigits = true,
            IncludeSpecialCharacters = false,
            ExcludeSimilarCharacters = false,
            ExcludeMovableCharacters = false,
            ExcludeUnpronounceable = false,
            ExcludeRepeatedCharacters = false,
        };
        Assert.AreEqual(14, gen.PasswordLength);
        Assert.AreEqual(1.2401769434657528E+25, gen.GetEstimatedCombinationCount());
        Assert.AreEqual(83, gen.GetEstimatedEntropyInBits());
    }

    [TestMethod]
    public void Generate14All() {
        var gen = new ClassicPasswordGenerator() {
            PasswordLength = 14,
            IncludeLowercaseLetters = true,
            IncludeUppercaseLetters = true,
            IncludeDigits = true,
            IncludeSpecialCharacters = true,
            ExcludeSimilarCharacters = false,
            ExcludeMovableCharacters = false,
            ExcludeUnpronounceable = false,
            ExcludeRepeatedCharacters = false,
        };
        Assert.AreEqual(14, gen.PasswordLength);
        Assert.AreEqual(2.670419511272061E+27, gen.GetEstimatedCombinationCount());
        Assert.AreEqual(91, gen.GetEstimatedEntropyInBits());
    }

    [TestMethod]
    public void Generate14AllButSimilar() {
        var gen = new ClassicPasswordGenerator() {
            PasswordLength = 14,
            IncludeLowercaseLetters = true,
            IncludeUppercaseLetters = true,
            IncludeDigits = true,
            IncludeSpecialCharacters = true,
            ExcludeSimilarCharacters = true,
            ExcludeMovableCharacters = false,
            ExcludeUnpronounceable = false,
            ExcludeRepeatedCharacters = false,
        };
        Assert.AreEqual(14, gen.PasswordLength);
        Assert.AreEqual(4.398046511104E+26, gen.GetEstimatedCombinationCount());
        Assert.AreEqual(88, gen.GetEstimatedEntropyInBits());
    }

    [TestMethod]
    public void Generate14AllButSimilarAndMovable() {
        var gen = new ClassicPasswordGenerator() {
            PasswordLength = 14,
            IncludeLowercaseLetters = true,
            IncludeUppercaseLetters = true,
            IncludeDigits = true,
            IncludeSpecialCharacters = true,
            ExcludeSimilarCharacters = true,
            ExcludeMovableCharacters = true,
            ExcludeUnpronounceable = false,
            ExcludeRepeatedCharacters = false,
        };
        Assert.AreEqual(14, gen.PasswordLength);
        Assert.AreEqual(1.0061319724179154E+26, gen.GetEstimatedCombinationCount());
        Assert.AreEqual(86, gen.GetEstimatedEntropyInBits());
    }

    [TestMethod]
    public void Generate14AllButSimilarMovableAndUnpronounceable() {
        var gen = new ClassicPasswordGenerator() {
            PasswordLength = 14,
            IncludeLowercaseLetters = true,
            IncludeUppercaseLetters = true,
            IncludeDigits = true,
            IncludeSpecialCharacters = true,
            ExcludeSimilarCharacters = true,
            ExcludeMovableCharacters = true,
            ExcludeUnpronounceable = true,
            ExcludeRepeatedCharacters = false,
        };
        Assert.AreEqual(14, gen.PasswordLength);
        Assert.AreEqual(5.3851443515311585E+23, gen.GetEstimatedCombinationCount());
        Assert.AreEqual(78, gen.GetEstimatedEntropyInBits());
    }

    [TestMethod]
    public void Generate14AllIncludeAllExclude() {
        var gen = new ClassicPasswordGenerator() {
            PasswordLength = 14,
            IncludeLowercaseLetters = true,
            IncludeUppercaseLetters = true,
            IncludeDigits = true,
            IncludeSpecialCharacters = true,
            ExcludeSimilarCharacters = true,
            ExcludeMovableCharacters = true,
            ExcludeUnpronounceable = true,
            ExcludeRepeatedCharacters = true,
        };
        Assert.AreEqual(14, gen.PasswordLength);
        Assert.AreEqual(4.0227181702895694E+23, gen.GetEstimatedCombinationCount());
        Assert.AreEqual(78, gen.GetEstimatedEntropyInBits());
    }

}
