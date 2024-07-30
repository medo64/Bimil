namespace Bimil.Core;

using System;
using System.Collections.Generic;
using System.IO;
using Medo.Security.Cryptography.PasswordSafe;

/// <summary>
/// State of the application.
/// </summary>
public static class State {

    /// <summary>
    /// Gets currently open document.
    /// </summary>
    public static Document? Document { get; private set; }

    /// <summary>
    /// Gets currently open file.
    /// </summary>
    public static FileInfo? File { get; private set; }

    /// <summary>
    /// Raised whenever state needs an update.
    /// </summary>
    public static event EventHandler<EventArgs>? DocumentChanged;

    /// <summary>
    /// Raised whenever categories need an update.
    /// </summary>
    public static event EventHandler<EventArgs>? GroupsChanged;

    /// <summary>
    /// Creates a new file.
    /// </summary>
    /// <param name="passphrase">Passphrase.</param>
    public static void NewFile(string passphrase) {
        Document = new Document(passphrase);
        File = null;

        Document.Changed += (s, e) => { UpdateGroupsAfterChange(raiseEventIfDifferent: true); };
        UpdateGroupsAfterChange(raiseEventIfDifferent: false);  // initial update

        DocumentChanged?.Invoke(null, EventArgs.Empty);
        GroupsChanged?.Invoke(null, EventArgs.Empty);
    }

    /// <summary>
    /// Open an existing file.
    /// </summary>
    /// <param name="file">File.</param>
    /// <param name="passphrase">Passphrase.</param>
    public static bool OpenFile(FileInfo file, string passphrase, bool @readonly) {
        Document = Document.Load(file.FullName, passphrase);
        Document.IsReadOnly = @readonly;
        File = file;

        Document.Changed += (s, e) => { UpdateGroupsAfterChange(raiseEventIfDifferent: true); };
        UpdateGroupsAfterChange(raiseEventIfDifferent: false);  // initial update

        DocumentChanged?.Invoke(null, EventArgs.Empty);
        GroupsChanged?.Invoke(null, EventArgs.Empty);
        return true;
    }

    /// <summary>
    /// Forces raising of document change event.
    /// </summary>
    public static void RaiseDocumentChange() {
        DocumentChanged?.Invoke(null, EventArgs.Empty);
    }


    private static readonly List<string> _groups = [];

    /// <summary>
    /// Enumerates all categories present in document.
    /// </summary>
    /// <returns></returns>
    public static IReadOnlyList<string> GetGroups() {
        if (Document == null) { return Array.Empty<string>(); }
        return _groups;
    }


    private static void UpdateGroupsAfterChange(bool raiseEventIfDifferent) {
        var newGroupDict = new Dictionary<string, object?>(StringComparer.CurrentCultureIgnoreCase);
        if (Document != null) {
            foreach (var entry in Document.Entries) {
                newGroupDict.TryAdd(entry.Group, null);
            }
            var newGroups = new List<string>(newGroupDict.Keys);
            newGroups.Sort(StringComparer.CurrentCultureIgnoreCase);
            var hasChanges = newGroups.Count != _groups.Count;
            if (!hasChanges) {
                for (var i = 0; i < newGroups.Count; i++) {
                    if (newGroups[i] != _groups[i]) {
                        hasChanges = true;
                        break;
                    }
                }
            }
            if (hasChanges) {
                _groups.Clear();
                _groups.AddRange(newGroups);
                if (raiseEventIfDifferent) { GroupsChanged?.Invoke(null, EventArgs.Empty); }
            }
        } else if (_groups.Count > 0) {  // no document
            _groups.Clear();
        }
    }

}
