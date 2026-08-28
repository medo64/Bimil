namespace Tests;

using System;
using System.Text;
using Bimil;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class ProtectedBytesTests {

    [TestMethod]
    public void ProtectedBytes_Basic() {
        var bytes = Encoding.UTF8.GetBytes("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.");

         var bytesIn = new byte[bytes.Length];
         Array.Copy(bytes, 0, bytesIn, 0, bytesIn.Length);

        var pb = new ProtectedBytes();
        pb.SetBytes(bytesIn);
        var bytesOut = pb.GetBytes();

        Assert.AreEqual(123, pb.Length);
        Assert.AreEqual(Convert.ToHexString(bytes), Convert.ToHexString(bytesIn));
        Assert.AreEqual(Convert.ToHexString(bytes), Convert.ToHexString(bytesOut));
    }

    [TestMethod]
    public void ProtectedBytes_Zeroed() {
        var bytes = Encoding.UTF8.GetBytes("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.");

         var bytesIn = new byte[bytes.Length];
         Array.Copy(bytes, 0, bytesIn, 0, bytesIn.Length);

        var pb = new ProtectedBytes();
        pb.SetBytes(bytesIn, zeroBytes: true);
        var bytesOut = pb.GetBytes();

        Assert.AreEqual(123, pb.Length);
        Assert.AreEqual("", Convert.ToHexString(bytesIn).Replace("0",""));
        Assert.AreEqual(Convert.ToHexString(bytes), Convert.ToHexString(bytesOut));
    }

    [TestMethod]
    public void ProtectedBytes_Static() {
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
    public void ProtectedBytes_ReadOnly() {
        var bytes = Encoding.UTF8.GetBytes("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.");
        var pb = new ProtectedBytes(bytes);
        pb.IsReadOnly = true;
        Assert.ThrowsException<InvalidOperationException>(() =>
            pb.SetBytes([])
        );
        Assert.AreEqual(Convert.ToHexString(bytes), Convert.ToHexString(pb.GetBytes()));
    }

}
