namespace Bimil.Core;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Word-based password generator.
/// </summary>
public sealed class WordPasswordGenerator : PasswordGenerator {

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public WordPasswordGenerator()
        : base() {
    }


    /// <summary>
    /// Returns newly generated password.
    /// </summary>
    public override string GetNewPassword() {
        return GeneratePassword();
    }

    /// <summary>
    /// Gets number of combinations based on the current settings.
    /// Assumption is made that the potential attacker knows exactly which
    /// method and dictionary were used to generate password (i.e. the worst
    /// case scenario).
    /// </summary>
    public override double GetEstimatedCombinationCount() {
        return GenerateCombinationCount();
    }


    #region Properties

    private int _wordCount = 5;
    /// <summary>
    /// Gets/sets number of words to use.
    /// </summary>
    public int WordCount {
        get { return _wordCount; }
        set {
            if (value is < 2 or > 16) { throw new ArgumentOutOfRangeException(nameof(value), "Word count must be between 2 and 16."); }
            _wordCount = value;
        }
    }



    /// <summary>
    /// Gets/sets if a number will be included in addition.
    /// </summary>
    public bool IncludeNumber { get; set; } = true;

    /// <summary>
    /// Gets/sets if special characters () are included in the generated password.
    /// Special characters are: ~ ! @ # $ % ^ & * ( ) - _ = + [ { ] } \ | ; : , < . > / ?
    /// </summary>
    public bool IncludeSpecialCharacters { get; set; } = true;

    /// <summary>
    /// Gets/sets if some words will have their case changed.
    /// </summary>
    public bool IncludeSwappedCase { get; set; } = true;

    /// <summary>
    /// Gets/sets if some word will have a character removed.
    /// </summary>
    public bool DropOneCharacter { get; set; } = true;


    /// <summary>
    /// Get/sets if words will be made to use title case.
    /// </summary>
    public bool UseTitlecase { get; set; } = false;

    /// <summary>
    /// Gets/sets if any non-word characters will be only appended or prepended to the word instead of being in the middle.
    /// /summary>
    public bool RestrictWordBreakup { get; set; } = false;

    /// <summary>
    /// Gets/sets if any non-word characters will be only appended at the end of password.
    /// </summary>
    public bool RestrictAdjustmentsToSuffix { get; set; } = false;

    /// <summary>
    /// Gets/sets if words will be separated by a space.
    /// </summary>
    public bool UseSpacesSeparator { get; set; } = false;

    #endregion Properties


    #region Generator

    private readonly char[] SpecialCharacters = ['~', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '_', '=', '+', '[', '{', ']', '}', '\\', '|', ';', ':', ',', '<', '.', '>', '/', '?'];

