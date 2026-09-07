namespace Bimil;

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

public sealed partial class Document {

    /// <summary>
    /// Returns all available groups.
    /// </summary>
    public IReadOnlyList<string> GetGroupList() {
        var newGroupDict = new Dictionary<string, object?>(StringComparer.CurrentCultureIgnoreCase);
        var groups = new List<string>();

        foreach (var record in Records) {
            if (record is EntryRecord entryRecord) {
                if (newGroupDict.TryAdd(entryRecord.Group, null)) {
                    groups.Add(entryRecord.Group);
                }
            }
        }
        groups.Sort(StringComparer.CurrentCultureIgnoreCase.Compare);

        return groups.AsReadOnly();
    }

    /// <summary>
    /// Returns all entry records.
    /// </summary>
    public IReadOnlyList<EntryRecord> GetEntryList() {
        var list = new List<EntryRecord>();
        foreach (var record in Records) {
            if (record is EntryRecord entry) {
                list.Add(entry);
            }
        }

        list.Sort((item1, item2) => {
            var groupCompare = string.Compare(item1.Group, item2.Group, StringComparison.CurrentCultureIgnoreCase);
            if (groupCompare != 0) {  // first compare group
                return groupCompare;
            } else if (item1.Title.StartsWith('.') && !item2.Title.StartsWith('.')) {  //title starting with dot (hidden) should go at the end
                return +1;
            } else if (!item1.Title.StartsWith('.') && item2.Title.StartsWith('.')) {  //title starting with dot (hidden) should go at the end
                return -1;
            } else {  // lastly, compare title
                return string.Compare(item1.Title, item2.Title, StringComparison.CurrentCultureIgnoreCase);
            }
        });
        return list.AsReadOnly();
    }

