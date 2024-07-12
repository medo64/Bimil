namespace Bimil.Core;

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Classic password generator.
/// </summary>
public sealed class ClassicPasswordGenerator : PasswordGenerator {

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public ClassicPasswordGenerator()
        : base() {
    }


    /// <summary>
    /// Returns newly generated password.
    /// </summary>
    public override string GetNewPassword() {
        if (IncludeLowercaseLetters || IncludeUppercaseLetters || IncludeDigits || IncludeSpecialCharacters) {
            return GeneratePassword();
        } else {
            throw new InvalidOperationException("At least one character set must be included.");
        }
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

    private int _passwordLength = 14;
    /// <summary>
    /// Gets/sets password length.
    /// </summary>
    public int PasswordLength {
        get { return _passwordLength; }
        set {
            if (value is < 8 or > 128) { throw new ArgumentOutOfRangeException(nameof(value), "Password length must be between 8 and 128 characters."); }
            _passwordLength = value;
        }
    }


    /// <summary>
    /// Gets/sets if lowercase letters (a-z) are included in the generated password.
    /// </summary>
    public bool IncludeLowercaseLetters { get; set; } = true;

    /// <summary>
    /// Gets/sets if uppercase letters (A-Z) are included in the generated password.
    /// </summary>
    public bool IncludeUppercaseLetters { get; set; } = true;

    /// <summary>
    /// Gets/sets if digits (0-9) are included in the generated password.
    /// </summary>
    public bool IncludeDigits { get; set; } = true;

    /// <summary>
    /// Gets/sets if special characters () are included in the generated password.
    /// Special characters are: ~ ! @ # $ % ^ & * ( ) - _ = + [ { ] } \ | ; : , < . > / ?
    /// </summary>
    public bool IncludeSpecialCharacters { get; set; } = true;


    /// <summary>
    /// Get/sets if ambiguous characters are excluded from the generated password.
    /// Ambiguous characters are: I O Q l 0 ` - _ ; : , .
    /// </summary>
    public bool ExcludeSimilarCharacters { get; set; } = false;

    /// <summary>
    /// Gets/sets if characters that are found at different places on common keyboards are excluded.
    /// Such characters are Y Z y z ` ~ # $ % - , .
    /// /summary>
    public bool ExcludeMovableCharacters { get; set; } = false;

    /// <summary>
    /// Gets/sets if attempt is made to make password pronounceable by alternating consonants and vowels.
    /// </summary>
    public bool ExcludeUnpronounceable { get; set; } = false;

    /// <summary>
    /// Gets/sets if repeated characters are excluded from the generated password.
    /// </summary>
    public bool ExcludeRepeatedCharacters { get; set; } = false;

    #endregion Properties


    #region Generator

    private readonly char[] UppercaseConsonants = ['B', 'C', 'D', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'V', 'W', 'X', 'Z'];
    private readonly char[] UppercaseVowels = ['A', 'E', 'I', 'O', 'U', 'Y'];
    private readonly char[] LowercaseConsonants = ['b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'z'];
    private readonly char[] LowercaseVowels = ['a', 'e', 'i', 'o', 'u', 'y'];
    private readonly char[] Digits = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9'];
    private readonly char[] SpecialCharacters = ['~', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '_', '=', '+', '[', '{', ']', '}', '\\', '|', ';', ':', ',', '<', '.', '>', '/', '?'];

    private readonly char[] SimilarCharacters = ['I', 'O', 'Q', 'l', '0', '`', '-', '_', ';', ':', ',', '.'];
    private readonly char[] MoveableCharacters = ['Y', 'Z', 'y', 'z', '`', '~', '#', '$', '%', '-', ',', '.'];

    private string GeneratePassword() {
        while (true) {
            var sb = new StringBuilder();

            var useVowelNext = false;
            while (sb.Length < PasswordLength) {
                var sixteenth = RandomNumberGenerator.GetInt32(16);

                var characters = new List<char>();
                if (IncludeUppercaseLetters && (sixteenth >= 0) && (sixteenth <= 5)) { //Uppercase: 6/16th ~ 37.5%
                    if (ExcludeUnpronounceable) {
                        AddCharactersToList(characters, useVowelNext ? UppercaseVowels : UppercaseConsonants);
                        useVowelNext = !useVowelNext;
                    } else {
                        AddCharactersToList(characters, UppercaseVowels, UppercaseConsonants);
                    }
                } else if (IncludeLowercaseLetters && (sixteenth >= 6) && (sixteenth <= 11)) { //Lowercase: 6/16th ~ 37.5%
                    if (ExcludeUnpronounceable) {
                        AddCharactersToList(characters, useVowelNext ? LowercaseVowels : LowercaseConsonants);
                        useVowelNext = !useVowelNext;
                    } else {
                        AddCharactersToList(characters, LowercaseVowels, LowercaseConsonants);
                    }
                } else if (IncludeDigits && (sixteenth >= 12) && (sixteenth <= 13)) { //Number: 2/16th ~ 12.5%
                    if (ExcludeUnpronounceable && !useVowelNext) { continue; } //treat numbers as vowels
                    AddCharactersToList(characters, Digits);
                    useVowelNext = false;
                } else if (IncludeSpecialCharacters && (sixteenth >= 14) && (sixteenth <= 15)) { //Number: 2/16th ~ 12.5%
                    if (ExcludeUnpronounceable && !useVowelNext) { continue; } //treat specials as vowels
                    AddCharactersToList(characters, SpecialCharacters);
                    useVowelNext = false;
                }

                if (ExcludeSimilarCharacters) { RemoveCharactersFromList(characters, SimilarCharacters); }
                if (ExcludeMovableCharacters) { RemoveCharactersFromList(characters, MoveableCharacters); }

                if (characters.Count > 0) {
                    var charIndex = RandomNumberGenerator.GetInt32(characters.Count);
                    var nextChar = characters[charIndex];
                    if (ExcludeRepeatedCharacters && (sb.Length > 1) && (sb[^1] == nextChar)) { continue; }
                    sb.Append(nextChar);
                }
            }

            int countUpper = 0, countLower = 0, countNumber = 0, countSpecial = 0;
            for (var i = 0; i < sb.Length; i++) {
                if ((Array.IndexOf(LowercaseConsonants, sb[i]) > 0) || (Array.IndexOf(LowercaseVowels, sb[i]) > 0)) { countLower += 1; }
                if ((Array.IndexOf(UppercaseConsonants, sb[i]) > 0) || (Array.IndexOf(UppercaseVowels, sb[i]) > 0)) { countUpper += 1; }
                if (Array.IndexOf(Digits, sb[i]) > 0) { countNumber += 1; }
                if (Array.IndexOf(SpecialCharacters, sb[i]) > 0) { countSpecial += 1; }
            }

            //another loop if one of selected is missing
            if (IncludeLowercaseLetters && (countLower == 0)) { continue; }
            if (IncludeUppercaseLetters && (countUpper == 0)) { continue; }
            if (IncludeDigits && (countNumber == 0)) { continue; }
            if (IncludeSpecialCharacters && (countSpecial == 0)) { continue; }

            return sb.ToString();
        }
    }

    private double GenerateCombinationCount() {
        var allCharacters = new List<char>();
        var vowelCharacters = new List<char>();
        var consonantCharacters = new List<char>();

        if (IncludeUppercaseLetters) {
            AddCharactersToList(allCharacters, UppercaseVowels, UppercaseConsonants);
            AddCharactersToList(vowelCharacters, UppercaseVowels);
            AddCharactersToList(consonantCharacters, UppercaseConsonants);
        }
        if (IncludeLowercaseLetters) {
            AddCharactersToList(allCharacters, LowercaseVowels, LowercaseConsonants);
            AddCharactersToList(vowelCharacters, LowercaseVowels);
            AddCharactersToList(consonantCharacters, LowercaseConsonants);
        }
        if (IncludeDigits) {
            AddCharactersToList(allCharacters, Digits);
            AddCharactersToList(vowelCharacters, Digits);
            AddCharactersToList(consonantCharacters, Digits);
        }
        if (IncludeSpecialCharacters) {
            AddCharactersToList(allCharacters, SpecialCharacters);
            AddCharactersToList(vowelCharacters, SpecialCharacters);
            AddCharactersToList(consonantCharacters, SpecialCharacters);
        }

        if (ExcludeSimilarCharacters) {
            RemoveCharactersFromList(allCharacters, SimilarCharacters);
            RemoveCharactersFromList(vowelCharacters, SimilarCharacters);
            RemoveCharactersFromList(consonantCharacters, SimilarCharacters);
        }
        if (ExcludeMovableCharacters) {
            RemoveCharactersFromList(allCharacters, MoveableCharacters);
            RemoveCharactersFromList(vowelCharacters, MoveableCharacters);
            RemoveCharactersFromList(consonantCharacters, MoveableCharacters);
        }

        //double combinations;
        if (ExcludeUnpronounceable) {
            var vowels = PasswordLength / 2;
            var consonants = PasswordLength - vowels;
            if (ExcludeRepeatedCharacters) {
                return Math.Pow(vowelCharacters.Count - 1, vowels)
                     * Math.Pow(consonantCharacters.Count - 1, consonants - 1)
                     * (vowelCharacters.Count + consonantCharacters.Count);
            } else {
                return Math.Pow(vowelCharacters.Count, vowels)
                     * Math.Pow(consonantCharacters.Count, consonants - 1)
                     * (vowelCharacters.Count + consonantCharacters.Count);
            }
        } else {
            if (ExcludeRepeatedCharacters) {
                return allCharacters.Count
                     * Math.Pow(allCharacters.Count - 1, PasswordLength - 1);
            } else {
                return Math.Pow(allCharacters.Count, PasswordLength);
            }
        }
    }

    private static void AddCharactersToList(List<char> characterList, params ICollection<char>[] characterCollections) {
        foreach (var characterCollection in characterCollections) {
            characterList.AddRange(characterCollection);
        }
    }

    private static void RemoveCharactersFromList(List<char> characterList, params ICollection<char>[] characterCollections) {
        foreach (var characterCollection in characterCollections) {
            foreach (var character in characterCollection) {
                characterList.Remove(character);
            }
        }
    }

    #endregion Generator
}
