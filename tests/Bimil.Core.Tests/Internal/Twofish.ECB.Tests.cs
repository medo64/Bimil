namespace Tests;

using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bimil;

public partial class TwofishTests {

    [TestMethod]
    public void Twofish_ECB_MonteCarlo_Encrypt_One() {
        var tests = GetTestBlocks(GetResourceStream("ECB_E_M.TXT"));
        var test = tests[Random.Shared.Next(tests.Count)];
        MonteCarlo_ECB_E(test);
    }

    [TestMethod]
    public void Twofish_ECB_MonteCarlo_Decrypt_One() {
        var tests = GetTestBlocks(GetResourceStream("ECB_D_M.TXT"));
        var test = tests[Random.Shared.Next(tests.Count)];
        MonteCarlo_ECB_D(test);
    }


    [TestMethod]
    public void Twofish_ECB_KnownAnswers() {
        var tests = GetTestBlocks(GetResourceStream("ECB_TBL.TXT"));
        foreach (var test in tests) {
            using var algorithm = new Twofish() { KeySize = test.KeySize, Mode = CipherMode.ECB, Padding = PaddingMode.None };
            var ct = Encrypt(algorithm, test.Key, null, test.PlainText);
            Assert.AreEqual(BitConverter.ToString(test.CipherText), BitConverter.ToString(ct));

            var pt = Decrypt(algorithm, test.Key, null, test.CipherText);
            Assert.AreEqual(BitConverter.ToString(test.PlainText), BitConverter.ToString(pt));
        }
    }

    [Ignore]
    [TestMethod]
    public void Twofish_ECB_MonteCarlo_Encrypt() { //takes ages
        var tests = GetTestBlocks(GetResourceStream("ECB_E_M.TXT"));
        var sw = Stopwatch.StartNew();
        foreach (var test in tests) {
            MonteCarlo_ECB_E(test);
        }
        sw.Stop();
        Debug.WriteLine("Duration: " + sw.ElapsedMilliseconds.ToString() + " ms");
    }

    [Ignore]
    [TestMethod]
    public void Twofish_ECB_MonteCarlo_Decrypt() { //takes ages
        var tests = GetTestBlocks(GetResourceStream("ECB_D_M.TXT"));
        var sw = Stopwatch.StartNew();
        foreach (var test in tests) {
            MonteCarlo_ECB_D(test);
        }
        sw.Stop();
        Debug.WriteLine("Duration: " + sw.ElapsedMilliseconds.ToString() + " ms");
    }


    [DataTestMethod]
    [DataRow(PaddingMode.None)]
    [DataRow(PaddingMode.PKCS7)]
    [DataRow(PaddingMode.Zeros)]
    [DataRow(PaddingMode.ANSIX923)]
    [DataRow(PaddingMode.ISO10126)]
    public void Twofish_ECB_Randomised(PaddingMode padding) {
        for (var n = 0; n < 1000; n++) {
            var crypto = new Twofish() { Padding = padding, Mode = CipherMode.ECB };
            crypto.GenerateKey();
            crypto.GenerateIV();
            var data = new byte[Random.Shared.Next(100)];
            if (padding is PaddingMode.None) { data = new byte[data.Length / 16 * 16]; }  // make it rounded number if no padding
            RandomNumberGenerator.Fill(data);
            if ((padding == PaddingMode.Zeros) && (data.Length > 0)) { data[^1] = 1; }  // zero padding needs to have the last number non-zero

            var ct = Encrypt(crypto, crypto.Key, crypto.IV, data);
            if (padding is PaddingMode.None or PaddingMode.Zeros) {
                Assert.IsTrue(data.Length <= ct.Length);
            } else {
                Assert.IsTrue(data.Length < ct.Length);
            }

            var pt = Decrypt(crypto, crypto.Key, crypto.IV, ct);
            Assert.AreEqual(data.Length, pt.Length);
            Assert.AreEqual(BitConverter.ToString(data), BitConverter.ToString(pt));
        }
    }