    /// <summary>
    /// Returns filtered entry records.
    /// </summary>
    /// <param name="filter">Filter restriction.</param>
    /// <param name="group">Group restriction</param>
    /// <param name="includeHidden">If true, entries starting with dot will also be included.</param>
    /// <returns></returns>
    public IReadOnlyList<EntryRecord> GetEntryList(string filter, string? group, bool includeHidden = false) {
        var list = new List<EntryRecord>();

        if (filter.Contains('*', StringComparison.Ordinal)) {  // wildcard search
            var regexPattern = Regex.Escape(filter).Replace(@"\*", ".*");
            var regex = new Regex(regexPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

            foreach (var record in Records) {
                if (record is EntryRecord entry) {
                    if ((group == null) || string.Equals(entry.Group, group, StringComparison.CurrentCultureIgnoreCase)) {
                        var isHidden = entry.Title.StartsWith('.');
                        if (isHidden && !includeHidden) { continue; }

                        if (!regex.IsMatch(entry.Title)) { continue; }
                        list.Add(entry);
                    }
                }
            }
        } else {  // word search
            var filterWords = filter.Split(Array.Empty<char>(), StringSplitOptions.RemoveEmptyEntries);
            foreach (var record in Records) {
                if (record is EntryRecord entry) {
                    if ((group == null) || string.Equals(entry.Group, group, StringComparison.CurrentCultureIgnoreCase)) {
                        var isHidden = entry.Title.StartsWith('.');
                        if (isHidden && !includeHidden) { continue; }

                        if (filterWords.Length > 0) {
                            var allMatched = true;
                            foreach (var word in filterWords) {
                                if ((entry.Title.IndexOf(word, StringComparison.CurrentCultureIgnoreCase) < 0)
                                && (entry.Group.ToString().IndexOf(word, StringComparison.CurrentCultureIgnoreCase) < 0)) {
                                    allMatched = false;
                                    break;
                                }
                            }
                            if (!allMatched) { continue; }
                        } else if ((group == null) && (entry.Group != "")) {
                            continue;  // if group is not set, skip entries with group
                        }
                        list.Add(entry);
                    }
                }
            }
        }

        list.Sort((item1, item2) => {
            var groupCompare = string.Compare(item1.Group, item2.Group, StringComparison.CurrentCultureIgnoreCase);
            if (groupCompare != 0) {  // first compare group
                return groupCompare;
            } else if (item1.Title.StartsWith('.') && !item2.Title.StartsWith('.')) {  //title starting with dot (hidden) should go at the end
                return +1;
            } else if (!item1.Title.StartsWith('.') && item2.Title.StartsWith('.')) {  //title starting with dot (hidden) should go at the end
                return -1;
            } else {  // lastly, compare title
                return string.Compare(item1.Title, item2.Title, StringComparison.CurrentCultureIgnoreCase);
            }
        });
        return list.AsReadOnly();
    }


    #region Passphrase

    /// <summary>
    /// Returns true if passphrase matches.
    /// </summary>
    /// <param name="passphrase">Old passphrase.</param>
    public bool TryValidatePassphrase(string passphrase) {
        if (passphrase == null) { throw new ArgumentNullException(nameof(passphrase), "Passphrase cannot be null."); }
        return TryValidatePassphrase(Encoding.UTF8.GetBytes(passphrase), clearBytes: true);
    }

    /// <summary>
    /// Returns true if password matches.
    /// </summary>
    /// <param name="passphraseBytes">Old passphrase bytes. Caller has to avoid keeping bytes unencrypted in memory.</param>
    public bool TryValidatePassphrase(byte[] passphraseBytes) {
        return TryValidatePassphrase(passphraseBytes, clearBytes: false);
    }

    /// <summary>
    /// Returns true if password matches.
    /// </summary>
    /// <param name="passphraseBytes">Old passphrase bytes. Caller has to avoid keeping bytes unencrypted in memory.</param>
    /// <param name="clearBytes">If true, passphrase bytes will be cleared.</param>
    public bool TryValidatePassphrase(byte[] passphraseBytes, bool clearBytes) {
        if (passphraseBytes == null) { throw new ArgumentNullException(nameof(passphraseBytes), "Passphrase cannot be null."); }

        var currPassphraseBuffer = ActiveKeyBlock.Passphrase.GetBytes();
        try {
            if (currPassphraseBuffer.Length != passphraseBytes.Length) { return false; }
            for (var i = 0; i < currPassphraseBuffer.Length; i++) {
                if (currPassphraseBuffer[i] != passphraseBytes[i]) { return false; }
            }
            return true;
        } finally {
            CryptographicOperations.ZeroMemory(currPassphraseBuffer); //remove passphrase bytes from memory - nothing to do about the string. :(
            if (clearBytes) { CryptographicOperations.ZeroMemory(passphraseBytes); }
        }
    }

    /// <summary>
    /// Validates password and throws exception if password doesn't match.
    /// </summary>
    /// <param name="passphrase">Old passphrase.</param>
    public void ValidatePassphrase(string passphrase) {
        if (passphrase == null) { throw new ArgumentNullException(nameof(passphrase), "Passphrase cannot be null."); }
        ValidatePassphrase(Encoding.UTF8.GetBytes(passphrase), clearBytes: true);
    }

    /// <summary>
    /// Validates password and throws exception if password doesn't match.
    /// </summary>
    /// <param name="passphraseBytes">Old passphrase bytes. Caller has to avoid keeping bytes unencrypted in memory.</param>
    public void ValidatePassphrase(byte[] passphraseBytes) {
        ValidatePassphrase(passphraseBytes, clearBytes: false);
    }

    /// <summary>
    /// Validates password and throws exception if password doesn't match.
    /// </summary>
    /// <param name="passphraseBytes">Old passphrase bytes. Caller has to avoid keeping bytes unencrypted in memory.</param>
    /// <param name="clearBytes">If true, passphrase bytes will be cleared.</param>
    public void ValidatePassphrase(byte[] passphraseBytes, bool clearBytes) {
        if (!TryValidatePassphrase(passphraseBytes, clearBytes)) {
            throw new InvalidOperationException("Cannot validate passphrase.");
        }
    }

    /// <summary>
    /// Change password only if old password matches and returns whether operation was successful.
    /// </summary>
    /// <param name="oldPassphrase">Old password.</param>
    /// <param name="newPassphrase">New password.</param>
    public bool TryChangePassphrase(string oldPassphrase, string newPassphrase) {
        ArgumentNullException.ThrowIfNull(oldPassphrase);
        ArgumentNullException.ThrowIfNull(newPassphrase);

        var oldPassphraseBuffer = Encoding.UTF8.GetBytes(oldPassphrase);
        var newPassphraseBuffer = Encoding.UTF8.GetBytes(newPassphrase);
        return TryChangePassphrase(oldPassphraseBuffer, newPassphraseBuffer, clearBytes: true);
    }

    /// <summary>
    /// Change password only if old password matches and returns whether operation was successful.
    /// </summary>
    /// <param name="oldPassphraseBytes">Old password bytes. Caller has to avoid keeping bytes unencrypted in memory.</param>
    /// <param name="newPassphraseBytes">New password bytes. Caller has to avoid keeping bytes unencrypted in memory.</param>
    public bool TryChangePassphrase(byte[] oldPassphraseBytes, byte[] newPassphraseBytes) {
        return TryChangePassphrase(oldPassphraseBytes, newPassphraseBytes, clearBytes: false);
    }

    /// <summary>
    /// Change password only if old password matches and returns whether operation was successful.
    /// </summary>
    /// <param name="oldPassphraseBytes">Old password bytes. Caller has to avoid keeping bytes unencrypted in memory.</param>
    /// <param name="newPassphraseBytes">New password bytes. Caller has to avoid keeping bytes unencrypted in memory.</param>
    /// <param name="clearBytes">If true, bytes will be cleared.</param>
    public bool TryChangePassphrase(byte[] oldPassphraseBytes, byte[] newPassphraseBytes, bool clearBytes) {
        ArgumentNullException.ThrowIfNull(oldPassphraseBytes);
        ArgumentNullException.ThrowIfNull(newPassphraseBytes);

        try {
            if (TryValidatePassphrase(oldPassphraseBytes)) {
                ActiveKeyBlock.SetPassphrase(newPassphraseBytes);
                return true;
            }
            return false;
        } finally {
            if (clearBytes) {
                CryptographicOperations.ZeroMemory(oldPassphraseBytes);
                CryptographicOperations.ZeroMemory(newPassphraseBytes);
            }
        }
    }
    /// <summary>
    /// Change password only if old password matches and returns whether operation was successful.
    /// </summary>
    /// <param name="oldPassphrase">Old password.</param>
    /// <param name="newPassphrase">New password.</param>
    public void ChangePassphrase(string oldPassphrase, string newPassphrase) {
        ArgumentNullException.ThrowIfNull(oldPassphrase);
        ArgumentNullException.ThrowIfNull(newPassphrase);

        var oldPassphraseBuffer = Encoding.UTF8.GetBytes(oldPassphrase);
        var newPassphraseBuffer = Encoding.UTF8.GetBytes(newPassphrase);
        ChangePassphrase(oldPassphraseBuffer, newPassphraseBuffer, clearBytes: true);
    }

    /// <summary>
    /// Change password only if old password matches and returns whether operation was successful.
    /// </summary>
    /// <param name="oldPassphraseBytes">Old password bytes. Caller has to avoid keeping bytes unencrypted in memory.</param>
    /// <param name="newPassphraseBytes">New password bytes. Caller has to avoid keeping bytes unencrypted in memory.</param>
    public void ChangePassphrase(byte[] oldPassphraseBytes, byte[] newPassphraseBytes) {
        ChangePassphrase(oldPassphraseBytes, newPassphraseBytes, clearBytes: false);
    }

    /// <summary>
    /// Change password only if old password matches and returns whether operation was successful.
    /// </summary>
    /// <param name="oldPassphraseBytes">Old password bytes. Caller has to avoid keeping bytes unencrypted in memory.</param>
    /// <param name="newPassphraseBytes">New password bytes. Caller has to avoid keeping bytes unencrypted in memory.</param>
    /// <param name="clearBytes">If true, bytes will be cleared.</param>
    public void ChangePassphrase(byte[] oldPassphraseBytes, byte[] newPassphraseBytes, bool clearBytes) {
        ArgumentNullException.ThrowIfNull(oldPassphraseBytes);
        ArgumentNullException.ThrowIfNull(newPassphraseBytes);
        if (!TryChangePassphrase(oldPassphraseBytes, newPassphraseBytes, clearBytes)) {
            throw new InvalidOperationException("Cannot change passphrase.");
        }
    }

    /// <summary>
    /// Gets if passphrase exists.
    /// </summary>
    public bool HasPassphrase {
        get { return ActiveKeyBlock.Passphrase.Length > 0; }
    }

    #endregion Passphrase

}
