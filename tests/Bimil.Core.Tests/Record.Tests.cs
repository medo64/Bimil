namespace Tests;

using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bimil;

[TestClass]
public class RecordTests {

    #region Export/Import

    [TestMethod]
    public void Entry_Export() {
        var entry = EntryRecord.Create();
        entry.Fields.Add(TextField.Create(FieldType.Title));
        entry.Fields.Add(TextField.Create(FieldType.Password));

        var exported = entry.ExportToJson();
        var exported2 = exported.Replace(entry.Uuid.ToString(), "01234567-89ab-cdef-0123-456789abcdef");
        Assert.AreEqual("""{"kind":"Bimil.Record","fields":[{"type":"Uuid","uuid":"01234567-89ab-cdef-0123-456789abcdef"},{"type":"Title","text":""},{"type":"Password","text":""}]}""", exported2);

        var imported = (EntryRecord)Record.ImportFromJson(exported);
        Assert.AreEqual(3, entry.Fields.Count);
        Assert.AreEqual(entry.Uuid, imported.Uuid);
        Assert.AreEqual(entry.Title, imported.Title);
        Assert.AreEqual(((TextField)(entry.Fields[FieldType.Password])).Text, ((TextField)(imported.Fields[FieldType.Password])).Text);
    }

    // [TestMethod]
    // public void Entry_ExportImport_AllTypes() {
    //     var entry = new Entry();
    //     entry.Uuid = Guid.Parse("01234567-89ab-cdef-0123-456789abcdef");
    //     entry.Group = "Group";
    //     entry.Title = "Title";
    //     entry.UserName = "UserName";
    //     entry.Notes = "Notes";
    //     entry.Password = "Password";
    //     entry.CreationTime = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    //     entry.PasswordModificationTime = new DateTime(2002, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    //     entry.LastAccessTime = new DateTime(2003, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    //     entry.PasswordExpiryTime = new DateTime(2004, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    //     entry.LastModificationTime = new DateTime(2005, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    //     entry.Url = "http://example.com";
    //     entry.Email = "example@example.com";
    //     entry.SetTwoFactorKey([0, 1, 2, 3, 4, 5, 6, 7, 8, 9]);
    //     entry.CreditCardNumber = "1234 5678 9012 3456";
    //     entry.CreditCardExpiration = "Title";
    //     entry.CreditCardVerificationValue = "0987";
    //     entry.CreditCardPin = "6543";
    //     entry.QRCode = "https://medo64.com/";

    //     var exported = entry.ExportToJson();
    //     Assert.AreEqual("""{"kind":"Bimil.Record","fields":[{"type":"Uuid","uuid":"01234567-89ab-cdef-0123-456789abcdef"},{"type":"Group","text":"Group"},{"type":"Title","text":"Title"},{"type":"UserName","text":"UserName"},{"type":"Notes","text":"Notes"},{"type":"Password","text":"Password"},{"type":"CreationTime","time":"2001-01-01T00:00:00.0000000Z"},{"type":"PasswordModificationTime","time":"2002-01-01T00:00:00.0000000Z"},{"type":"LastAccessTime","time":"2003-01-01T00:00:00.0000000Z"},{"type":"PasswordExpiryTime","time":"2004-01-01T00:00:00.0000000Z"},{"type":"LastModificationTime","time":"2005-01-01T00:00:00.0000000Z"},{"type":"Url","text":"http://example.com"},{"type":"EmailAddress","text":"example@example.com"},{"type":"TwoFactorKey","binary":"AAECAwQFBgcICQ=="},{"type":"CreditCardNumber","text":"1234 5678 9012 3456"},{"type":"CreditCardExpiration","text":"Title"},{"type":"CreditCardVerificationValue","text":"0987"},{"type":"CreditCardPin","text":"6543"},{"type":"QRCode","text":"https://medo64.com/"}]}""", exported);
    // }

