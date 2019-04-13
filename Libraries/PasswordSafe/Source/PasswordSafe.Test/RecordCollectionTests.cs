using System;
using Xunit;
using PwSafe = Medo.Security.Cryptography.PasswordSafe;

namespace PasswordSafe.Test {
    public class RecordCollectionTests {

        [Fact(DisplayName = "PasswordSafe: RecordCollection: Add")]
        public void RecordCollection_New() {
            var doc = new PwSafe.Document("Password");
            doc.Entries.Add(new PwSafe.Entry());
            doc.Entries[0].Records.Add(new PwSafe.Record(PwSafe.RecordType.Group) { Text = "Test" });

            Assert.True(string.Equals("Test", doc.Entries[0].Group, StringComparison.Ordinal));
        }


        [Fact(DisplayName = "PasswordSafe: RecordCollection: Add (read-only document)")]
        public void RecordCollection_ReadOnly() {
            Assert.Throws<NotSupportedException>(() => {
                var doc = new PwSafe.Document("Password");
                doc.Entries.Add(new PwSafe.Entry());
                doc.IsReadOnly = true;
                doc.Entries[0].Records.Add(new PwSafe.Record(PwSafe.RecordType.Group));
            });
        }

        [Fact(DisplayName = "PasswordSafe: RecordCollection: Indexer Get")]
        public void RecordCollection_ReadOnly_IndexerRead() {
            var doc = new PwSafe.Document("Password");
            doc.Entries.Add(new PwSafe.Entry());
            doc.IsReadOnly = true;
            Assert.Equal("", doc.Entries[0].Records[PwSafe.RecordType.Title].Text);
        }

        [Fact(DisplayName = "PasswordSafe: RecordCollection: Indexer Set")]
        public void RecordCollection_ReadOnly_IndexerWrite() {
            Assert.Throws<NotSupportedException>(() => {
                var doc = new PwSafe.Document("Password");
                doc.Entries.Add(new PwSafe.Entry());
                doc.IsReadOnly = true;
                doc.Entries[0].Records[PwSafe.RecordType.Title] = null;
            });
        }

    }
}
