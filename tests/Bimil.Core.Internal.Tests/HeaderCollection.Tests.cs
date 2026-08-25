namespace Tests;

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bimil;

[TestClass]
public class HeaderCollectionTests {

    [TestMethod]
    public void HeaderCollection_New_V3() {
        var headers = new HeaderCollection(DatabaseVersion.V3, []);
        Assert.AreEqual(1, headers.Count);
        Assert.AreEqual(new Version(3, 0, 0, 0), ((VersionHeader)headers[0]).Version);
    }

    [TestMethod]
    public void HeaderCollection_New_V4() {
        var headers = new HeaderCollection(DatabaseVersion.V4, []);
        Assert.AreEqual(1, headers.Count);
        Assert.AreEqual(new Version(4, 0, 0, 0), ((VersionHeader)headers[0]).Version);
    }

    [TestMethod]
    public void HeaderCollection_FixVersionOrder() {
        var headers = new HeaderCollection(DatabaseVersion.V4, [
            Header.Create(HeaderType.Uuid, UuidHeader.GetBytes(Guid.AllBitsSet)),
            Header.Create(HeaderType.Version, VersionHeader.GetBytes(new Version(4, 0, 0, 0))),
        ]);
        Assert.AreEqual(2, headers.Count);
        Assert.AreEqual(new Version(4, 0, 0, 0), ((VersionHeader)headers[0]).Version);
        Assert.AreEqual(Guid.AllBitsSet, ((UuidHeader)headers[1]).Uuid);
    }

    [TestMethod]
    public void HeaderCollection_ClearDoesntRemoveVersion() {
        var headers = new HeaderCollection(DatabaseVersion.V4, [
            Header.Create(HeaderType.Uuid, UuidHeader.GetBytes(Guid.AllBitsSet)),
            Header.Create(HeaderType.Version, VersionHeader.GetBytes(new Version(4, 1, 0, 0))),
        ]);
        headers.Clear();
        Assert.AreEqual(1, headers.Count);
        Assert.AreEqual(new Version(4, 1, 0, 0), ((VersionHeader)headers[0]).Version);
    }


    [TestMethod]
    public void HeaderCollection_New_Unknown() {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => {
            var headers = new HeaderCollection((DatabaseVersion)5, []);
        });
    }

    [TestMethod]
    public void HeaderCollection_CannotUseWrongMainVersion() {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => {
            var headers = new HeaderCollection(DatabaseVersion.V4, [
                Header.Create(HeaderType.Uuid, UuidHeader.GetBytes(Guid.AllBitsSet)),
                Header.Create(HeaderType.Version, VersionHeader.GetBytes(new Version(3, 0, 0, 0))),
            ]);
        });
    }

    [TestMethod]
    public void HeaderCollection_CannotRemoveVersion() {
        var version = Header.Create(HeaderType.Version, VersionHeader.GetBytes(new Version(4, 0, 0, 0)));
        var headers = new HeaderCollection(DatabaseVersion.V4, [
            version,
            Header.Create(HeaderType.Uuid, UuidHeader.GetBytes(Guid.AllBitsSet)),
        ]);
        Assert.ThrowsException<InvalidOperationException>(() =>
            headers.RemoveAt(0)
        );
        Assert.ThrowsException<InvalidOperationException>(() =>
            headers.Remove(version)
        );
        Assert.ThrowsException<InvalidOperationException>(() =>
            headers.Remove(HeaderType.Version)
        );
    }

}
