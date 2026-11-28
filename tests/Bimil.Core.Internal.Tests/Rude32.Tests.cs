namespace Tests;

using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using Bimil;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class Rude32Tests {

    [TestMethod]
    [DataRow(new byte[] { }, "")]
    [DataRow(new byte[] { 0 }, "00")]
    [DataRow(new byte[] { 0, 0 }, "0000")]
    [DataRow(new byte[] { 0, 0, 0 }, "00000")]
    [DataRow(new byte[] { 0, 0, 0, 0 }, "0000000")]
    [DataRow(new byte[] { 0, 0, 0, 0, 0 }, "00000000")]
    [DataRow(new byte[] { 0b0000_1000 }, "10")]
    [DataRow(new byte[] { 0b0000_1000, 0b0100_0010 }, "1110")]
    [DataRow(new byte[] { 0b0000_1000, 0b0100_0010, 0b0001_0000 }, "11110")]
    [DataRow(new byte[] { 0b0000_1000, 0b0100_0010, 0b0001_0000, 0b1000_0100 }, "1111110")]
    [DataRow(new byte[] { 0b0000_1000, 0b0100_0010, 0b0001_0000, 0b1000_0100, 0b0010_0001 }, "11111111")]
    [DataRow(new byte[] { 0b0001_1000 }, "30")]
    [DataRow(new byte[] { 0b0001_1000, 0b1100_0110 }, "3330")]
    [DataRow(new byte[] { 0b0001_1000, 0b1100_0110, 0b0011_0000 }, "33330")]
    [DataRow(new byte[] { 0b0001_1000, 0b1100_0110, 0b0011_0001, 0b1000_1100 }, "3333330")]
    [DataRow(new byte[] { 0b0001_1000, 0b1100_0110, 0b0011_0001, 0b1000_1100, 0b0110_0011 }, "33333333")]
    [DataRow(new byte[] { 0b0011_1000 }, "70")]
    [DataRow(new byte[] { 0b0011_1001, 0b1100_1110 }, "7770")]
    [DataRow(new byte[] { 0b0011_1001, 0b1100_1110, 0b0111_0000 }, "77770")]
    [DataRow(new byte[] { 0b0011_1001, 0b1100_1110, 0b0111_0011, 0b1001_1100 }, "7777770")]
    [DataRow(new byte[] { 0b0011_1001, 0b1100_1110, 0b0111_0011, 0b1001_1100, 0b1110_0111 }, "77777777")]
    [DataRow(new byte[] { 0b0111_1000 }, "F0")]
    [DataRow(new byte[] { 0b0111_1011, 0b1101_1110 }, "FFF0")]
    [DataRow(new byte[] { 0b0111_1011, 0b1101_1110, 0b1111_0000 }, "FFFF0")]
    [DataRow(new byte[] { 0b0111_1011, 0b1101_1110, 0b1111_0111, 0b1011_1100 }, "FFFFFF0")]
    [DataRow(new byte[] { 0b0111_1011, 0b1101_1110, 0b1111_0111, 0b1011_1101, 0b1110_1111 }, "FFFFFFFF")]
    [DataRow(new byte[] { 0b1010_1010 }, "N8")]
    [DataRow(new byte[] { 0b1010_1010, 0b1010_1010 }, "NAN0")]
    [DataRow(new byte[] { 0b1010_1010, 0b1010_1010, 0b1010_1010 }, "NANAM")]
    [DataRow(new byte[] { 0b1010_1010, 0b1010_1010, 0b1010_1010, 0b1010_1010 }, "NANANAG")]
    [DataRow(new byte[] { 0b1010_1010, 0b1010_1010, 0b1010_1010, 0b1010_1010, 0b1010_1010 }, "NANANANA")]
    [DataRow(new byte[] { 0b1111_1000 }, "Z0")]
    [DataRow(new byte[] { 0b1111_1111, 0b1111_1110 }, "ZZZ0")]
    [DataRow(new byte[] { 0b1111_1111, 0b1111_1111, 0b1111_0000 }, "ZZZZ0")]
    [DataRow(new byte[] { 0b1111_1111, 0b1111_1111, 0b1111_1111, 0b1111_1100 }, "ZZZZZZ0")]
    [DataRow(new byte[] { 0b1111_1111, 0b1111_1111, 0b1111_1111, 0b1111_1111, 0b1111_1111 }, "ZZZZZZZZ")]
    [DataRow(new byte[] { 0b0000_0000, 0b0100_0100, 0b0011_0010, 0b0001_0100, 0b1100_0111 }, "01234567")]
    [DataRow(new byte[] { 0b0100_0010, 0b0101_0100, 0b1011_0110, 0b0011_0101, 0b1100_1111 }, "89ABCDEF")]
    [DataRow(new byte[] { 0b1000_0100, 0b0110_0101, 0b0011_1010, 0b0101_0110, 0b1101_0111 }, "GHJKMNPQ")]
    [DataRow(new byte[] { 0b1100_0110, 0b0111_0101, 0b1011_1110, 0b0111_0111, 0b1101_1111 }, "RTUVWXYZ")]
    public void Rude32_ToString(byte[] bytes, string text) {
        Assert.AreEqual(text, Rude32.ToString(bytes));
        {
            Assert.IsTrue(Rude32.TryToString(bytes, out var actual));
            Assert.AreEqual(text, actual);
        }
        {
            var actual = new char[text.Length];
            Assert.IsTrue(Rude32.TryToString(bytes, actual));
            Assert.AreSequenceEqual(actual, [.. text]);
        }
        {
            var actual = new char[text.Length];
            Assert.IsTrue(Rude32.TryToString(bytes, actual, out var charsWritten));
            Assert.AreEqual(actual.Length, charsWritten);
            Assert.AreSequenceEqual(actual, [.. text]);
        }

        Assert.AreSequenceEqual(bytes, Rude32.FromString(text));
        {
            Assert.IsTrue(Rude32.TryFromString(text, out var actual));
            Assert.AreSequenceEqual(bytes, actual);
        }
        {
            var actual = new byte[bytes.Length];
            Assert.IsTrue(Rude32.TryFromString(text, actual));
            Assert.AreSequenceEqual(actual, bytes);
        }
        {
            var actual = new byte[bytes.Length];
            Assert.IsTrue(Rude32.TryFromString(text, actual, out var bytesWritten));
            Assert.AreEqual(actual.Length, bytesWritten);
            Assert.AreSequenceEqual(actual, bytes);
        }
        {
            var actual = new byte[bytes.Length];
            Assert.IsTrue(Rude32.TryFromString(Encoding.ASCII.GetBytes(text), actual, out var bytesWritten));
            Assert.AreEqual(actual.Length, bytesWritten);
            Assert.AreSequenceEqual(actual, bytes);
        }
    }

    [TestMethod]
    [DataRow(new byte[] { }, "")]
    [DataRow(new byte[] { 0 }, "02")]
    [DataRow(new byte[] { 0, 0 }, "0003")]
    [DataRow(new byte[] { 0, 0, 0 }, "00000")]
    [DataRow(new byte[] { 0, 0, 0, 0 }, "0000005")]
    [DataRow(new byte[] { 0, 0, 0, 0, 0 }, "00000000")]
    [DataRow(new byte[] { 0b0000_1000 }, "11")]
    [DataRow(new byte[] { 0b0000_1000, 0b0100_0010 }, "111A")]
    [DataRow(new byte[] { 0b0000_1000, 0b0100_0010, 0b0001_0000 }, "11110")]
    [DataRow(new byte[] { 0b0000_1000, 0b0100_0010, 0b0001_0000, 0b1000_0100 }, "1111117")]
    [DataRow(new byte[] { 0b0000_1000, 0b0100_0010, 0b0001_0000, 0b1000_0100, 0b0010_0001 }, "11111111")]
    [DataRow(new byte[] { 0b0001_1000 }, "32")]
    [DataRow(new byte[] { 0b0001_1000, 0b1100_0110 }, "3339")]
    [DataRow(new byte[] { 0b0001_1000, 0b1100_0110, 0b0011_0000 }, "33330")]
    [DataRow(new byte[] { 0b0001_1000, 0b1100_0110, 0b0011_0001, 0b1000_1100 }, "3333332")]
    [DataRow(new byte[] { 0b0001_1000, 0b1100_0110, 0b0011_0001, 0b1000_1100, 0b0110_0011 }, "33333333")]
    [DataRow(new byte[] { 0b0011_1001 }, "76")]
    [DataRow(new byte[] { 0b0011_1001, 0b1100_1111 }, "777T")]
    [DataRow(new byte[] { 0b0011_1001, 0b1100_1110, 0b0111_0011 }, "77776")]
    [DataRow(new byte[] { 0b0011_1001, 0b1100_1110, 0b0111_0011, 0b1001_1100 }, "7777775")]
    [DataRow(new byte[] { 0b0011_1001, 0b1100_1110, 0b0111_0011, 0b1001_1100, 0b1110_0111 }, "77777777")]
    [DataRow(new byte[] { 0b0111_1000 }, "F2")]
    [DataRow(new byte[] { 0b0111_1011, 0b1101_1110 }, "FFF6")]
    [DataRow(new byte[] { 0b0111_1011, 0b1101_1110, 0b1111_0000 }, "FFFF0")]
    [DataRow(new byte[] { 0b0111_1011, 0b1101_1110, 0b1111_0111, 0b1011_1100 }, "FFFFFF4")]
    [DataRow(new byte[] { 0b0111_1011, 0b1101_1110, 0b1111_0111, 0b1011_1101, 0b1110_1111 }, "FFFFFFFF")]
    [DataRow(new byte[] { 0b1010_1010 }, "N9")]
    [DataRow(new byte[] { 0b1010_1010, 0b1010_1010 }, "NAN3")]
    [DataRow(new byte[] { 0b1010_1010, 0b1010_1010, 0b1010_1010 }, "NANAM")]
    [DataRow(new byte[] { 0b1010_1010, 0b1010_1010, 0b1010_1010, 0b1010_1010 }, "NANANAM")]
    [DataRow(new byte[] { 0b1010_1010, 0b1010_1010, 0b1010_1010, 0b1010_1010, 0b1010_1010 }, "NANANANA")]
    [DataRow(new byte[] { 0b1111_1000 }, "Z1")]
    [DataRow(new byte[] { 0b1111_1111, 0b1111_1110 }, "ZZZ2")]
    [DataRow(new byte[] { 0b1111_1111, 0b1111_1111, 0b1111_0000 }, "ZZZZ0")]
    [DataRow(new byte[] { 0b1111_1111, 0b1111_1111, 0b1111_1111, 0b1111_1100 }, "ZZZZZZ4")]
    [DataRow(new byte[] { 0b1111_1111, 0b1111_1111, 0b1111_1111, 0b1111_1111, 0b1111_1111 }, "ZZZZZZZZ")]
    public void Rude32_ToString_WithChecksum(byte[] bytes, string text) {
        Assert.AreEqual(text, Rude32.ToString(bytes, includeChecksumInPadding: true));
        {
            Assert.IsTrue(Rude32.TryToString(bytes, out var actual, includeChecksumInPadding: true));
            Assert.AreEqual(text, actual);
        }
        {
            var chars = new char[text.Length];
            Assert.IsTrue(Rude32.TryToString(bytes, chars, includeChecksumInPadding: true));
            Assert.AreSequenceEqual(chars, [.. text]);
        }
        {
            var chars = new char[text.Length];
            Assert.IsTrue(Rude32.TryToString(bytes, chars, out var charsWritten, includeChecksumInPadding: true));
            Assert.AreEqual(chars.Length, charsWritten);
            Assert.AreSequenceEqual(chars, [.. text]);
        }

        Assert.AreSequenceEqual(bytes, Rude32.FromString(text));
        {
            Assert.IsTrue(Rude32.TryFromString(text, out var actual));
            Assert.AreSequenceEqual(bytes, actual);
        }
        {
            var actual = new byte[bytes.Length];
            Assert.IsTrue(Rude32.TryFromString(text, actual));
            Assert.AreSequenceEqual(actual, bytes);
        }
        {
            var actual = new byte[bytes.Length];
            Assert.IsTrue(Rude32.TryFromString(text, actual, out var bytesWritten));
            Assert.AreEqual(actual.Length, bytesWritten);
            Assert.AreSequenceEqual(actual, bytes);
        }
        {
            var actual = new byte[bytes.Length];
            Assert.IsTrue(Rude32.TryFromString(Encoding.ASCII.GetBytes(text), actual, out var bytesWritten));
            Assert.AreEqual(actual.Length, bytesWritten);
            Assert.AreSequenceEqual(actual, bytes);
        }
    }

    [TestMethod]
    public void Rude32_ToString_Incomplete_1() {
        var bytes = new byte[] { 0 };
        var chars = new char[1];
        Assert.IsFalse(Rude32.TryToString(bytes, chars, out var charsWritten));
        Assert.AreEqual("0", new string(chars));
        Assert.AreEqual(1, charsWritten);
    }

    [TestMethod]
    public void Rude32_ToString_Incomplete_2() {
        var bytes = new byte[] { 0, 0 };
        var chars = new char[3];
        Assert.IsFalse(Rude32.TryToString(bytes, chars, out var charsWritten));
        Assert.AreEqual("000", new string(chars));
        Assert.AreEqual(3, charsWritten);
    }

}
