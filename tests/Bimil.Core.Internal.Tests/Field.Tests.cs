namespace Tests;

using System;
using Bimil;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class FieldTests {

    [TestMethod]
    public void Field_ReadOnly() {
        var field = Field.Create(FieldType.Uuid, UuidField.GetBytes(Guid.AllBitsSet));

        field.IsReadOnly = true;
        Assert.ThrowsException<InvalidOperationException>(() =>
            field.Data.SetBytes([])
        );
        Assert.AreEqual(Convert.ToHexString(UuidField.GetBytes(Guid.AllBitsSet)), Convert.ToHexString(field.Data.GetBytes()));

        field.IsReadOnly = false;
        field.Data.SetBytes(UuidField.GetBytes(Guid.Empty));
        Assert.AreEqual(Convert.ToHexString(UuidField.GetBytes(Guid.Empty)), Convert.ToHexString(field.Data.GetBytes()));
    }

}