    [DataTestMethod]
    [DataRow(PaddingMode.None)]
    [DataRow(PaddingMode.PKCS7)]
    [DataRow(PaddingMode.Zeros)]
    [DataRow(PaddingMode.ANSIX923)]
    [DataRow(PaddingMode.ISO10126)]
    public void Twofish_ECB_EncryptDecrypt(PaddingMode padding) {
        var crypto = new Twofish() { Padding = padding, Mode = CipherMode.ECB };
        crypto.GenerateKey();
        crypto.GenerateIV();
        var bytes = RandomNumberGenerator.GetBytes(1024);
        var bytesEnc = new byte[bytes.Length];
        var bytesDec = new byte[bytes.Length];

        var sw = Stopwatch.StartNew();
        using var encryptor = crypto.CreateEncryptor();
        using var decryptor = crypto.CreateDecryptor();
        for (var n = 0; n < 1024; n++) {
            encryptor.TransformBlock(bytes, 0, bytes.Length, bytesEnc, 0);
            decryptor.TransformBlock(bytesEnc, 0, bytesEnc.Length, bytesDec, 0);
        }

        if (padding is PaddingMode.None) {  // has to be a full block if no padding
            var lastBytesEnc = encryptor.TransformFinalBlock(new byte[16], 0, 16);
            var lastBytesDec = decryptor.TransformFinalBlock(lastBytesEnc, 0, lastBytesEnc.Length);
        } else {
            var lastBytesEnc = encryptor.TransformFinalBlock(new byte[10], 0, 10);
            var lastBytesDec = decryptor.TransformFinalBlock(lastBytesEnc, 0, lastBytesEnc.Length);
        }
        sw.Stop();

        Debug.WriteLine($"Duration: {sw.ElapsedMilliseconds} ms");
    }


    #region Multiblock

