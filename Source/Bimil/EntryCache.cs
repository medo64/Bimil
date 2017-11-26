using System.Collections.Generic;
using System.Text;
using Medo.Security.Cryptography.PasswordSafe;

namespace Bimil {
    internal class EntryCache {

        public EntryCache(Entry entry) {
            this.Entry = entry;

            this.Title = entry.Title;
            this.Group = entry.Group;
        }

        public Entry Entry { get; }
        public string Title { get; }
        public string Group { get; }

        public string MatchedText {
            get {
                if (_matchList != null) {
                    var sb = new StringBuilder();
                    foreach (var text in _matchList) {
                        if (sb.Length > 0) { sb.Append(", "); }
                        sb.Append(text);
                    }
                    return sb.ToString();
                } else {
                    return null;
                }
            }
        }

        private List<string> _matchList;

        public void AddMatch(string text) {
            if (_matchList == null) { _matchList = new List<string>(); }
            if (!_matchList.Contains(text)) {
                _matchList.Add(text);
            }
        }

    }
}
