namespace Tests;

using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bimil.Core;
using System.Collections.Generic;
using System;

[TestClass]
public class WordLoader {

    [TestMethod]
    public void All() {
        var words = WordsLoader.GetAll();
        Assert.AreNotEqual(0, words.Count);
    }

    [TestMethod]
    public void All_NoDuplicates() {
        var words = WordsLoader.GetAll();
        CheckForDuplicates(words);
    }


    [TestMethod]
    public void AllTriplets() {
        var words = WordsLoader.GetAll();
        var triplets = WordsLoader.GetAllTriplets();
        Assert.AreNotEqual(0, triplets.Count);
        Assert.IsTrue(words.Count > triplets.Count);
    }

    [TestMethod]
    public void AllTriplets_NoDuplicates() {
        var triplets = WordsLoader.GetAllTriplets();
        CheckForDuplicates(triplets);
    }


    [TestMethod]
    public void Bible() {
        var words = WordsLoader.GetFromBible();
        Assert.AreNotEqual(0, words.Count);
    }

    [TestMethod]
    public void English() {
        var words = WordsLoader.GetFromEnglish();
        Assert.AreNotEqual(0, words.Count);
    }

    [TestMethod]
    public void JaneAusten() {
        var words = WordsLoader.GetFromJaneAusten();
        Assert.AreNotEqual(0, words.Count);
    }

    [TestMethod]
    public void Names() {
        var words = WordsLoader.GetFromNames();
        Assert.AreNotEqual(0, words.Count);
    }

    [TestMethod]
    public void WilliamShakespeare() {
        var words = WordsLoader.GetFromWilliamShakespeare();
        Assert.AreNotEqual(0, words.Count);
    }


    [TestMethod]
    public void CommonPasswords() {
        var words = WordsLoader.GetFromCommonPasswords();
        Assert.IsTrue(words.Count > 10000);
    }


    [TestMethod]
    public void IsCommonPasswordWhole() {
        Assert.IsTrue(WordsLoader.IsCommonPassword("snowball"));
        Assert.IsTrue(WordsLoader.IsCommonPassword("Snowball"));
        Assert.IsFalse(WordsLoader.IsCommonPassword("73784snowball82803"));
        Assert.IsFalse(WordsLoader.IsCommonPassword("73784Snowball82803"));
        Assert.IsFalse(WordsLoader.IsCommonPassword("sjwqdltt"));
    }

    [TestMethod]
    public void IsCommonPasswordPartial() {
        Assert.IsTrue(WordsLoader.IsCommonPassword("snowball", partialMatch: true));
        Assert.IsTrue(WordsLoader.IsCommonPassword("Snowball", partialMatch: true));
        Assert.IsTrue(WordsLoader.IsCommonPassword("73784snowball82803", partialMatch: true));
        Assert.IsTrue(WordsLoader.IsCommonPassword("73784Snowball82803", partialMatch: true));
        Assert.IsFalse(WordsLoader.IsCommonPassword("sjwqdltt", partialMatch: true));
    }


    #region Helpers

    private static void CheckForDuplicates(IReadOnlyList<string> words) {
        var counter = new Dictionary<string, int>();
        foreach (var word in words) {
            if (counter.ContainsKey(word)) {
                counter[word]++;
            } else {
                counter.Add(word, 1);
            }
        }
        foreach (var count in counter) {
            Assert.AreEqual(1, count.Value, $"Duplicate word: {count.Key} ({count.Value} times)");
        }
    }

    #endregion Helpers
}