    [TestMethod]
    public void Twofish_ECB_MultiBlock_128_Encrypt() {
        var key = ParseBytes("00000000000000000000000000000000");
        var pt = ParseBytes("000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
        using var algorithm = new Twofish() { KeySize = 128, Mode = CipherMode.ECB, Padding = PaddingMode.None };
        var ct = Encrypt(algorithm, key, null, pt);
        Assert.AreEqual("9F589F5CF6122C32B6BFEC2F2AE8C35A9F589F5CF6122C32B6BFEC2F2AE8C35A9F589F5CF6122C32B6BFEC2F2AE8C35A", BitConverter.ToString(ct).Replace("-", ""));
    }

    [TestMethod]
    public void Twofish_ECB_MultiBlock_128_Decrypt() {
        var key = ParseBytes("00000000000000000000000000000000");
        var ct = ParseBytes("9F589F5CF6122C32B6BFEC2F2AE8C35A9F589F5CF6122C32B6BFEC2F2AE8C35A9F589F5CF6122C32B6BFEC2F2AE8C35A");
        using var algorithm = new Twofish() { KeySize = 128, Mode = CipherMode.ECB, Padding = PaddingMode.None };
        var pt = Decrypt(algorithm, key, null, ct);
        Assert.AreEqual("000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", BitConverter.ToString(pt).Replace("-", ""));
    }


    [TestMethod]
    public void Twofish_ECB_MultiBlock_192_Encrypt() {
        var key = ParseBytes("000000000000000000000000000000000000000000000000");
        var pt = ParseBytes("000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
        var algorithm = new Twofish() { KeySize = 192, Mode = CipherMode.ECB, Padding = PaddingMode.None };
        var ct = Encrypt(algorithm, key, null, pt);
        Assert.AreEqual("EFA71F788965BD4453F860178FC19101EFA71F788965BD4453F860178FC19101EFA71F788965BD4453F860178FC19101", BitConverter.ToString(ct).Replace("-", ""));
    }

    [TestMethod]
    public void Twofish_ECB_MultiBlock_192_Decrypt() {
        var key = ParseBytes("000000000000000000000000000000000000000000000000");
        var ct = ParseBytes("EFA71F788965BD4453F860178FC19101EFA71F788965BD4453F860178FC19101EFA71F788965BD4453F860178FC19101");
        var algorithm = new Twofish() { KeySize = 192, Mode = CipherMode.ECB, Padding = PaddingMode.None };
        var pt = Decrypt(algorithm, key, null, ct);
        Assert.AreEqual("000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", BitConverter.ToString(pt).Replace("-", ""));
    }


    [TestMethod]
    public void Twofish_ECB_MultiBlock_256_Encrypt() {
        var key = ParseBytes("0000000000000000000000000000000000000000000000000000000000000000");
        var pt = ParseBytes("000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
        var algorithm = new Twofish() { KeySize = 256, Mode = CipherMode.ECB, Padding = PaddingMode.None };
        var ct = Encrypt(algorithm, key, null, pt);
        Assert.AreEqual("57FF739D4DC92C1BD7FC01700CC8216F57FF739D4DC92C1BD7FC01700CC8216F57FF739D4DC92C1BD7FC01700CC8216F", BitConverter.ToString(ct).Replace("-", ""));
    }

    [TestMethod]
    public void Twofish_ECB_MultiBlock_256_Decrypt() {
        var key = ParseBytes("0000000000000000000000000000000000000000000000000000000000000000");
        var ct = ParseBytes("57FF739D4DC92C1BD7FC01700CC8216F57FF739D4DC92C1BD7FC01700CC8216F57FF739D4DC92C1BD7FC01700CC8216F");
        var algorithm = new Twofish() { KeySize = 256, Mode = CipherMode.ECB, Padding = PaddingMode.None };
        var pt = Decrypt(algorithm, key, null, ct);
        Assert.AreEqual("000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", BitConverter.ToString(pt).Replace("-", ""));
    }


    [TestMethod]
    public void Twofish_ECB_MultiBlockNonFinal_256_Encrypt() {
        var key = ParseBytes("0000000000000000000000000000000000000000000000000000000000000000");
        var pt = ParseBytes("000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
        using var algorithm = new Twofish() { KeySize = 256, Mode = CipherMode.ECB, Padding = PaddingMode.None };
        algorithm.Key = key;
        var ct = new byte[pt.Length];
        using (var transform = algorithm.CreateEncryptor()) {
            transform.TransformBlock(pt, 0, pt.Length, ct, 0);
            transform.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
        }
        Assert.AreEqual("57FF739D4DC92C1BD7FC01700CC8216F57FF739D4DC92C1BD7FC01700CC8216F57FF739D4DC92C1BD7FC01700CC8216F", BitConverter.ToString(ct).Replace("-", ""));
    }

    [TestMethod]
    public void Twofish_ECB_MultiBlockNotFinal_256_Decrypt() {
        var key = ParseBytes("0000000000000000000000000000000000000000000000000000000000000000");
        var ct = ParseBytes("57FF739D4DC92C1BD7FC01700CC8216F57FF739D4DC92C1BD7FC01700CC8216F57FF739D4DC92C1BD7FC01700CC8216F");
        using var algorithm = new Twofish() { KeySize = 256, Mode = CipherMode.ECB, Padding = PaddingMode.None };
        algorithm.Key = key;
        var pt = new byte[ct.Length];
        using (var transform = algorithm.CreateDecryptor()) {
            transform.TransformBlock(ct, 0, ct.Length, pt, 0);
            transform.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
        }
        Assert.AreEqual("000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", BitConverter.ToString(pt).Replace("-", ""));
    }


    [TestMethod]
    public void Twofish_ECB_MultiBlockFinal_256_Encrypt() {
        var key = ParseBytes("0000000000000000000000000000000000000000000000000000000000000000");
        var pt = ParseBytes("000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
        using var algorithm = new Twofish() { KeySize = 256, Mode = CipherMode.ECB, Padding = PaddingMode.None };
        algorithm.Key = key;
        var ct = algorithm.CreateEncryptor().TransformFinalBlock(pt, 0, pt.Length);
        Assert.AreEqual("57FF739D4DC92C1BD7FC01700CC8216F57FF739D4DC92C1BD7FC01700CC8216F57FF739D4DC92C1BD7FC01700CC8216F", BitConverter.ToString(ct).Replace("-", ""));
    }

    [TestMethod]
    public void Twofish_ECB_MultiBlockFinal_256_Decrypt() {
        var key = ParseBytes("0000000000000000000000000000000000000000000000000000000000000000");
        var ct = ParseBytes("57FF739D4DC92C1BD7FC01700CC8216F57FF739D4DC92C1BD7FC01700CC8216F57FF739D4DC92C1BD7FC01700CC8216F");
        using var algorithm = new Twofish() { KeySize = 256, Mode = CipherMode.ECB, Padding = PaddingMode.None };
        algorithm.Key = key;
        var pt = algorithm.CreateDecryptor().TransformFinalBlock(ct, 0, ct.Length);
        Assert.AreEqual("000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", BitConverter.ToString(pt).Replace("-", ""));
    }

    #endregion

    #region Other

    [TestMethod]
    public void Twofish_ECB_TransformBlock_Encrypt_UseSameArray() {
        var key = ParseBytes("00000000000000000000000000000000");
        var iv = ParseBytes("00000000000000000000000000000000");
        var ctpt = Encoding.UTF8.GetBytes("The quick brown fox jumps over the lazy dog once");
        using (var twofish = new Twofish() { Mode = CipherMode.ECB, Padding = PaddingMode.None, KeySize = 128, Key = key, IV = iv }) {
            using var transform = twofish.CreateEncryptor();
            transform.TransformBlock(ctpt, 0, 48, ctpt, 0);
        }
        Assert.AreEqual("B0DD30E9AB1F1329C1BEE154DDBE88AF1194B36D8E0BDD5AC10842B549230BB36D66FC3AFE1F40216590079AF862AB59", BitConverter.ToString(ctpt).Replace("-", ""));
    }

    [TestMethod]
    public void Twofish_ECB_TransformBlock_Decrypt_UseSameArray() {
        var key = ParseBytes("00000000000000000000000000000000");
        var iv = ParseBytes("00000000000000000000000000000000");
        var ctpt = ParseBytes("B0DD30E9AB1F1329C1BEE154DDBE88AF1194B36D8E0BDD5AC10842B549230BB36D66FC3AFE1F40216590079AF862AB59");
        using (var twofish = new Twofish() { Mode = CipherMode.ECB, Padding = PaddingMode.None, KeySize = 128, Key = key, IV = iv }) {
            using var transform = twofish.CreateDecryptor();
            transform.TransformBlock(ctpt, 0, 48, ctpt, 0); //no caching last block if Padding is none
        }
        Assert.AreEqual("The quick brown fox jumps over the lazy dog once", Encoding.UTF8.GetString(ctpt));
    }

    #endregion



    #region Private: Monte carlo
    // http://www.ntua.gr/cryptix/old/cryptix/aes/docs/katmct.html

    private static void MonteCarlo_ECB_E(TestBlock test) {
        using var algorithm = new Twofish() { KeySize = test.KeySize, Mode = CipherMode.ECB, Padding = PaddingMode.None };
        var key = test.Key;
        var pt = test.PlainText;
        byte[] ct = null;
        for (var j = 0; j < 10000; j++) {
            ct = Encrypt(algorithm, key, null, pt);
            pt = ct;
        }
        Assert.AreEqual(BitConverter.ToString(test.CipherText), BitConverter.ToString(ct));
    }

    private static void MonteCarlo_ECB_D(TestBlock test) {
        using var algorithm = new Twofish() { KeySize = test.KeySize, Mode = CipherMode.ECB, Padding = PaddingMode.None };
        var key = test.Key;
        var ct = test.CipherText;
        byte[] pt = null;
        for (var j = 0; j < 10000; j++) {
            pt = Decrypt(algorithm, key, null, ct);
            ct = pt;
        }
        Assert.AreEqual(BitConverter.ToString(test.PlainText), BitConverter.ToString(pt));
    }

    #endregion

}
