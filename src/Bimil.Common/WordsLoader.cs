namespace Bimil;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

internal static class WordsLoader {

    /// <summary>
    /// Returns list of words from all available sources.
    /// List is always alphabetically sorted.
    /// List contains no duplicates.
    /// </summary>
    public static IReadOnlyList<string> GetAll() {
        return AllWords.Value;
    }

    /// <summary>
    /// Returns list of triplets from all available sources.
    /// List is always alphabetically sorted.
    /// List contains no duplicates.
    /// </summary>
    public static IReadOnlyList<string> GetAllTriplets() {
        return AllTriplets.Value;
    }


    /// <summary>
    /// Returns true if password is found in common passwords list.
    /// Only full match is checked.
    /// </summary>
    /// <param name="password">Password to </param>
    public static bool IsCommonPassword(string password) {
        return IsCommonPassword(password, partialMatch: false);
    }

    /// <summary>
    /// Returns true if password is found in common passwords list.
    /// </summary>
    /// <param name="password">Password to </param>
    /// <param name="partialMatch">If true, given password is checked against partial matches too.</param>
    public static bool IsCommonPassword(string password, bool partialMatch) {
        if (partialMatch) {
            foreach (var word in CommonPasswords.Value) {
                if (word.Length <= 5) { continue; }  // don't match short words
                if (password.Contains(word, StringComparison.OrdinalIgnoreCase)) { return true; }
            }
        }
        return CommonPasswordsDict.Value.ContainsKey(password);
    }


    /// <summary>
    /// Returns list of all words from the Bible word source.
    /// </summary>
    internal static IReadOnlyList<string> GetFromBible() {
        return BibleWords.Value;
    }

    /// <summary>
    /// Returns list of all words from the English word source.
    /// </summary>
    internal static IReadOnlyList<string> GetFromEnglish() {
        return EnglishWords.Value;
    }

    /// <summary>
    /// Returns list of all words from the Jane Austen word source.
    /// </summary>
    internal static IReadOnlyList<string> GetFromJaneAusten() {
        return JaneAustenWords.Value;
    }

    /// <summary>
    /// Returns list of all words from the Names word source.
    /// </summary>
    internal static IReadOnlyList<string> GetFromNames() {
        return NamesWords.Value;
    }

    /// <summary>
    /// Returns list of all words from the William Shakespeare word source.
    /// </summary>
    internal static IReadOnlyList<string> GetFromWilliamShakespeare() {
        return WilliamShakespeareWords.Value;
    }


    /// <summary>
    /// Returns list of all words from the common password list.
    /// </summary>
    internal static IReadOnlyList<string> GetFromCommonPasswords() {
        return CommonPasswords.Value;
    }


    #region Helpers

    private static readonly Lazy<IReadOnlyList<string>> AllWords = new(() => {
        var sw = Stopwatch.StartNew();
        try {
            var list = new List<string>();
            var dict = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
            foreach (var words in new[] { GetFromBible(), GetFromEnglish(), GetFromJaneAusten(), GetFromNames(), GetFromWilliamShakespeare() }) {
                foreach (var word in words) {
                    if (!dict.ContainsKey(word)) {
                        dict.Add(word, null);
                        list.Add(word);
                    }
                }
            }
            return list.AsReadOnly();
        } finally {
            Debug.WriteLine($"Loading all words took {sw.ElapsedMilliseconds} ms");
        }
    });

    private static readonly Lazy<IReadOnlyList<string>> AllTriplets = new(() => {
        var sw = Stopwatch.StartNew();
        try {
            var list = new List<string>();
            var dict = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
            foreach (var word in GetAll()) {
                var triplet = word.Substring(0, 3);
                if (!dict.ContainsKey(triplet)) {
                    dict.Add(triplet, null);
                    list.Add(triplet);
                }
            }
            return list.AsReadOnly();
        } finally {
            Debug.WriteLine($"Loading triplets took {sw.ElapsedMilliseconds} ms");
        }
    });

    private static readonly Lazy<IReadOnlyList<string>> BibleWords = new(() => GetWordsFromResource("Bible.txt"));
    private static readonly Lazy<IReadOnlyList<string>> EnglishWords = new(() => GetWordsFromResource("English.txt"));
    private static readonly Lazy<IReadOnlyList<string>> JaneAustenWords = new(() => GetWordsFromResource("JaneAusten.txt"));
    private static readonly Lazy<IReadOnlyList<string>> NamesWords = new(() => GetWordsFromResource("Names.txt"));
    private static readonly Lazy<IReadOnlyList<string>> WilliamShakespeareWords = new(() => GetWordsFromResource("WilliamShakespeare.txt"));

    private static readonly Lazy<IReadOnlyList<string>> CommonPasswords = new(() => {
        var sw = Stopwatch.StartNew();
        try {
            var list = new List<string>();
            list.AddRange(GetWordsFromResource("CommonPasswords10K.txt", filterCommonPasswords: false));
            list.AddRange(GetWordsFromResource("CommonPasswords100K.txt", filterCommonPasswords: false));
            list.Sort();
            for (var i = list.Count - 2; i >= 0; i--) {
                if (list[i].Equals(list[i + 1], StringComparison.OrdinalIgnoreCase)) { list.RemoveAt(i); }
            }
            return list.AsReadOnly();
        } finally {
            Debug.WriteLine($"Loading common passwords took {sw.ElapsedMilliseconds} ms");
        }
    });

    private static Lazy<IReadOnlyDictionary<string, object?>> CommonPasswordsDict = new(() => {
        var dict = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        foreach (var word in GetFromCommonPasswords()) {
            if (!dict.ContainsKey(word)) { dict.Add(word, null); }
        }
        return dict;
    });

    private static IReadOnlyList<string> GetWordsFromResource(string resourceName, bool filterCommonPasswords = true) {
        var sw = Stopwatch.StartNew();
        try {
            var list = new List<string>();
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Bimil._Resources." + resourceName);
            if (stream == null) { throw new InvalidOperationException("Missing word list resource named " + resourceName); }
            using var textStream = new StreamReader(stream);
            var words = textStream.ReadToEnd().Split(new string[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var word in words) {
                if (word.Length == 0) { continue; }
                if (word.StartsWith("#", StringComparison.OrdinalIgnoreCase)) { continue; }
                if (filterCommonPasswords && CommonPasswordsDict.Value.ContainsKey(word)) { continue; }
                list.Add(word);  // no duplicate check
            }
            list.Sort(StringComparer.Ordinal);
            return list.AsReadOnly();
        } finally {
            Debug.WriteLine($"Loading {resourceName} took {sw.ElapsedMilliseconds} ms");
        }
    }

    #endregion Helpers

}
