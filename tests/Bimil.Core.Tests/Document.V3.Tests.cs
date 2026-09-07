namespace Tests;

using System;
using System.IO;
using System.Text;
using Bimil;
using Microsoft.VisualStudio.TestTools.UnitTesting;

public partial class DocumentTests {

    [TestMethod]
    public void Document_V3_Load_Empty() {
        var stream = GetResourceStream("Empty.psafe3");
        var doc = Document.Load(stream, Encoding.UTF8.GetBytes("changeme"));

        Assert.AreEqual(9, doc.Headers.Count);
        Assert.AreEqual(HeaderType.Version, doc.Headers[0].Type);
        Assert.AreEqual(HeaderType.Uuid, doc.Headers[1].Type);
        Assert.AreEqual(HeaderType.NonDefaultPreferences, doc.Headers[2].Type);
        Assert.AreEqual(HeaderType.TimestampOfLastSave, doc.Headers[3].Type);
        Assert.AreEqual(HeaderType.TimestampOfLastMasterPasswordChange, doc.Headers[4].Type);
        Assert.AreEqual(HeaderType.LastSavedByUser, doc.Headers[5].Type);
        Assert.AreEqual(HeaderType.LastSavedOnHost, doc.Headers[6].Type);
        Assert.AreEqual(HeaderType.WhatPerformedLastSave, doc.Headers[7].Type);
        Assert.AreEqual(HeaderType.RecentlyUsedEntries, doc.Headers[8].Type);

        Assert.AreEqual(new Version(3, 17, 0, 0), ((VersionHeader)doc.Headers[0]).Version);
        Assert.AreEqual(Guid.Parse("7f8dc27f-8e80-424d-8561-4e1ff54a366e"), ((UuidHeader)doc.Headers[1]).Uuid);
        Assert.AreEqual("B 5 0 ", ((TextHeader)doc.Headers[2]).Text);
        Assert.AreEqual(new DateTime(2026, 8, 16, 22, 50, 21, DateTimeKind.Utc), ((TimestampHeader)doc.Headers[3]).Timestamp);
        Assert.AreEqual(new DateTime(2026, 8, 16, 22, 48, 20, DateTimeKind.Utc), ((TimestampHeader)doc.Headers[4]).Timestamp);
        Assert.AreEqual("Josip", ((TextHeader)doc.Headers[5]).Text);
        Assert.AreEqual("WANDALF", ((TextHeader)doc.Headers[6]).Text);
        Assert.AreEqual("Password Safe V3.72.1", ((TextHeader)doc.Headers[7]).Text);
        Assert.AreEqual("0153caff65334548a48f376775f0dd2534", ((TextHeader)doc.Headers[8]).Text);

        Assert.AreEqual(DatabaseVersion.V3, doc.DatabaseVersion);
        Assert.AreEqual(1, doc.KeyBlocks.Count);
        Assert.AreEqual(327680U, doc.ActiveKeyBlock.IterationCount);
        Assert.IsFalse(doc.IsReadOnly);

        Assert.AreEqual(new Version(3, 17, 0, 0), doc.Version);
        Assert.AreEqual(Guid.Parse("7f8dc27f-8e80-424d-8561-4e1ff54a366e"), doc.Uuid);
        Assert.AreEqual(new DateTime(2026, 8, 16, 22, 50, 21, DateTimeKind.Utc), doc.LastSaveTime);
        Assert.AreEqual(new DateTime(2026, 8, 16, 22, 48, 20, DateTimeKind.Utc), doc.LastPasswordChangeTime);
        Assert.AreEqual("Josip", doc.LastSaveUser);
        Assert.AreEqual("WANDALF", doc.LastSaveHost);
        Assert.AreEqual("Password Safe V3.72.1", doc.LastSaveApplication);

        Assert.AreEqual(1, doc.Records.Count);

        Assert.AreEqual(4, doc.Records[0].Fields.Count);
        Assert.AreEqual(FieldType.Uuid, doc.Records[0].Fields[0].Type);
        Assert.AreEqual(FieldType.Title, doc.Records[0].Fields[1].Type);
        Assert.AreEqual(FieldType.Password, doc.Records[0].Fields[2].Type);
        Assert.AreEqual(FieldType.CreationTime, doc.Records[0].Fields[3].Type);

        Assert.AreEqual(Guid.Parse("65ffca53-4533-a448-8f37-6775f0dd2534"), ((UuidField)(((EntryRecord)(doc.Records[0])).Fields[0])).Uuid);
        Assert.AreEqual("1", ((TextField)(((EntryRecord)(doc.Records[0])).Fields[1])).Text);
        Assert.AreEqual("1", ((TextField)(((EntryRecord)(doc.Records[0])).Fields[2])).Text);
        Assert.AreEqual(new DateTime(2026, 8, 16, 22, 50, 16, DateTimeKind.Utc), ((TimestampField)(((EntryRecord)(doc.Records[0])).Fields[3])).Timestamp);

        Assert.AreEqual(Guid.Parse("65ffca53-4533-a448-8f37-6775f0dd2534"), ((EntryRecord)(doc.Records[0])).Uuid);
        Assert.AreEqual("1", ((EntryRecord)(doc.Records[0])).Title);
        Assert.AreEqual("", ((EntryRecord)(doc.Records[0])).Group);
        Assert.AreEqual(new DateTime(2026, 8, 16, 22, 50, 16, DateTimeKind.Utc), ((EntryRecord)(doc.Records[0])).CreationTime);
    }