    // [TestMethod]
    // public void Entry_ExportImport_CustomTextField() {
    //     var entry = new Entry("Title");
    //     entry.Uuid = Guid.Parse("01234567-89ab-cdef-0123-456789abcdef");
    //     entry.Records.Add(new CustomTextRecord() {
    //         Caption = "Caption",
    //         Text = "Text",
    //     });

    //     var exported = entry.ExportToJson();
    //     Assert.AreEqual("""{"kind":"Bimil.Record","fields":[{"type":"Uuid","uuid":"01234567-89ab-cdef-0123-456789abcdef"},{"type":"Title","text":"Title"},{"type":"Password","text":""},{"type":"CustomTextField","caption":"Caption","text":"Text"}]}""", exported);

    //     var imported = Entry.ImportFromJson(exported);
    //     var exported2 = imported.ExportToJson();

    //     Assert.AreEqual(exported, exported2);
    // }

    // [TestMethod]
    // public void Entry_ExportImport_CustomTextFieldSensitive() {
    //     var entry = new Entry("Title");
    //     entry.Uuid = Guid.Parse("01234567-89ab-cdef-0123-456789abcdef");
    //     entry.Records.Add(new CustomTextRecord() {
    //         Caption = "Caption",
    //         Text = "Text",
    //         IsSensitive = true,
    //     });

    //     var exported = entry.ExportToJson();
    //     Assert.AreEqual("""{"kind":"Bimil.Record","fields":[{"type":"Uuid","uuid":"01234567-89ab-cdef-0123-456789abcdef"},{"type":"Title","text":"Title"},{"type":"Password","text":""},{"type":"CustomTextField","caption":"Caption","text":"Text","isSensitive":true}]}""", exported);

    //     var imported = Entry.ImportFromJson(exported);
    //     var exported2 = imported.ExportToJson();

    //     Assert.AreEqual(exported, exported2);
    // }

    // public void Entry_ExportImport_TryImport() {
    //     var entry = new Entry("Title");
    //     entry.Uuid = Guid.Parse("01234567-89ab-cdef-0123-456789abcdef");
    //     entry.Records.Add(new CustomTextRecord() {
    //         Caption = "Caption",
    //         Text = "Text",
    //         IsSensitive = true,
    //     });

    //     var exported = entry.ExportToJson();
    //     Assert.AreEqual("""{"kind":"Bimil.Record","fields":[{"type":"Uuid","uuid":"01234567-89ab-cdef-0123-456789abcdef"},{"type":"Title","text":"Title"},{"type":"Password","text":""},{"type":"CustomTextField","caption":"Caption","text":"Text","isSensitive":true}]}""", exported);

    //     Assert.IsTrue(Entry.TryImportFromJson(exported, out var imported));
    //     var exported2 = imported.ExportToJson();

    //     Assert.AreEqual(exported, exported2);
    // }

    // [TestMethod]
    // public void Entry_ExportImport_WrongKind() {
    //     var exported = """{"kind":"X","fields":[{"type":"Uuid","uuid":"01234567-89ab-cdef-0123-456789abcdef"},{"type":"Title","text":"Title"},{"type":"Password","text":""},{"type":"CustomTextField","caption":"Caption","text":"Text","isSensitive":true}]}""";
    //     Assert.Throws<InvalidDataException>(() => {
    //         Entry.ImportFromJson(exported);
    //     });
    // }

    // [TestMethod]
    // public void Entry_ExportImport_NoData() {
    //     var exported = "";
    //     Assert.Throws<InvalidDataException>(() => {
    //         Entry.ImportFromJson(exported);
    //     });
    // }

    // [TestMethod]
    // public void Entry_ExportImport_WrongData() {
    //     var exported = "[]";
    //     Assert.Throws<InvalidDataException>(() => {
    //         Entry.ImportFromJson(exported);
    //     });
    // }

    // [TestMethod]
    // public void Entry_ExportImport_NonJson() {
    //     var exported = "C";
    //     Assert.Throws<InvalidDataException>(() => {
    //         Entry.ImportFromJson(exported);
    //     });
    //     Assert.IsFalse(Entry.TryImportFromJson(exported, out _));
    // }

    #endregion Export/Import

}