    private string GeneratePassword() {
        var sb = new StringBuilder();

        var words = WordsLoader.GetAll();

        var selectedWords = new List<List<char>>();
        for (var i = 0; i < WordCount; i++) {
            var wordIndex = RandomNumberGenerator.GetInt32(words.Count);
            selectedWords.Add(new List<char>(words[wordIndex]));
        }

        if (DropOneCharacter) {
            var wordIndex = RestrictAdjustmentsToSuffix ? selectedWords.Count - 1 : RandomNumberGenerator.GetInt32(selectedWords.Count); //incomplete may be restricted to last word only
            if (RestrictWordBreakup) { //break restriction only removes last character
                selectedWords[wordIndex].RemoveAt(selectedWords[wordIndex].Count - 1);
            } else {
                var charIndex = RandomNumberGenerator.GetInt32(selectedWords[wordIndex].Count);
                selectedWords[wordIndex].RemoveAt(charIndex);
            }
        }

        if (UseTitlecase) {
            foreach (var selectedWord in selectedWords) {
                selectedWord[0] = char.ToUpperInvariant(selectedWord[0]);
            }
        }

        if (IncludeSwappedCase) {
            var wordIndex = RestrictAdjustmentsToSuffix ? 0 : RandomNumberGenerator.GetInt32(selectedWords.Count); //uppercase may be restricted to first word only
            int charIndex;
            if (RestrictWordBreakup || RestrictAdjustmentsToSuffix) { //break restriction only uppercases the first character.
                charIndex = 0;
            } else {
                charIndex = RandomNumberGenerator.GetInt32(selectedWords[wordIndex].Count);
            }
            if (char.IsLower(selectedWords[wordIndex][charIndex])) {
                selectedWords[wordIndex][charIndex] = char.ToUpperInvariant(selectedWords[wordIndex][charIndex]);
            } else {
                selectedWords[wordIndex][charIndex] = char.ToLowerInvariant(selectedWords[wordIndex][charIndex]);
            }
        }

        if (IncludeNumber) {
            var wordIndex = RestrictAdjustmentsToSuffix ? selectedWords.Count - 1 : RandomNumberGenerator.GetInt32(selectedWords.Count); //number may be restricted to last word only
            int charIndex;
            if (RestrictWordBreakup) { //break restriction only adds number before or after the word
                if (RestrictAdjustmentsToSuffix) {
                    charIndex = selectedWords[wordIndex].Count;
                } else {
                    charIndex = (RandomNumberGenerator.GetInt32(2) == 0) ? 0 : selectedWords[wordIndex].Count;
                }
            } else {
                charIndex = RandomNumberGenerator.GetInt32(selectedWords[wordIndex].Count + 1);
            }
            var number = RandomNumberGenerator.GetInt32(100);
            selectedWords[wordIndex].InsertRange(charIndex, number.ToString(CultureInfo.InvariantCulture));
        }

        if (IncludeSpecialCharacters) {
            var wordIndex = RestrictAdjustmentsToSuffix ? selectedWords.Count - 1 : RandomNumberGenerator.GetInt32(selectedWords.Count); //special character may be restricted to last word only
            int charIndex;
            if (RestrictWordBreakup) { //break restriction only adds character before or after the word
                if (RestrictAdjustmentsToSuffix) {
                    charIndex = selectedWords[wordIndex].Count;
                } else {
                    charIndex = (RandomNumberGenerator.GetInt32(2) == 0) ? 0 : selectedWords[wordIndex].Count;
                }
            } else {
                charIndex = RandomNumberGenerator.GetInt32(selectedWords[wordIndex].Count + 1);
            }
            var specialIndex = RandomNumberGenerator.GetInt32(SpecialCharacters.Length);
            selectedWords[wordIndex].Insert(charIndex, SpecialCharacters[specialIndex]);
        }

        for (var i = 0; i < selectedWords.Count; i++) {
            if (UseSpacesSeparator && (sb.Length > 0)) { sb.Append(" "); }
            sb.Append(new string(selectedWords[i].ToArray()));
        }

        return sb.ToString();
    }

    private double GenerateCombinationCount() {  // this is really rough calculation assuming everybody knows exactly how password was created and it assumes all words are 5 characters only
        var words = WordsLoader.GetAll().Count;
        if (IncludeSwappedCase && !RestrictAdjustmentsToSuffix) { words *= (1 + (RestrictWordBreakup ? 1 : 4)); } //1 original + 5 characters (shortest length) that can be upper case; if break is restricted, only the first character will be upper-case
        if (DropOneCharacter && !RestrictAdjustmentsToSuffix) { words *= (1 + (RestrictWordBreakup ? 1 : 4)); } //1 original + 5 characters (shortest length) that can be upper case; if break is restricted, only the last character will be removed thus only doubling the space

        double wordCombinations;
        if (RestrictAdjustmentsToSuffix) {
            var wordsLast = WordsLoader.GetAll().Count;
            if (DropOneCharacter) { wordsLast *= 2; }
            if (IncludeNumber) { wordsLast *= 100; }
            if (IncludeSpecialCharacters) { wordsLast *= SpecialCharacters.Length; }
            wordCombinations = Math.Pow(words, WordCount - 1) * wordsLast;
        } else {
            wordCombinations = Math.Pow(words, WordCount);
            if (IncludeNumber) { wordCombinations *= (RestrictWordBreakup ? 4 : 20) * 100; } //assume attacker knows number between 0 and 100 is to be inserted; if restricted, assume it knows it will be on word-breaks
            if (IncludeSpecialCharacters) { wordCombinations *= (RestrictWordBreakup ? 1 : 20) * SpecialCharacters.Length; } //special character can be inserted in any word at any place; if break is restricted, only start and end are good
        }

        return wordCombinations;
    }

    #endregion Generator
}
