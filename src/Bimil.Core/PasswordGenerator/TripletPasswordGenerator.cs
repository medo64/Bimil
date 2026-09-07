namespace Bimil;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Triplet-based password generator.
/// </summary>
public sealed class TripletPasswordGenerator : PasswordGenerator {

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public TripletPasswordGenerator()
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

    private int _tripletCount = 6;
    /// <summary>
    /// Gets/sets number of words to use.
    /// </summary>
    public int TripletCount {
        get { return _tripletCount; }
        set {
            if (value is < 2 or > 32) { throw new ArgumentOutOfRangeException(nameof(value), "Triplet count must be between 2 and 32."); }
            _tripletCount = value;
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

        var triplets = WordsLoader.GetAllTriplets();
        var selectedTriplets = new List<List<char>>();
        for (var i = 0; i < TripletCount; i++) {
            var tripletIndex = RandomNumberGenerator.GetInt32(triplets.Count);
            selectedTriplets.Add(new List<char>(triplets[tripletIndex]));
        }

        if (DropOneCharacter) {
            var tripletIndex = RestrictAdjustmentsToSuffix ? selectedTriplets.Count - 1 : RandomNumberGenerator.GetInt32(selectedTriplets.Count); //drop may be restricted to last triplet only
            if (RestrictWordBreakup) { //break restriction only removes last character
                selectedTriplets[tripletIndex].RemoveAt(selectedTriplets[tripletIndex].Count - 1);
            } else {
                var charIndex = RandomNumberGenerator.GetInt32(selectedTriplets[tripletIndex].Count);
                selectedTriplets[tripletIndex].RemoveAt(charIndex);
            }
        }

        if (IncludeSwappedCase) {
            var tripletIndex = RestrictAdjustmentsToSuffix ? 0 : RandomNumberGenerator.GetInt32(selectedTriplets.Count); //uppercase may be restricted to first triplet only
            int charIndex;
            if (RestrictWordBreakup || RestrictAdjustmentsToSuffix) { //break restriction only uppercases the first character.
                charIndex = 0;
            } else {
                charIndex = RandomNumberGenerator.GetInt32(selectedTriplets[tripletIndex].Count);
            }
            if (char.IsLower(selectedTriplets[tripletIndex][charIndex])) {
                selectedTriplets[tripletIndex][charIndex] = char.ToUpperInvariant(selectedTriplets[tripletIndex][charIndex]);
            } else {
                selectedTriplets[tripletIndex][charIndex] = char.ToLowerInvariant(selectedTriplets[tripletIndex][charIndex]);
            }
        }

        if (UseTitlecase) {
            foreach (var selectedTriplet in selectedTriplets) {
                selectedTriplet[0] = char.ToUpperInvariant(selectedTriplet[0]);
            }
        }

        if (IncludeNumber) {
            var tripletIndex = RestrictAdjustmentsToSuffix ? selectedTriplets.Count - 1 : RandomNumberGenerator.GetInt32(selectedTriplets.Count); //number may be restricted to last triplet only
            int charIndex;
            if (RestrictWordBreakup) { //break restriction only adds number before or after the triplet
                if (RestrictAdjustmentsToSuffix) {
                    charIndex = selectedTriplets[tripletIndex].Count;
                } else {
                    charIndex = (RandomNumberGenerator.GetInt32(2) == 0) ? 0 : selectedTriplets[tripletIndex].Count;
                }
            } else {
                charIndex = RandomNumberGenerator.GetInt32(selectedTriplets[tripletIndex].Count + 1);
            }
            var number = RandomNumberGenerator.GetInt32(100);
            selectedTriplets[tripletIndex].InsertRange(charIndex, number.ToString(CultureInfo.InvariantCulture));
        }

        if (IncludeSpecialCharacters) {
            var tripletIndex = RestrictAdjustmentsToSuffix ? selectedTriplets.Count - 1 : RandomNumberGenerator.GetInt32(selectedTriplets.Count); //special character may be restricted to last triplet only
            int charIndex;
            if (RestrictWordBreakup) { //break restriction only adds character before or after the triplet
                if (RestrictAdjustmentsToSuffix) {
                    charIndex = selectedTriplets[tripletIndex].Count;
                } else {
                    charIndex = (RandomNumberGenerator.GetInt32(2) == 0) ? 0 : selectedTriplets[tripletIndex].Count;
                }
            } else {
                charIndex = RandomNumberGenerator.GetInt32(selectedTriplets[tripletIndex].Count + 1);
            }

            var specialIndex = RandomNumberGenerator.GetInt32(SpecialCharacters.Length);
            selectedTriplets[tripletIndex].Insert(charIndex, SpecialCharacters[specialIndex]);
        }

        for (var i = 0; i < selectedTriplets.Count; i++) {
            if (UseSpacesSeparator && (sb.Length > 0)) { sb.Append(" "); }
            sb.Append(new string(selectedTriplets[i].ToArray()));
        }

        return sb.ToString();
    }

    private double GenerateCombinationCount() {  // this is really rough calculation assuming everybody knows exactly how password was created and it assumes all words are 5 characters only
        var triplets = WordsLoader.GetAllTriplets().Count;
        if (IncludeSwappedCase && !RestrictAdjustmentsToSuffix) { triplets *= (1 + (RestrictWordBreakup ? 1 : 2)); } //1 original + 3 characters (standard triplet length) that can be upper case; if break is restricted, only the first character will be upper-case
        if (DropOneCharacter && !RestrictAdjustmentsToSuffix) { triplets *= (1 + (RestrictWordBreakup ? 1 : 2)); } //1 original + 3 characters (standard triplet length) that can be upper case; if break is restricted, only the last character will be removed thus only doubling the space

        double tripletCombinations;
        if (RestrictAdjustmentsToSuffix) {
            var tripletsLast = WordsLoader.GetAllTriplets().Count;
            if (DropOneCharacter) { tripletsLast *= 2; }
            if (IncludeNumber) { tripletsLast *= 100; }
            if (IncludeSpecialCharacters) { tripletsLast *= SpecialCharacters.Length; }
            tripletCombinations = Math.Pow(triplets, TripletCount - 1) * tripletsLast;
        } else {
            tripletCombinations = Math.Pow(triplets, TripletCount);
            if (IncludeNumber) { tripletCombinations *= (RestrictWordBreakup ? 4 : 20) * 100; } //assume attacker knows number between 0 and 100 is to be inserted; if restricted, assume it knows it will be on triplet-breaks
            if (IncludeSpecialCharacters) { tripletCombinations *= (RestrictWordBreakup ? 1 : 20) * SpecialCharacters.Length; } //special character can be inserted in any triplet at any place; if break is restricted, only start and end are good
        }

        return tripletCombinations;
    }

    #endregion Generator
}
