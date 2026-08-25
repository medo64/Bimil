namespace Tests;

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Bimil;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public partial class DocumentTests {

    [TestMethod]
    public void Document_New() {
        var doc = new Document();
        Assert.AreEqual(DatabaseVersion.V3, doc.DatabaseVersion);
        Assert.AreEqual(new Version(3, 17, 0, 0), doc.Version);
        Assert.AreNotEqual(Guid.Empty, doc.Uuid);
    }

    [TestMethod]
    public void Document_New_V3() {
        var doc = new Document(DatabaseVersion.V3);
        Assert.AreEqual(DatabaseVersion.V3, doc.DatabaseVersion);
        Assert.AreEqual(new Version(3, 17, 0, 0), doc.Version);
        Assert.AreNotEqual(Guid.Empty, doc.Uuid);
    }

    [TestMethod]
    public void Document_New_V4() {
        var doc = new Document(DatabaseVersion.V4);
        Assert.AreEqual(DatabaseVersion.V4, doc.DatabaseVersion);
        Assert.AreEqual(new Version(4, 2, 0, 0), doc.Version);
        Assert.AreNotEqual(Guid.Empty, doc.Uuid);
    }

    [TestMethod]
    public void Document_ClearIdentityData() {
        var stream = GetResourceStream("Empty.psafe3");
        var doc = Document.Load(stream, Encoding.UTF8.GetBytes("changeme"));
        doc.ClearIdentityData();

        Assert.AreEqual(6, doc.Headers.Count);

        Assert.AreEqual(new Version(3, 17, 0, 0), doc.Version);
        Assert.AreEqual(Guid.Parse("7f8dc27f-8e80-424d-8561-4e1ff54a366e"), doc.Uuid);
        Assert.AreEqual(new DateTime(2026, 8, 16, 22, 50, 21, DateTimeKind.Utc), doc.LastSaveTime);
        Assert.AreEqual(new DateTime(2026, 8, 16, 22, 48, 20, DateTimeKind.Utc), doc.LastPasswordChangeTime);
        Assert.AreEqual("", doc.LastSaveUser);
        Assert.AreEqual("", doc.LastSaveHost);
        Assert.AreEqual("", doc.LastSaveApplication);
    }

    [TestMethod]
    public void Document_ClearTimestamps() {
        var stream = GetResourceStream("Empty.psafe3");
        var doc = Document.Load(stream, Encoding.UTF8.GetBytes("changeme"));
        doc.ClearTimestamps();

        Assert.AreEqual(7, doc.Headers.Count);

        Assert.AreEqual(new Version(3, 17, 0, 0), doc.Version);
        Assert.AreEqual(Guid.Parse("7f8dc27f-8e80-424d-8561-4e1ff54a366e"), doc.Uuid);
        Assert.AreEqual(DateTime.MinValue, doc.LastSaveTime);
        Assert.AreEqual(DateTime.MinValue, doc.LastPasswordChangeTime);
        Assert.AreEqual("Josip", doc.LastSaveUser);
        Assert.AreEqual("WANDALF", doc.LastSaveHost);
        Assert.AreEqual("Password Safe V3.72.1", doc.LastSaveApplication);
    }

    [TestMethod]
    public void Document_HashCode() {
        var stream = GetResourceStream("Empty.psafe3");
        var doc = Document.Load(stream, Encoding.UTF8.GetBytes("changeme"));
        var hc1 = doc.GetHashCode();

        doc.Headers.Add(Header.Create(HeaderType.EmptyGroups));
        var hc2 = doc.GetHashCode();

        doc.Records.Add(EntryRecord.Create());
        var hc3 = doc.GetHashCode();

        doc.Records[0].Fields.Add(Field.Create(FieldType.Group));
        var hc4 = doc.GetHashCode();

        doc.Version = new Version(3, 0, 0, 0);
        var hc5 = doc.GetHashCode();

        ((EntryRecord)doc.Records[0]).Uuid = Guid.CreateVersion7();
        var hc6 = doc.GetHashCode();

        Assert.IsTrue(doc.HasChanged);
        Assert.AreNotEqual(hc1, hc2); Assert.AreNotEqual(hc1, hc3); Assert.AreNotEqual(hc1, hc4); Assert.AreNotEqual(hc1, hc5); Assert.AreNotEqual(hc1, hc6);
        Assert.AreNotEqual(hc2, hc3); Assert.AreNotEqual(hc2, hc4); Assert.AreNotEqual(hc2, hc5); Assert.AreNotEqual(hc2, hc6);
        Assert.AreNotEqual(hc3, hc4); Assert.AreNotEqual(hc3, hc5); Assert.AreNotEqual(hc3, hc6);
        Assert.AreNotEqual(hc4, hc5); Assert.AreNotEqual(hc4, hc6);
        Assert.AreNotEqual(hc5, hc6);

        var ms = new MemoryStream();
        doc.Save(ms);
        Assert.IsFalse(doc.HasChanged);
    }

    [TestMethod]
    public void Document_HashCodeNotChanged() {
        var stream = GetResourceStream("Empty.psafe3");
        var doc = Document.Load(stream, Encoding.UTF8.GetBytes("changeme"));
        var hc1 = doc.GetHashCode();

        doc.Uuid = doc.Uuid;
        var hc2 = doc.GetHashCode();

        ((EntryRecord)doc.Records[0]).Uuid = ((EntryRecord)doc.Records[0]).Uuid;
        var hc3 = doc.GetHashCode();

        Assert.IsFalse(doc.HasChanged);
        Assert.AreEqual(hc1, hc2); Assert.AreEqual(hc1, hc3);
        Assert.AreEqual(hc2, hc3);
    }

}