    [TestMethod]
    public void Document_V3_LoadSaveLoad() {
        var stream = GetResourceStream("Empty.psafe3");
        var docIn = Document.Load(stream, Encoding.UTF8.GetBytes("changeme"));

        using var ms = new MemoryStream();
        docIn.Save(ms);
        ms.Position = 0;

        var doc = Document.Load(ms, Encoding.UTF8.GetBytes("changeme"));

        Assert.AreEqual(DatabaseVersion.V3, doc.DatabaseVersion);
        Assert.AreEqual(327680U, doc.ActiveKeyBlock.IterationCount);
        Assert.AreEqual("323C8AEB9DADA7EB304A4632CE6E84F863A95FB61AA4E23BA507D6D0EE49658F", Convert.ToHexString(doc.ActiveKeyBlock.KeyK.GetBytes()));
        Assert.AreEqual("28F1C1C1EA74FE64B00B27870FA1CC072A99080150484548456B4A0925B178CA", Convert.ToHexString(doc.ActiveKeyBlock.KeyL.GetBytes()));

        Assert.AreEqual(new Version(3, 17, 0, 0), doc.Version);
        Assert.AreEqual(Guid.Parse("7f8dc27f-8e80-424d-8561-4e1ff54a366e"), doc.Uuid);
        Assert.IsTrue(new DateTime(2026, 8, 16, 22, 50, 21, DateTimeKind.Utc) < doc.LastSaveTime);
        Assert.AreEqual(new DateTime(2026, 8, 16, 22, 48, 20, DateTimeKind.Utc), doc.LastPasswordChangeTime);
        Assert.AreEqual(Environment.UserName, doc.LastSaveUser);
        Assert.AreEqual(Environment.MachineName, doc.LastSaveHost);
        Assert.AreEqual("Bimil V1.0.0", doc.LastSaveApplication);

        Assert.AreEqual(4, doc.Records[0].Fields.Count);
        var record0 = (EntryRecord)(doc.Records[0]);
        Assert.AreEqual(Guid.Parse("65ffca53-4533-a448-8f37-6775f0dd2534"), record0.Uuid);
        Assert.AreEqual("1", record0.Title);
        Assert.AreEqual("", record0.Group);
        Assert.AreEqual(new DateTime(2026, 8, 16, 22, 50, 16, DateTimeKind.Utc), record0.CreationTime);

        var keyInK = Convert.ToHexString(docIn.ActiveKeyBlock.KeyK.GetBytes());
        var keyInL = Convert.ToHexString(docIn.ActiveKeyBlock.KeyL.GetBytes());
        var keyK = Convert.ToHexString(doc.ActiveKeyBlock.KeyK.GetBytes());
        var keyL = Convert.ToHexString(doc.ActiveKeyBlock.KeyL.GetBytes());
        Assert.AreEqual(keyInK, keyK);
        Assert.AreEqual(keyInL, keyL);
    }

