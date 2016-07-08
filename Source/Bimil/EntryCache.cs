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

        public string MatchedText { get; private set; }

        public void AddMatch(string text) {
            if (string.IsNullOrEmpty(this.MatchedText)) {
                this.MatchedText = text;
            } else {
                this.MatchedText += ", " + text;
            }
        }

    }
}
