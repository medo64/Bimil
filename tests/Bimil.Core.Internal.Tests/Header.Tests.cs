namespace Tests;

using System;
using Bimil;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class HeaderTests {

    [TestMethod]
    public void Header_ReadOnly() {
        var field = Header.Create(HeaderType.Uuid, UuidHeader.GetBytes(Guid.AllBitsSet));

        field.IsReadOnly = true;
        Assert.ThrowsException<InvalidOperationException>(() =>
            field.Data.SetBytes([])
        );
        Assert.AreEqual(Convert.ToHexString(UuidHeader.GetBytes(Guid.AllBitsSet)), Convert.ToHexString(field.Data.GetBytes()));

        field.IsReadOnly = false;
        field.Data.SetBytes(UuidHeader.GetBytes(Guid.Empty));
        Assert.AreEqual(Convert.ToHexString(UuidHeader.GetBytes(Guid.Empty)), Convert.ToHexString(field.Data.GetBytes()));
    }

}