    [TestMethod]
    public void Document_V3_LoadFromKey() {
        var stream = GetResourceStream("Empty.psafe3");
        var doc = Document.Load(stream, Encoding.UTF8.GetBytes("323C8AEB9DADA7EB304A4632CE6E84F863A95FB61AA4E23BA507D6D0EE49658F"));

        Assert.AreEqual(DatabaseVersion.V3, doc.DatabaseVersion);
        Assert.AreEqual(327680U, doc.ActiveKeyBlock.IterationCount);
        Assert.AreEqual("323C8AEB9DADA7EB304A4632CE6E84F863A95FB61AA4E23BA507D6D0EE49658F", Convert.ToHexString(doc.ActiveKeyBlock.KeyK.GetBytes()));
        Assert.AreEqual("", Convert.ToHexString(doc.ActiveKeyBlock.KeyL.GetBytes()));
        Assert.IsTrue(doc.IsReadOnly);

        Assert.AreEqual(new Version(3, 17, 0, 0), ((VersionHeader)doc.Headers[0]).Version);
        Assert.AreEqual(Guid.Parse("7f8dc27f-8e80-424d-8561-4e1ff54a366e"), ((UuidHeader)doc.Headers[1]).Uuid);
        Assert.AreEqual("B 5 0 ", ((TextHeader)doc.Headers[2]).Text);
        Assert.AreEqual(new DateTime(2026, 8, 16, 22, 50, 21, DateTimeKind.Utc), ((TimestampHeader)doc.Headers[3]).Timestamp);
        Assert.AreEqual(new DateTime(2026, 8, 16, 22, 48, 20, DateTimeKind.Utc), ((TimestampHeader)doc.Headers[4]).Timestamp);
        Assert.AreEqual("Josip", ((TextHeader)doc.Headers[5]).Text);
        Assert.AreEqual("WANDALF", ((TextHeader)doc.Headers[6]).Text);
        Assert.AreEqual("Password Safe V3.72.1", ((TextHeader)doc.Headers[7]).Text);
        Assert.AreEqual("0153caff65334548a48f376775f0dd2534", ((TextHeader)doc.Headers[8]).Text);

        Assert.AreEqual(1, doc.Records.Count);

        Assert.AreEqual(4, doc.Records[0].Fields.Count);
        Assert.AreEqual(FieldType.Uuid, doc.Records[0].Fields[0].Type);
        Assert.AreEqual(FieldType.Title, doc.Records[0].Fields[1].Type);
        Assert.AreEqual(FieldType.Password, doc.Records[0].Fields[2].Type);
        Assert.AreEqual(FieldType.CreationTime, doc.Records[0].Fields[3].Type);

        Assert.AreEqual(Guid.Parse("65ffca53-4533-a448-8f37-6775f0dd2534"), ((UuidField)(((EntryRecord)(doc.Records[0])).Fields[0])).Uuid);
        Assert.AreEqual("1", ((TextField)(((EntryRecord)(doc.Records[0])).Fields[1])).Text);
        Assert.AreEqual("1", ((TextField)(((EntryRecord)(doc.Records[0])).Fields[2])).Text);
        Assert.AreEqual(new DateTime(2026, 8, 16, 22, 50, 16, DateTimeKind.Utc), ((TimestampField)(((EntryRecord)(doc.Records[0])).Fields[3])).Timestamp);

        Assert.AreEqual(Guid.Parse("65ffca53-4533-a448-8f37-6775f0dd2534"), ((EntryRecord)(doc.Records[0])).Uuid);
        Assert.AreEqual("1", ((EntryRecord)(doc.Records[0])).Title);
        Assert.AreEqual("", ((EntryRecord)(doc.Records[0])).Group);
        Assert.AreEqual(new DateTime(2026, 8, 16, 22, 50, 16, DateTimeKind.Utc), ((EntryRecord)(doc.Records[0])).CreationTime);

        using var ms = new MemoryStream();
        Assert.ThrowsException<InvalidOperationException>(() =>  // cannot save because passphrase + key L is missing
            doc.Save(ms)
        );
    }


    [TestMethod]
    public void Document_V3_LoadIncorrectPassphrase() {
        var stream = GetResourceStream("Empty.psafe3");
        var ex = Assert.ThrowsException<FormatException>(() => {
            var doc = Document.Load(stream, Encoding.UTF8.GetBytes("dontchangeme"));
        });
        Assert.AreEqual("Passphrase mismatch.", ex.Message);
    }

    [TestMethod]
    public void Document_V3_LoadIncorrectKey() {
        var stream = GetResourceStream("Empty.psafe3");
        var ex = Assert.ThrowsException<FormatException>(() => {
            var doc = Document.Load(stream, Encoding.UTF8.GetBytes("0000000000000000000000000000000000000000000000000000000000000000"));
        });
        Assert.AreEqual("Key mismatch.", ex.Message);
    }

}
