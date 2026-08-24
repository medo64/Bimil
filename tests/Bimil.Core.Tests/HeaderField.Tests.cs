namespace Tests;

using System;
using Bimil;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class HeaderFieldTests {

    [TestMethod]
    public void HeaderField_Version() {
        var value = new Version(3, 2, 0, 0);
        var field = (VersionHeader)Header.Create(HeaderType.Version);
        field.Version = value;

        Assert.AreEqual(value, field.Version);
        Assert.AreEqual("0203", Convert.ToHexString(field.Data.GetBytes()));

        var value2 = new Version(3, 11, 0, 0);
        field.Version = value2;

        Assert.AreEqual(value2, field.Version);
        Assert.AreEqual("0B03", Convert.ToHexString(field.Data.GetBytes()));
    }

}
