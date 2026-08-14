namespace Tests;

using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bimil;

public partial class TwofishTests {

    [DataTestMethod]
    [DataRow(PaddingMode.None)]
    [DataRow(PaddingMode.PKCS7)]
    [DataRow(PaddingMode.Zeros)]
    [DataRow(PaddingMode.ANSIX923)]
    [DataRow(PaddingMode.ISO10126)]
    public void Twofish_Padding_Full(PaddingMode padding) {
        var key = new byte[32]; RandomNumberGenerator.Fill(key);
        var iv = new byte[16]; RandomNumberGenerator.Fill(iv);
        var data = new byte[48]; RandomNumberGenerator.Fill(data);  // full blocks

        var algorithm = new Twofish() { Padding = padding, };

        var ct = Encrypt(algorithm, key, iv, data);
        var pt = Decrypt(algorithm, key, iv, ct);
        Assert.AreEqual(data.Length, pt.Length);
        Assert.AreEqual(BitConverter.ToString(data), BitConverter.ToString(pt));
    }

    [DataTestMethod]
    [DataRow(PaddingMode.PKCS7)]
    [DataRow(PaddingMode.Zeros)]
    [DataRow(PaddingMode.ANSIX923)]
    [DataRow(PaddingMode.ISO10126)]
    public void Twofish_Padding_Partial(PaddingMode padding) {
        var key = new byte[32]; RandomNumberGenerator.Fill(key);
        var iv = new byte[16]; RandomNumberGenerator.Fill(iv);
        var data = new byte[42]; RandomNumberGenerator.Fill(data);

        var algorithm = new Twofish() { Padding = padding };

        var ct = Encrypt(algorithm, key, iv, data);
        var pt = Decrypt(algorithm, key, iv, ct);
        Assert.AreEqual(data.Length, pt.Length);
        Assert.AreEqual(BitConverter.ToString(data), BitConverter.ToString(pt));
    }



    [DataTestMethod]
    [DataRow(PaddingMode.None)]
    [DataRow(PaddingMode.PKCS7)]
    [DataRow(PaddingMode.Zeros)]
    [DataRow(PaddingMode.ANSIX923)]
    [DataRow(PaddingMode.ISO10126)]
    public void Twofish_Padding_LargeFinalBlock(PaddingMode padding) {
        var crypto = new Twofish() { Padding = padding };
        crypto.GenerateKey();
        crypto.GenerateIV();
        var text = "This is a final block wider than block size.";  // more than 128 bits of data
        if (padding is PaddingMode.None) { text += "1234"; }  // must have a full block if no padding
        var bytes = Encoding.ASCII.GetBytes(text);

        using var encryptor = crypto.CreateEncryptor();
        var ct = encryptor.TransformFinalBlock(bytes, 0, bytes.Length);

        Assert.AreEqual(padding == PaddingMode.None ? bytes.Length : 48, ct.Length);

        using var decryptor = crypto.CreateDecryptor();
        var pt = decryptor.TransformFinalBlock(ct, 0, ct.Length);

        Assert.AreEqual(bytes.Length, pt.Length);
        Assert.AreEqual(text, Encoding.ASCII.GetString(pt));
    }

    [DataTestMethod]
    [DataRow(PaddingMode.None)]
    [DataRow(PaddingMode.PKCS7)]
    [DataRow(PaddingMode.Zeros)]
    [DataRow(PaddingMode.ANSIX923)]
    [DataRow(PaddingMode.ISO10126)]
    public void Twofish_Padding_BlockSizeRounding(PaddingMode padding) {
        var key = new byte[32]; RandomNumberGenerator.Fill(key);
        var iv = new byte[16]; RandomNumberGenerator.Fill(iv);

        for (int n = 0; n < 50; n++) {
            if ((n % 16 != 0) && (padding is PaddingMode.None)) { continue; }  // padding None works only on full blocks

            var data = new byte[n];
            RandomNumberGenerator.Fill(data);
            if ((padding == PaddingMode.Zeros) && (data.Length > 0)) { data[^1] = 1; }  // zero padding needs to have the last number non-zero

            var algorithm = new Twofish() { Padding = padding, };

            var expectedCryptLength = padding switch {
                PaddingMode.None => data.Length,
                PaddingMode.PKCS7 => ((data.Length / 16) + 1) * 16,
                PaddingMode.Zeros => (data.Length / 16 + (data.Length % 16 > 0 ? 1 : 0)) * 16,
                PaddingMode.ANSIX923 => ((data.Length / 16) + 1) * 16,
                PaddingMode.ISO10126 => ((data.Length / 16) + 1) * 16,
                _ => -1

            };
            var ct = Encrypt(algorithm, key, iv, data);
            Assert.AreEqual(expectedCryptLength, ct.Length);

            var pt = Decrypt(algorithm, key, iv, ct);
            Assert.AreEqual(data.Length, pt.Length);
            Assert.AreEqual(BitConverter.ToString(data), BitConverter.ToString(pt));
        }
    }


