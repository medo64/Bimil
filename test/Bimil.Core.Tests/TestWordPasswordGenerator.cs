namespace Tests;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bimil.Core;

[TestClass]
public class TestWordPasswordGenerator {

    [TestMethod]
    public void Generate4() {
        var gen = new WordPasswordGenerator() {
            WordCount = 4,
        };
        var dict = new Dictionary<string, object?>();
        for (var i = 0; i < 10000; i++) {
            var password = gen.GetNewPassword();
            Assert.IsFalse(dict.ContainsKey(password));
            dict.Add(password, null);
        }
    }

    [TestMethod]
    public void Generate5() {
        var gen = new WordPasswordGenerator() {
            WordCount = 5,
        };
        var dict = new Dictionary<string, object?>();
        for (var i = 0; i < 10000; i++) {
            var password = gen.GetNewPassword();
            Assert.IsFalse(dict.ContainsKey(password));
            dict.Add(password, null);
        }
    }

    [TestMethod]
    public void Generate5None() {
        var gen = new WordPasswordGenerator() {
            WordCount = 5,
            IncludeNumber = false,
            IncludeSpecialCharacters = false,
            IncludeSwappedCase = false,
            DropOneCharacter = false,
            UseTitlecase = false,
            RestrictWordBreakup = false,
            RestrictAdjustmentsToSuffix = false,
            UseSpacesSeparator = false,
        };
        Assert.AreEqual(5, gen.WordCount);
        Assert.AreEqual(1.6273676242087346E+19, gen.GetEstimatedCombinationCount());
        Assert.AreEqual(63, gen.GetEstimatedEntropyInBits());
    }

    [TestMethod]
    public void Generate5WithNumber() {
        var gen = new WordPasswordGenerator() {
            WordCount = 5,
            IncludeNumber = true,
            IncludeSpecialCharacters = false,
            IncludeSwappedCase = false,
            DropOneCharacter = false,
            UseTitlecase = false,
            RestrictWordBreakup = false,
            RestrictAdjustmentsToSuffix = false,
            UseSpacesSeparator = false,
        };
        Assert.AreEqual(5, gen.WordCount);
        Assert.AreEqual(3.2547352484174694E+22, gen.GetEstimatedCombinationCount());
        Assert.AreEqual(74, gen.GetEstimatedEntropyInBits());
    }

    [TestMethod]
    public void Generate5WithNumberAndSpecial() {
        var gen = new WordPasswordGenerator() {
            WordCount = 5,
            IncludeNumber = true,
            IncludeSpecialCharacters = true,
            IncludeSwappedCase = false,
            DropOneCharacter = false,
            UseTitlecase = false,
            RestrictWordBreakup = false,
            RestrictAdjustmentsToSuffix = false,
            UseSpacesSeparator = false,
        };
        Assert.AreEqual(5, gen.WordCount);
        Assert.AreEqual(1.8877464440821322E+25, gen.GetEstimatedCombinationCount());
        Assert.AreEqual(83, gen.GetEstimatedEntropyInBits());
    }

    [TestMethod]
    public void Generate5WithNumberSpecialAndSwappedCase() {
        var gen = new WordPasswordGenerator() {
            WordCount = 5,
            IncludeNumber = true,
            IncludeSpecialCharacters = true,
            IncludeSwappedCase = true,
            DropOneCharacter = false,
            UseTitlecase = false,
            RestrictWordBreakup = false,
            RestrictAdjustmentsToSuffix = false,
            UseSpacesSeparator = false,
        };
        Assert.AreEqual(5, gen.WordCount);
        Assert.AreEqual(5.899207637756662E+28, gen.GetEstimatedCombinationCount());
        Assert.AreEqual(95, gen.GetEstimatedEntropyInBits());
    }

    [TestMethod]
    public void Generate5WithNumberSpecialSwappedCaseAndIncomplete() {
        var gen = new WordPasswordGenerator() {
            WordCount = 5,
            IncludeNumber = true,
            IncludeSpecialCharacters = true,
            IncludeSwappedCase = true,
            DropOneCharacter = true,
            UseTitlecase = false,
            RestrictWordBreakup = false,
            RestrictAdjustmentsToSuffix = false,
            UseSpacesSeparator = false,
        };
        Assert.AreEqual(5, gen.WordCount);
        Assert.AreEqual(1.8435023867989573E+32, gen.GetEstimatedCombinationCount());
        Assert.AreEqual(107, gen.GetEstimatedEntropyInBits());
    }

    [TestMethod]
    public void Generate5WithAllButTitle() {
        var gen = new WordPasswordGenerator() {
            WordCount = 5,
            IncludeNumber = true,
            IncludeSpecialCharacters = true,
            IncludeSwappedCase = true,
            DropOneCharacter = true,
            UseTitlecase = true,
            RestrictWordBreakup = false,
            RestrictAdjustmentsToSuffix = false,
            UseSpacesSeparator = false,
        };
        Assert.AreEqual(5, gen.WordCount);
        Assert.AreEqual(1.8435023867989573E+32, gen.GetEstimatedCombinationCount());  // Title case alone doesn't change combination count
        Assert.AreEqual(107, gen.GetEstimatedEntropyInBits());
    }

    [TestMethod]
    public void Generate5WithAllButTitleAndBreakup() {
        var gen = new WordPasswordGenerator() {
            WordCount = 5,
            IncludeNumber = true,
            IncludeSpecialCharacters = true,
            IncludeSwappedCase = true,
            DropOneCharacter = true,
            UseTitlecase = true,
            RestrictWordBreakup = true,
            RestrictAdjustmentsToSuffix = false,
            UseSpacesSeparator = false,
        };
        Assert.AreEqual(5, gen.WordCount);
        Assert.AreEqual(1.9330523587401032E+26, gen.GetEstimatedCombinationCount());
        Assert.AreEqual(87, gen.GetEstimatedEntropyInBits());
    }

    [TestMethod]
    public void Generate5WithAllButTitleBreakupAndSuffixRestriction() {
        var gen = new WordPasswordGenerator() {
            WordCount = 5,
            IncludeNumber = true,
            IncludeSpecialCharacters = true,
            IncludeSwappedCase = true,
            DropOneCharacter = true,
            UseTitlecase = true,
            RestrictWordBreakup = true,
            RestrictAdjustmentsToSuffix = true,
            UseSpacesSeparator = false,
        };
        Assert.AreEqual(5, gen.WordCount);
        Assert.AreEqual(9.438732220410662E+22, gen.GetEstimatedCombinationCount());
        Assert.AreEqual(76, gen.GetEstimatedEntropyInBits());
    }

    [TestMethod]
    public void Generate5WithAllButAll() {
        var gen = new WordPasswordGenerator() {
            WordCount = 5,
            IncludeNumber = true,
            IncludeSpecialCharacters = true,
            IncludeSwappedCase = true,
            DropOneCharacter = true,
            UseTitlecase = true,
            RestrictWordBreakup = true,
            RestrictAdjustmentsToSuffix = true,
            UseSpacesSeparator = true,
        };
        Assert.AreEqual(5, gen.WordCount);
        Assert.AreEqual(9.438732220410662E+22, gen.GetEstimatedCombinationCount());  // Space separator alone doesn't change combination count
        Assert.AreEqual(76, gen.GetEstimatedEntropyInBits());
    }

}
