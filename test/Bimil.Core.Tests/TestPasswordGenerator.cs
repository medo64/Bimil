namespace Tests;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bimil.Core;

[TestClass]
public class TestPasswordGenerator {

    [TestMethod]
    public void GetNumberOfCombinations() {
        var combinations = PasswordGenerator.GetNumberOfCombinations((2, 8));
        Assert.AreEqual(256.0, combinations);
    }

    [TestMethod]
    public void GetNumberOfCombinationsMany() {
        var combinations = PasswordGenerator.GetNumberOfCombinations((2, 8), (2, 8));
        Assert.AreEqual(Math.Pow(2, 16), combinations);
    }

    [TestMethod]
    public void GetNumberOfCombinations10LettersAndNumbers() {
        var combinations = PasswordGenerator.GetNumberOfCombinations((26, 10), (26, 10), (10, 10));
        Assert.AreEqual(1.992814889520941E+38, combinations);
    }



    [TestMethod]
    public void CalculateCombinationStats() {
        PasswordGenerator.CalculateCombinationStats(1_000_000_000_000_000, out var crackDuration, out var securityLevel, out var entropyBits, new DateTimeOffset(2016, 1, 1, 0, 0, 0, TimeSpan.Zero));
        Assert.AreEqual(10, crackDuration.TotalSeconds);
        Assert.AreEqual(PasswordSecurityLevel.Low, securityLevel);
        Assert.AreEqual(49, entropyBits);
    }

    [TestMethod]
    public void CalculateCombinationStats2015() {
        PasswordGenerator.CalculateCombinationStats(1_000_000_000_000_000, out var crackDuration, out var securityLevel, out var entropyBits, new DateTimeOffset(2015, 1, 1, 0, 0, 0, TimeSpan.Zero));
        Assert.AreEqual(10, crackDuration.TotalSeconds);
        Assert.AreEqual(PasswordSecurityLevel.Low, securityLevel);
        Assert.AreEqual(49, entropyBits);
    }

    [TestMethod]
    public void CalculateCombinationStats2016() {
        PasswordGenerator.CalculateCombinationStats(1_000_000_000_000_000, out var crackDuration, out var securityLevel, out var entropyBits, new DateTimeOffset(2016, 1, 1, 0, 0, 0, TimeSpan.Zero));
        Assert.AreEqual(10, crackDuration.TotalSeconds);
        Assert.AreEqual(PasswordSecurityLevel.Low, securityLevel);
        Assert.AreEqual(49, entropyBits);
    }

    [TestMethod]
    public void CalculateCombinationStats2017() {
        PasswordGenerator.CalculateCombinationStats(1_000_000_000_000_000, out var crackDuration, out var securityLevel, out var entropyBits, new DateTimeOffset(2017, 1, 1, 0, 0, 0, TimeSpan.Zero));
        Assert.AreEqual(7.06, crackDuration.TotalSeconds, 0.01);
        Assert.AreEqual(PasswordSecurityLevel.Low, securityLevel);
        Assert.AreEqual(49, entropyBits);
    }

    [TestMethod]
    public void CalculateCombinationStats2024() {
        PasswordGenerator.CalculateCombinationStats(1_000_000_000_000_000, out var crackDuration, out var securityLevel, out var entropyBits, new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        Assert.AreEqual(0.62, crackDuration.TotalSeconds, 0.01);
        Assert.AreEqual(PasswordSecurityLevel.Low, securityLevel);
        Assert.AreEqual(49, entropyBits);
    }

    [TestMethod]
    public void CalculateCombinationStats2024Extra() {
        PasswordGenerator.CalculateCombinationStats(123_456_789_000_000_000, out var crackDuration, out var securityLevel, out var entropyBits, new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        Assert.AreEqual(77.01, crackDuration.TotalSeconds, 0.01);
        Assert.AreEqual(PasswordSecurityLevel.Low, securityLevel);
        Assert.AreEqual(56, entropyBits);
    }


    [TestMethod]
    public void GetCracksPerSecond2015() {
        var cps = PasswordGenerator.GetCracksPerSecond(new DateTimeOffset(2015, 1, 1, 0, 0, 0, TimeSpan.Zero));
        Assert.AreEqual(100_000_000_000_000.0, cps);
    }

    [TestMethod]
    public void GetCracksPerSecond2016() {
        var cps = PasswordGenerator.GetCracksPerSecond(new DateTimeOffset(2016, 1, 1, 0, 0, 0, TimeSpan.Zero));
        Assert.AreEqual(100_000_000_000_000.0, cps);
    }

    [TestMethod]
    public void GetCracksPerSecond2017() {
        var cps = PasswordGenerator.GetCracksPerSecond(new DateTimeOffset(2017, 1, 1, 0, 0, 0, TimeSpan.Zero));
        Assert.AreEqual(141_555_701_946_423.56, cps);
    }

    [TestMethod]
    public void GetCracksPerSecond2018() {
        var cps = PasswordGenerator.GetCracksPerSecond(new DateTimeOffset(2018, 1, 1, 0, 0, 0, TimeSpan.Zero));
        Assert.IsTrue(200_000_000_000_000.0 < cps);
        Assert.IsTrue(210_000_000_000_000.0 > cps);
    }

    [TestMethod]
    public void GetCracksPerSecond2019() {
        var cps = PasswordGenerator.GetCracksPerSecond(new DateTimeOffset(2019, 1, 1, 0, 0, 0, TimeSpan.Zero));
        Assert.AreEqual(283_111_403_892_847.1, cps);
    }

    [TestMethod]
    public void GetCracksPerSecond2020() {
        var cps = PasswordGenerator.GetCracksPerSecond(new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero));
        Assert.IsTrue(400_000_000_000_000.0 < cps);
        Assert.IsTrue(410_000_000_000_000.0 > cps);
    }

    [TestMethod]
    public void GetCracksPerSecond2022() {
        var cps = PasswordGenerator.GetCracksPerSecond(new DateTimeOffset(2022, 1, 1, 0, 0, 0, TimeSpan.Zero));
        Assert.IsTrue(800_000_000_000_000.0 < cps);
        Assert.IsTrue(810_000_000_000_000.0 > cps);
    }

    [TestMethod]
    public void GetCracksPerSecond2024() {
        var cps = PasswordGenerator.GetCracksPerSecond(new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        Assert.IsTrue(1_600_000_000_000_000.0 < cps);
        Assert.IsTrue(1_610_000_000_000_000.0 > cps);
    }

}