    [TestMethod]
    public void Twofish_Padding_Zeros_ECB_128_Encrypt() {
        var key = ParseBytes("00000000000000000000000000000000");
        var pt = Encoding.UTF8.GetBytes("The quick brown fox jumps over the lazy dog");
        using var algorithm = new Twofish() { KeySize = 128, Mode = CipherMode.ECB, Padding = PaddingMode.Zeros };
        var ct = Encrypt(algorithm, key, null, pt);
        Assert.AreEqual("B0DD30E9AB1F1329C1BEE154DDBE88AF1194B36D8E0BDD5AC10842B549230BB33C25C273BF09B94A31DE3C27C28DFB5C", BitConverter.ToString(ct).Replace("-", ""));
    }

    [TestMethod]
    public void Twofish_Padding_Zeros_ECB_128_Decrypt() {
        var key = ParseBytes("00000000000000000000000000000000");
        var ct = ParseBytes("B0DD30E9AB1F1329C1BEE154DDBE88AF1194B36D8E0BDD5AC10842B549230BB33C25C273BF09B94A31DE3C27C28DFB5C");
        using var algorithm = new Twofish() { KeySize = 128, Mode = CipherMode.ECB, Padding = PaddingMode.Zeros };
        var pt = Decrypt(algorithm, key, null, ct);
        Assert.AreEqual("The quick brown fox jumps over the lazy dog", Encoding.UTF8.GetString(pt));
    }

    [TestMethod]
    public void Twofish_Padding_None_ECB_128_Encrypt_16() {
        var key = ParseBytes("00000000000000000000000000000000");
        var pt = Encoding.UTF8.GetBytes("The quick brown fox jumps over the lazy dog once");
        using var algorithm = new Twofish() { KeySize = 128, Mode = CipherMode.ECB, Padding = PaddingMode.None };
        var ct = Encrypt(algorithm, key, null, pt);
        Assert.AreEqual("B0DD30E9AB1F1329C1BEE154DDBE88AF1194B36D8E0BDD5AC10842B549230BB36D66FC3AFE1F40216590079AF862AB59", BitConverter.ToString(ct).Replace("-", ""));
    }

    [TestMethod]
    public void Twofish_Padding_None_ECB_128_Decrypt_16() {
        var key = ParseBytes("00000000000000000000000000000000");
        var ct = ParseBytes("B0DD30E9AB1F1329C1BEE154DDBE88AF1194B36D8E0BDD5AC10842B549230BB36D66FC3AFE1F40216590079AF862AB59");
        using var algorithm = new Twofish() { KeySize = 128, Mode = CipherMode.ECB, Padding = PaddingMode.None };
        var pt = Decrypt(algorithm, key, null, ct);
        Assert.AreEqual("The quick brown fox jumps over the lazy dog once", Encoding.UTF8.GetString(pt));
    }


    [TestMethod]
    public void Twofish_Padding_Zeros_ECB_128_Encrypt_16() {
        var key = ParseBytes("00000000000000000000000000000000");
        var pt = Encoding.UTF8.GetBytes("The quick brown fox jumps over the lazy dog once");
        using var algorithm = new Twofish() { KeySize = 128, Mode = CipherMode.ECB, Padding = PaddingMode.Zeros };
        var ct = Encrypt(algorithm, key, null, pt);
        Assert.AreEqual("B0DD30E9AB1F1329C1BEE154DDBE88AF1194B36D8E0BDD5AC10842B549230BB36D66FC3AFE1F40216590079AF862AB59", BitConverter.ToString(ct).Replace("-", ""));
    }

    [TestMethod]
    public void Twofish_Padding_Zeros_ECB_128_Decrypt_16() {
        var key = ParseBytes("00000000000000000000000000000000");
        var ct = ParseBytes("B0DD30E9AB1F1329C1BEE154DDBE88AF1194B36D8E0BDD5AC10842B549230BB36D66FC3AFE1F40216590079AF862AB59");
        using var algorithm = new Twofish() { KeySize = 128, Mode = CipherMode.ECB, Padding = PaddingMode.Zeros };
        var pt = Decrypt(algorithm, key, null, ct);
        Assert.AreEqual("The quick brown fox jumps over the lazy dog once", Encoding.UTF8.GetString(pt));
    }


