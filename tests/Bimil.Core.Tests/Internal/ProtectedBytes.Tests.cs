namespace Tests;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Bimil;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class ProtectedBytesTests {

    [TestMethod]
    public void ProtectedBytes_Basic() {
        var bytes = Encoding.UTF8.GetBytes("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.");

        var bytes1PT = new byte[bytes.Length];
        Array.Copy(bytes, 0, bytes1PT, 0, bytes1PT.Length);

        var bytes1CT = ProtectedBytes.ProtectData(bytes1PT);
        Assert.AreEqual(Convert.ToHexString(bytes), Convert.ToHexString(bytes1PT));
        Assert.AreNotEqual(Convert.ToHexString(bytes), Convert.ToHexString(bytes1CT));

        var bytes2CT = new byte[bytes1CT.Length];
        Array.Copy(bytes1CT, 0, bytes2CT, 0, bytes2CT.Length);

        var bytes2PT = ProtectedBytes.UnprotectData(bytes2CT);
        Assert.AreEqual(Convert.ToHexString(bytes1CT), Convert.ToHexString(bytes2CT));
        Assert.AreEqual(Convert.ToHexString(bytes), Convert.ToHexString(bytes2PT));

    }

    [TestMethod]
    public void ProtectedBytes_Zeroed() {
        var bytes = Encoding.UTF8.GetBytes("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.");

        var bytes1PT = new byte[bytes.Length];
        Array.Copy(bytes, 0, bytes1PT, 0, bytes1PT.Length);

        var bytes1CT = ProtectedBytes.ProtectData(bytes1PT, zeroUserData: true);
        Assert.AreEqual("", Convert.ToHexString(bytes1PT).Replace("0", ""));  // clears source buffer
        Assert.AreNotEqual(Convert.ToHexString(bytes), Convert.ToHexString(bytes1CT));

        var bytes2CT = new byte[bytes1CT.Length];
        Array.Copy(bytes1CT, 0, bytes2CT, 0, bytes2CT.Length);

        var bytes2PT = ProtectedBytes.UnprotectData(bytes2CT, zeroEncryptedData: true);
        Assert.AreEqual("", Convert.ToHexString(bytes2CT).Replace("0", ""));  // clears source buffer
        Assert.AreEqual(Convert.ToHexString(bytes), Convert.ToHexString(bytes2PT));

    }

}
