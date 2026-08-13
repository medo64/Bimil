namespace Tests;

using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bimil;

public partial class TwofishTests {

    [DataTestMethod]
    [DataRow(CipherMode.CFB)]
    [DataRow(CipherMode.CTS)]
    public void Twofish_Exceptions_OnlyCbcAndEbcSupported(CipherMode mode) {
        Assert.ThrowsException<CryptographicException>(() => {
            var _ = new Twofish() { Mode = mode };
        });
    }

}