    [TestMethod]
    public void Twofish_Padding_Pkcs7_ECB_128_Encrypt() {
        var key = ParseBytes("00000000000000000000000000000000");
        var pt = Encoding.UTF8.GetBytes("The quick brown fox jumps over the lazy dog");
        using var algorithm = new Twofish() { KeySize = 128, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 };
        var ct = Encrypt(algorithm, key, null, pt);
        Assert.AreEqual("B0DD30E9AB1F1329C1BEE154DDBE88AF1194B36D8E0BDD5AC10842B549230BB3235D2E6063F32DE35B8A62A384FC587E", BitConverter.ToString(ct).Replace("-", ""));
    }

    [TestMethod]
    public void Twofish_Padding_Pkcs7_ECB_128_Decrypt() {
        var key = ParseBytes("00000000000000000000000000000000");
        var ct = ParseBytes("B0DD30E9AB1F1329C1BEE154DDBE88AF1194B36D8E0BDD5AC10842B549230BB3235D2E6063F32DE35B8A62A384FC587E");
        using var algorithm = new Twofish() { KeySize = 128, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 };
        var pt = Decrypt(algorithm, key, null, ct);
        Assert.AreEqual("The quick brown fox jumps over the lazy dog", Encoding.UTF8.GetString(pt));
    }

    [TestMethod]
    public void Twofish_Padding_Pkcs7_ECB_128_Encrypt_16() {
        var key = ParseBytes("00000000000000000000000000000000");
        var pt = Encoding.UTF8.GetBytes("The quick brown fox jumps over the lazy dog once");
        using var algorithm = new Twofish() { KeySize = 128, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 };
        var ct = Encrypt(algorithm, key, null, pt);
        Assert.AreEqual("B0DD30E9AB1F1329C1BEE154DDBE88AF1194B36D8E0BDD5AC10842B549230BB36D66FC3AFE1F40216590079AF862AB59771D591428AF301D69FA1E227D083527", BitConverter.ToString(ct).Replace("-", ""));
    }

    [TestMethod]
    public void Twofish_Padding_Pkcs7_ECB_128_Decrypt_16() {
        var key = ParseBytes("00000000000000000000000000000000");
        var ct = ParseBytes("B0DD30E9AB1F1329C1BEE154DDBE88AF1194B36D8E0BDD5AC10842B549230BB36D66FC3AFE1F40216590079AF862AB59771D591428AF301D69FA1E227D083527");
        using var algorithm = new Twofish() { KeySize = 128, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 };
        var pt = Decrypt(algorithm, key, null, ct);
        Assert.AreEqual("The quick brown fox jumps over the lazy dog once", Encoding.UTF8.GetString(pt));
    }


    [TestMethod]
    public void Twofish_Padding_AnsiX923_ECB_128_Encrypt() {
        var key = ParseBytes("00000000000000000000000000000000");
        var pt = Encoding.UTF8.GetBytes("The quick brown fox jumps over the lazy dog");
        using var algorithm = new Twofish() { KeySize = 128, Mode = CipherMode.ECB, Padding = PaddingMode.ANSIX923 };
        var ct = Encrypt(algorithm, key, null, pt);
        Assert.AreEqual("B0DD30E9AB1F1329C1BEE154DDBE88AF1194B36D8E0BDD5AC10842B549230BB3B696D40A5E12225D3E05E8A466F078C2", BitConverter.ToString(ct).Replace("-", ""));
    }

    [TestMethod]
    public void Twofish_Padding_AnsiX923_ECB_128_Decrypt() {
        var key = ParseBytes("00000000000000000000000000000000");
        var ct = ParseBytes("B0DD30E9AB1F1329C1BEE154DDBE88AF1194B36D8E0BDD5AC10842B549230BB3B696D40A5E12225D3E05E8A466F078C2");
        using var algorithm = new Twofish() { KeySize = 128, Mode = CipherMode.ECB, Padding = PaddingMode.ANSIX923 };
        var pt = Decrypt(algorithm, key, null, ct);
        Assert.AreEqual("The quick brown fox jumps over the lazy dog", Encoding.UTF8.GetString(pt));
    }

