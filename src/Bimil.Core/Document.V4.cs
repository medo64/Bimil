namespace Bimil;

using System;
using System.Buffers.Binary;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public sealed partial class Document {

    private static Document LoadCoreV4(Span<byte> bytes, byte[] passphrase) {
        var doc = new Document(DatabaseVersion.V4);
        throw new NotImplementedException();
    }

    private void SaveCoreV4(Stream stream, byte[] passphrase) {
        throw new NotImplementedException();
    }

}
