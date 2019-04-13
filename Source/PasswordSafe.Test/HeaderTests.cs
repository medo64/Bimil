using System;
using Xunit;
using PwSafe = Medo.Security.Cryptography.PasswordSafe;

namespace PasswordSafe.Test {
    public class HeaderTests {

        [Fact(DisplayName = "PasswordSafe: Header: New")]
        public void Header_New() {
            var field = new PwSafe.Header(PwSafe.HeaderType.DatabaseName) { Text = "Test" };
            Assert.Equal("Test", field.Text);
        }

        [Fact(DisplayName = "PasswordSafe: Header: New (wrong type)")]
        public void Header_New_WrongType() {
            Assert.Throws<FormatException>(() => {
                var field = new PwSafe.Header(PwSafe.HeaderType.DatabaseName) { Uuid = new Guid() };
            });
        }


        [Fact(DisplayName = "PasswordSafe: Header: Add (read-only document)")]
        public void Header_ReadOnly() {
            Assert.Throws<NotSupportedException>(() => {
                var doc = new PwSafe.Document("Password");
                doc.Headers.Add(new PwSafe.Header(PwSafe.HeaderType.DatabaseName) { Text = "Test" });

                doc.IsReadOnly = true;
                doc.Headers[PwSafe.HeaderType.DatabaseName].Text = "NewName";
            });
        }

        [Fact(DisplayName = "PasswordSafe: Header: Indexer Get")]
        public void Header_ReadOnly_IndexerRead() {
            var doc = new PwSafe.Document("Password") { IsReadOnly = true };
            Assert.NotNull(doc.Headers[PwSafe.HeaderType.DatabaseName]);
            Assert.Equal("", doc.Headers[PwSafe.HeaderType.DatabaseName].Text);
        }

        [Fact(DisplayName = "PasswordSafe: Header: Indexer Set")]
        public void Header_ReadOnly_IndexerWrite() {
            Assert.Throws<NotSupportedException>(() => {
                var doc = new PwSafe.Document("Password") { IsReadOnly = true };
                doc.Headers[PwSafe.HeaderType.DatabaseName] = null;
            });
        }

    }
}
