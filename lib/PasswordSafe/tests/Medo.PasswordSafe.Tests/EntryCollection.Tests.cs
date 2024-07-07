using System;
using Xunit;
using PwSafe = Medo.Security.Cryptography.PasswordSafe;

namespace Tests;

public class EntryCollection_Tests {

    [Fact(DisplayName = "PasswordSafe: EntryCollection: Add")]
    public void EntryCollection_New() {
        var doc = new PwSafe.Document("Password");
        doc.Entries.Add(new PwSafe.Entry("Test"));

        Assert.Equal("Test", doc.Entries[0].Title);
    }


    [Fact(DisplayName = "PasswordSafe: EntryCollection: Add (read-only document")]
    public void EntryCollection_ReadOnly() {
        Assert.Throws<NotSupportedException>(() => {
            var doc = new PwSafe.Document("Password") { IsReadOnly = true };
            doc.Entries.Add(new PwSafe.Entry());
        });
    }

    [Fact(DisplayName = "PasswordSafe: EntryCollection: Indexer Get (read-only document)")]
    public void EntryCollection_ReadOnly_IndexerRead() {
        var doc = new PwSafe.Document("Password") { IsReadOnly = true };
        Assert.NotNull(doc.Entries["Test"]);
        Assert.Equal("", doc.Entries["Test"].Title);
        Assert.NotNull(doc.Entries["Test", PwSafe.RecordType.Title]);
        Assert.Equal("", doc.Entries["Test", PwSafe.RecordType.Title].Text);
    }

    [Fact(DisplayName = "PasswordSafe: EntryCollection: Indexer Set 2 (read-only document)")]
    public void EntryCollection_ReadOnly_IndexerWrite2() {
        Assert.Throws<NotSupportedException>(() => {
            var doc = new PwSafe.Document("Password");
            doc.Entries.Add(new PwSafe.Entry("A"));
            doc.IsReadOnly = true;
            doc.Entries.Remove("A", PwSafe.RecordType.EmailAddress);
        });
    }

    [Fact(DisplayName = "PasswordSafe: EntryCollection: Indexer Set 3 (read-only document)")]
    public void EntryCollection_ReadOnly_IndexerWrite3() {
        Assert.Throws<NotSupportedException>(() => {
            var doc = new PwSafe.Document("Password");
            doc.Entries.Add(new PwSafe.Entry("A"));
            doc.IsReadOnly = true;
            doc.Entries.Remove("A", PwSafe.RecordType.EmailAddress);
        });
    }

    [Fact(DisplayName = "PasswordSafe: EntryCollection: Indexer Set 4 (read-only document)")]
    public void EntryCollection_ReadOnly_IndexerWrite4() {
        Assert.Throws<NotSupportedException>(() => {
            var doc = new PwSafe.Document("Password");
            doc.Entries.Add(new PwSafe.Entry("X.Y", "A"));
            doc.IsReadOnly = true;
            doc.Entries.Remove("X.Y", "A", PwSafe.RecordType.EmailAddress);
        });
    }


    [Fact(DisplayName = "PasswordSafe: EntryCollection: Indexer Get via Title")]
    public void EntryCollection_IndexerReadByTitleNonEmpty() {
        var doc = new PwSafe.Document("Password");
        doc.Entries.Add(new PwSafe.Entry("A"));
        Assert.Equal("A", doc.Entries["A"].Title);

        doc.Entries["A"].Title = "B";
        Assert.Equal("B", doc.Entries["B"].Title);
    }

    [Fact(DisplayName = "PasswordSafe: EntryCollection: Indexer Get via Title (type non-empty)")]
    public void EntryCollection_IndexerReadByTitleTypeNonEmpty() {
        var doc = new PwSafe.Document("Password");
        doc.Entries.Add(new PwSafe.Entry("A"));
        Assert.Equal("A", doc.Entries["A"][PwSafe.RecordType.Title].Text);

        doc.Entries["A"].Title = "B";
        Assert.Equal("B", doc.Entries["B"][PwSafe.RecordType.Title].Text);
    }

    [Fact(DisplayName = "PasswordSafe: EntryCollection: Indexer Get via Group and Title (type non-empty)")]
    public void EntryCollection_IndexerReadByGroupTitleTypeNonEmpty() {
        var doc = new PwSafe.Document("Password");
        doc.Entries.Add(new PwSafe.Entry("X.Y", "A"));
        Assert.Equal("X.Y", doc.Entries["A"][PwSafe.RecordType.Group].Text);
        Assert.Equal("A", doc.Entries["A"][PwSafe.RecordType.Title].Text);

        doc.Entries["A"].Group = doc.Entries["A"].Group.Up();
        doc.Entries["A"].Title = "B";
        Assert.Equal("X", doc.Entries["B"][PwSafe.RecordType.Group].Text);
        Assert.Equal("B", doc.Entries["B"][PwSafe.RecordType.Title].Text);
    }


    [Fact(DisplayName = "PasswordSafe: EntryCollection: Indexer Get via Title")]
    public void EntryCollection_IndexerReadByTitle() {
        var doc = new PwSafe.Document("Password");
        Assert.NotEqual(Guid.Empty, doc.Entries["A"].Uuid);
        Assert.Equal("A", doc.Entries["A"].Title);
    }

    [Fact(DisplayName = "PasswordSafe: EntryCollection: Indexer Get via Title and Type")]
    public void EntryCollection_IndexerReadByTitleType() {
        var doc = new PwSafe.Document("Password");
        Assert.NotEqual(Guid.Empty, doc.Entries["A", PwSafe.RecordType.Uuid].Uuid);
        Assert.Equal("A", doc.Entries["A", PwSafe.RecordType.Title].Text);
        Assert.NotEqual(Guid.Empty, doc.Entries["A"][PwSafe.RecordType.Uuid].Uuid);
        Assert.Equal("A", doc.Entries["A"][PwSafe.RecordType.Title].Text);
    }

    [Fact(DisplayName = "PasswordSafe: EntryCollection: Indexer Get via Group, Title, and Type")]
    public void EntryCollection_IndexerReadByGroupTitleType() {
        var doc = new PwSafe.Document("Password");
        Assert.NotEqual(Guid.Empty, doc.Entries["X.Y", "A", PwSafe.RecordType.Uuid].Uuid);
        Assert.Equal("X.Y", doc.Entries["X.Y", "A", PwSafe.RecordType.Group].Text);
        Assert.Equal("A", doc.Entries["X.Y", "A", PwSafe.RecordType.Title].Text);
        Assert.NotEqual(Guid.Empty, doc.Entries["X.Y", "A"][PwSafe.RecordType.Uuid].Uuid);
        Assert.Equal("A", doc.Entries["X.Y", "A"][PwSafe.RecordType.Title].Text);
    }

}
