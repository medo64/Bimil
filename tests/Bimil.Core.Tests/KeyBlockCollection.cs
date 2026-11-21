namespace Tests;

using System;
using Bimil;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class KeyBlockCollectionTests {

    [TestMethod]
    public void KeyBlockCollection_Default() {
        var doc = new Document(DatabaseVersion.V3);

        Assert.AreEqual(1, doc.KeyBlocks.Count);
        Assert.AreSame(doc.ActiveKeyBlock, doc.KeyBlocks[0]);
    }

    [TestMethod]
    public void KeyBlockCollection_Clear() {
        var doc = new Document(DatabaseVersion.V3);
        doc.KeyBlocks.Clear();

        Assert.AreEqual(1, doc.KeyBlocks.Count);
        Assert.AreSame(doc.ActiveKeyBlock, doc.KeyBlocks[0]);
    }

    [TestMethod]
    public void KeyBlockCollection_AppendKey() {
        var doc = new Document(DatabaseVersion.V4);
        doc.KeyBlocks.AppendNew([1]);
        Assert.AreEqual(2, doc.KeyBlocks.Count);
    }

    [TestMethod]
    public void KeyBlockCollection_CannotAppendV3() {
        var doc = new Document(DatabaseVersion.V3);
        Assert.ThrowsExactly<NotSupportedException>(() => {
            doc.KeyBlocks.AppendNew([1]);
        });
    }

}
