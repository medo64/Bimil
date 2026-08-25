namespace Tests;

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bimil;

[TestClass]
public class RecordCollectionTests {

    [TestMethod]
    public void RecordCollection_New() {
        var records = new RecordCollection([]);
        Assert.AreEqual(0, records.Count);
    }

    [TestMethod]
    public void RecordCollection_Clear() {
        var records = new RecordCollection([]) {
            EntryRecord.Create(),
            EntryRecord.Create()
        };
        Assert.AreEqual(2, records.Count);
        Assert.AreEqual(1, records[0].Fields.Count);
        Assert.AreEqual(1, records[1].Fields.Count);

        records.Clear();
        Assert.AreEqual(0, records.Count);
    }

}
