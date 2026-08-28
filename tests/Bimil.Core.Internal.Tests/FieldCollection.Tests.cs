namespace Tests;

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bimil;

[TestClass]
public class FieldCollectionTests {

    [TestMethod]
    public void FieldCollection_New() {
        var fields = new FieldCollection([]);
        Assert.AreEqual(0, fields.Count);
    }

    [TestMethod]
    public void FieldCollection_NewWithFirstUuid() {
        var fields = new FieldCollection([], FieldType.Uuid);
        Assert.AreEqual(1, fields.Count);
        Assert.AreNotEqual(Guid.Empty, ((UuidField)fields[0]).Uuid);
    }


    [TestMethod]
    public void FieldCollection_Clear() {
        var fields = new FieldCollection([]) {
            Field.Create(FieldType.Uuid),
            Field.Create(FieldType.Title)
        };
        Assert.AreEqual(2, fields.Count);

        fields.Clear();
        Assert.AreEqual(0, fields.Count);
    }

    [TestMethod]
    public void FieldCollection_RemoveAt0() {
        var fields = new FieldCollection([]) {
            Field.Create(FieldType.Uuid),
            Field.Create(FieldType.Title)
        };
        Assert.AreEqual(2, fields.Count);

        fields.RemoveAt(0);
    }

    [TestMethod]
    public void FieldCollection_Remove0() {
        var fields = new FieldCollection([]) {
            Field.Create(FieldType.Uuid),
            Field.Create(FieldType.Title)
        };
        fields.Remove(fields[FieldType.Uuid]);
        Assert.AreEqual(1, fields.Count);
    }

    [TestMethod]
    public void FieldCollection_ClearWithFirstUuid() {
        var fields = new FieldCollection([], FieldType.Uuid) {
            Field.Create(FieldType.Title)
        };
        Assert.AreEqual(2, fields.Count);

        fields.Clear();
        Assert.AreEqual(1, fields.Count);
        Assert.AreNotEqual(Guid.Empty, ((UuidField)fields[0]).Uuid);
    }


    [TestMethod]
    public void FieldCollection_NewCannotUseFirstBaseUuid() {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => {
            var fields = new FieldCollection([], FieldType.BaseUuid);
        });
    }

    [TestMethod]
    public void FieldCollection_NewCannotUseFirstNonUuidField() {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => {
            var fields = new FieldCollection([], FieldType.Title);
        });
    }

    [TestMethod]
    public void FieldCollection_CannotRemoveFirstUuid() {
        var fields = new FieldCollection([], FieldType.Uuid) {
            Field.Create(FieldType.Title)
        };
        Assert.AreEqual(2, fields.Count);

        Assert.ThrowsException<InvalidOperationException>(() =>
            fields.RemoveAt(0)
        );
        Assert.ThrowsException<InvalidOperationException>(() =>
            fields.Remove(fields[FieldType.Uuid])
        );
    }

    [TestMethod]
    public void FieldCollection_ReadOnly() {
        var field = Field.Create(FieldType.Uuid, UuidField.GetBytes(Guid.AllBitsSet));
        var fields = new FieldCollection([field]);
        Assert.AreEqual(1, fields.Count);

        fields.IsReadOnly = true;
        Assert.IsTrue(fields[0].IsReadOnly);
        Assert.ThrowsException<InvalidOperationException>(() =>
            fields[0].Data.SetBytes([])
        );
        Assert.ThrowsException<InvalidOperationException>(() =>
            fields.Add(Field.Create(FieldType.Attachment3MediaType))
        );
        Assert.ThrowsException<InvalidOperationException>(() =>
            fields.Remove(Field.Create(FieldType.Attachment3MediaType))
        );
        Assert.ThrowsException<InvalidOperationException>(() =>
            fields.Insert(0, Field.Create(FieldType.Attachment3MediaType))
        );
        Assert.AreEqual(Convert.ToHexString(UuidField.GetBytes(Guid.AllBitsSet)), Convert.ToHexString(fields[0].Data.GetBytes()));

        fields.IsReadOnly = false;
        field.Data.SetBytes(UuidField.GetBytes(Guid.Empty));
        Assert.AreEqual(Convert.ToHexString(UuidField.GetBytes(Guid.Empty)), Convert.ToHexString(fields[0].Data.GetBytes()));
    }

}
