using System;
using Xunit;
using PwSafe = Medo.Security.Cryptography.PasswordSafe;

namespace PasswordSafe.Test {
    public class HeaderCollectionTests {

        [Fact(DisplayName = "PasswordSafe: HeaderCollection: Add")]
        public void HeaderCollection_New() {
            var doc = new PwSafe.Document("Password");
            doc.Headers.Add(new PwSafe.Header(PwSafe.HeaderType.DatabaseName) { Text = "Test" });

            Assert.Equal("Test", doc.Name);
        }


        [Fact(DisplayName = "PasswordSafe: HeaderCollection: Add (read-only document)")]
        public void HeaderCollection_ReadOnly() {
            Assert.Throws<NotSupportedException>(() => {
                var doc = new PwSafe.Document("Password") { IsReadOnly = true };
                doc.Headers.Add(new PwSafe.Header(PwSafe.HeaderType.DatabaseName) { Text = "Test" });
            });
        }

        [Fact(DisplayName = "PasswordSafe: HeaderCollection: Indexer Get")]
        public void HeaderCollection_ReadOnly_IndexerRead() {
            var doc = new PwSafe.Document("Password") { IsReadOnly = true };
            Assert.NotNull(doc.Headers[PwSafe.HeaderType.DatabaseName]);
            Assert.Equal("", doc.Headers[PwSafe.HeaderType.DatabaseName].Text);
        }

        [Fact(DisplayName = "PasswordSafe: HeaderCollection: Indexer Set")]
        public void HeaderCollection_ReadOnly_IndexerWrite() {
            Assert.Throws<NotSupportedException>(() => {
                var doc = new PwSafe.Document("Password") { IsReadOnly = true };
                doc.Headers[PwSafe.HeaderType.DatabaseName] = null;
            });
        }

    }
}
