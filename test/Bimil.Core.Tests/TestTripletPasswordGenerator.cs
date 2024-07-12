namespace Tests;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bimil.Core;

[TestClass]
public class TestTripletPasswordGenerator {

    [TestMethod]
    public void Generate5() {
        var gen = new TripletPasswordGenerator() {
            TripletCount = 5,
        };
        var dict = new Dictionary<string, object?>();
        for (var i = 0; i < 10000; i++) {
            var password = gen.GetNewPassword();
            Assert.IsFalse(dict.ContainsKey(password));
            dict.Add(password, null);
        }
    }

    [TestMethod]
    public void Generate6() {
        var gen = new TripletPasswordGenerator() {
            TripletCount = 6,
        };
        var dict = new Dictionary<string, object?>();
        for (var i = 0; i < 10000; i++) {
            var password = gen.GetNewPassword();
            Assert.IsFalse(dict.ContainsKey(password));
            dict.Add(password, null);
        }
    }

    [TestMethod]
    public void Generate6None() {
        var gen = new TripletPasswordGenerator() {
            TripletCount = 6,
            IncludeNumber = false,
            IncludeSpecialCharacters = false,
            IncludeSwappedCase = false,
            DropOneCharacter = false,
            UseTitlecase = false,
            RestrictWordBreakup = false,
            RestrictAdjustmentsToSuffix = false,
            UseSpacesSeparator = false,
        };
        Assert.AreEqual(6, gen.TripletCount);
        Assert.AreEqual(3.3226281284987204E+19, gen.GetEstimatedCombinationCount());
        Assert.AreEqual(64, gen.GetEstimatedEntropyInBits());
    }

    [TestMethod]
    public void Generate6WithNumber() {
        var gen = new TripletPasswordGenerator() {
            TripletCount = 6,
            IncludeNumber = true,
            IncludeSpecialCharacters = false,
            IncludeSwappedCase = false,
            DropOneCharacter = false,
            UseTitlecase = false,
            RestrictWordBreakup = false,
            RestrictAdjustmentsToSuffix = false,
            UseSpacesSeparator = false,
        };
        Assert.AreEqual(6, gen.TripletCount);
        Assert.AreEqual(6.6452562569974405E+22, gen.GetEstimatedCombinationCount());
        Assert.AreEqual(75, gen.GetEstimatedEntropyInBits());
    }

    [TestMethod]
    public void Generate6WithNumberAndSpecial() {
        var gen = new TripletPasswordGenerator() {
            TripletCount = 6,
            IncludeNumber = true,
            IncludeSpecialCharacters = true,
            IncludeSwappedCase = false,
            DropOneCharacter = false,
            UseTitlecase = false,
            RestrictWordBreakup = false,
            RestrictAdjustmentsToSuffix = false,
            UseSpacesSeparator = false,
        };
        Assert.AreEqual(6, gen.TripletCount);
        Assert.AreEqual(3.8542486290585156E+25, gen.GetEstimatedCombinationCount());
        Assert.AreEqual(84, gen.GetEstimatedEntropyInBits());
    }

    [TestMethod]
    public void Generate6WithNumberSpecialAndSwappedCase() {
        var gen = new TripletPasswordGenerator() {
            TripletCount = 6,
            IncludeNumber = true,
            IncludeSpecialCharacters = true,
            IncludeSwappedCase = true,
            DropOneCharacter = false,
            UseTitlecase = false,
            RestrictWordBreakup = false,
            RestrictAdjustmentsToSuffix = false,
            UseSpacesSeparator = false,
        };
        Assert.AreEqual(6, gen.TripletCount);
        Assert.AreEqual(2.809747250583658E+28, gen.GetEstimatedCombinationCount());
        Assert.AreEqual(94, gen.GetEstimatedEntropyInBits());
    }

    [TestMethod]
    public void Generate6WithNumberSpecialSwappedCaseAndIncomplete() {
        var gen = new TripletPasswordGenerator() {
            TripletCount = 6,
            IncludeNumber = true,
            IncludeSpecialCharacters = true,
            IncludeSwappedCase = true,
            DropOneCharacter = true,
            UseTitlecase = false,
            RestrictWordBreakup = false,
            RestrictAdjustmentsToSuffix = false,
            UseSpacesSeparator = false,
        };
        Assert.AreEqual(6, gen.TripletCount);
        Assert.AreEqual(2.0483057456754867E+31, gen.GetEstimatedCombinationCount());
        Assert.AreEqual(104, gen.GetEstimatedEntropyInBits());
    }

    [TestMethod]
    public void Generate6WithAllButTitle() {
        var gen = new TripletPasswordGenerator() {
            TripletCount = 6,
            IncludeNumber = true,
            IncludeSpecialCharacters = true,
            IncludeSwappedCase = true,
            DropOneCharacter = true,
            UseTitlecase = true,
            RestrictWordBreakup = false,
            RestrictAdjustmentsToSuffix = false,
            UseSpacesSeparator = false,
        };
        Assert.AreEqual(6, gen.TripletCount);
        Assert.AreEqual(2.0483057456754867E+31, gen.GetEstimatedCombinationCount());  // Title case alone doesn't change combination count
        Assert.AreEqual(104, gen.GetEstimatedEntropyInBits());
    }

    [TestMethod]
    public void Generate6WithAllButTitleAndBreakup() {
        var gen = new TripletPasswordGenerator() {
            TripletCount = 6,
            IncludeNumber = true,
            IncludeSpecialCharacters = true,
            IncludeSwappedCase = true,
            DropOneCharacter = true,
            UseTitlecase = true,
            RestrictWordBreakup = true,
            RestrictAdjustmentsToSuffix = false,
            UseSpacesSeparator = false,
        };
        Assert.AreEqual(6, gen.TripletCount);
        Assert.AreEqual(1.578700238462368E+27, gen.GetEstimatedCombinationCount());
        Assert.AreEqual(90, gen.GetEstimatedEntropyInBits());
    }

    [TestMethod]
    public void Generate6WithAllButTitleBreakupAndSuffixRestriction() {
        var gen = new TripletPasswordGenerator() {
            TripletCount = 6,
            IncludeNumber = true,
            IncludeSpecialCharacters = true,
            IncludeSwappedCase = true,
            DropOneCharacter = true,
            UseTitlecase = true,
            RestrictWordBreakup = true,
            RestrictAdjustmentsToSuffix = true,
            UseSpacesSeparator = false,
        };
        Assert.AreEqual(6, gen.TripletCount);
        Assert.AreEqual(1.9271243145292577E+23, gen.GetEstimatedCombinationCount());
        Assert.AreEqual(77, gen.GetEstimatedEntropyInBits());
    }

    [TestMethod]
    public void Generate6WithAllButAll() {
        var gen = new TripletPasswordGenerator() {
            TripletCount = 6,
            IncludeNumber = true,
            IncludeSpecialCharacters = true,
            IncludeSwappedCase = true,
            DropOneCharacter = true,
            UseTitlecase = true,
            RestrictWordBreakup = true,
            RestrictAdjustmentsToSuffix = true,
            UseSpacesSeparator = true,
        };
        Assert.AreEqual(6, gen.TripletCount);
        Assert.AreEqual(1.9271243145292577E+23, gen.GetEstimatedCombinationCount());  // Space separator alone doesn't change combination count
        Assert.AreEqual(77, gen.GetEstimatedEntropyInBits());
    }

}
