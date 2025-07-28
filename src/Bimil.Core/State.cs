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
    /// Raised whenever groups need an update.
    /// </summary>
    public static event EventHandler<EventArgs>? GroupsChanged;

    /// <summary>
    /// Raised whenever items need an update.
    /// </summary>
    public static event EventHandler<EventArgs>? ItemsChanged;


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
        ItemsChanged?.Invoke(null, EventArgs.Empty);
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
        ItemsChanged?.Invoke(null, EventArgs.Empty);

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
        return _groups.AsReadOnly();
    }

    public static IReadOnlyList<Entry> GetEntries(string filter, string? group, bool includeHidden = false) {
        if (Document == null) { return Array.Empty<Entry>(); }

        var list = new List<Entry>();
        foreach (var entry in Document.Entries) {
            if ((group == null) || string.Equals(entry.Group, group, StringComparison.CurrentCultureIgnoreCase)) {
                var isHidden = entry.Title.StartsWith('.');
                if (isHidden && !includeHidden) { continue; }
                if (string.IsNullOrEmpty(filter)) {
                    if ((group == null) && (entry.Group != "")) { continue; }  // if group is not set, skip entries with group
                } else {
                    if (entry.Title.IndexOf(filter, StringComparison.CurrentCultureIgnoreCase) < 0) { continue; }
                    // TODO better filter
                }
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