    [TestMethod]
    public void Twofish_Padding_AnsiX923_ECB_128_Encrypt_16() {
        var key = ParseBytes("00000000000000000000000000000000");
        var pt = Encoding.UTF8.GetBytes("The quick brown fox jumps over the lazy dog once");
        using var algorithm = new Twofish() { KeySize = 128, Mode = CipherMode.ECB, Padding = PaddingMode.ANSIX923 };
        var ct = Encrypt(algorithm, key, null, pt);
        Assert.AreEqual("B0DD30E9AB1F1329C1BEE154DDBE88AF1194B36D8E0BDD5AC10842B549230BB36D66FC3AFE1F40216590079AF862AB5958A06DC5AD2D7C0550771D6E9D59D58B", BitConverter.ToString(ct).Replace("-", ""));
    }

    [TestMethod]
    public void Twofish_Padding_AnsiX923_ECB_128_Decrypt_16() {
        var key = ParseBytes("00000000000000000000000000000000");
        var ct = ParseBytes("B0DD30E9AB1F1329C1BEE154DDBE88AF1194B36D8E0BDD5AC10842B549230BB36D66FC3AFE1F40216590079AF862AB5958A06DC5AD2D7C0550771D6E9D59D58B");
        using var algorithm = new Twofish() { KeySize = 128, Mode = CipherMode.ECB, Padding = PaddingMode.ANSIX923 };
        var pt = Decrypt(algorithm, key, null, ct);
        Assert.AreEqual("The quick brown fox jumps over the lazy dog once", Encoding.UTF8.GetString(pt));
    }


    [TestMethod]
    public void Twofish_Padding_Iso10126_ECB_128_DecryptAndEncrypt() {
        var key = ParseBytes("00000000000000000000000000000000");
        var pt = "The quick brown fox jumps over the lazy dog";

        var ctA = ParseBytes("B0DD30E9AB1F1329C1BEE154DDBE88AF1194B36D8E0BDD5AC10842B549230BB3B696D40A5E12225D3E05E8A466F078C2");
        using (var algorithm = new Twofish() { KeySize = 128, Mode = CipherMode.ECB, Padding = PaddingMode.ISO10126 }) {
            var ptA = Decrypt(algorithm, key, null, ctA);
            Assert.AreEqual(pt, Encoding.UTF8.GetString(ptA));
        }

        var ptB = Encoding.UTF8.GetBytes(pt);
        using (var algorithm = new Twofish() { KeySize = 128, Mode = CipherMode.ECB, Padding = PaddingMode.ISO10126 }) {
            var ctB = Encrypt(algorithm, key, null, ptB);
            var ptC = Decrypt(algorithm, key, null, ctB);
            Assert.AreEqual(pt, Encoding.UTF8.GetString(ptC));
            Assert.AreNotEqual(BitConverter.ToString(ctA).Replace("-", ""), BitConverter.ToString(ctB).Replace("-", "")); //chances are good padding will be different (due to randomness involved)
        }
    }

    [TestMethod]
    public void Twofish_Padding_Iso10126_ECB_128_DecryptAndEncrypt_16() {
        var key = ParseBytes("00000000000000000000000000000000");
        var pt = "The quick brown fox jumps over the lazy dog once";

        var ctA = ParseBytes("B0DD30E9AB1F1329C1BEE154DDBE88AF1194B36D8E0BDD5AC10842B549230BB36D66FC3AFE1F40216590079AF862AB5958A06DC5AD2D7C0550771D6E9D59D58B");
        using (var algorithm = new Twofish() { KeySize = 128, Mode = CipherMode.ECB, Padding = PaddingMode.ISO10126 }) {
            var ptA = Decrypt(algorithm, key, null, ctA);
            Assert.AreEqual(pt, Encoding.UTF8.GetString(ptA));
        }

        var ptB = Encoding.UTF8.GetBytes(pt);
        using (var algorithm = new Twofish() { KeySize = 128, Mode = CipherMode.ECB, Padding = PaddingMode.ISO10126 }) {
            var ctB = Encrypt(algorithm, key, null, ptB);
            var ptC = Decrypt(algorithm, key, null, ctB);
            Assert.AreEqual(pt, Encoding.UTF8.GetString(ptC));
            Assert.AreNotEqual(BitConverter.ToString(ctA).Replace("-", ""), BitConverter.ToString(ctB).Replace("-", "")); //chances are good padding will be different (due to randomness involved)
        }
    }

}
