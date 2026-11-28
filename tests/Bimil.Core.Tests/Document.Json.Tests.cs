namespace Tests;

using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bimil;

public partial class DocumentTests {

    [TestMethod]
    public void Document_Export_Empty() {
        var exported = Document.ExportRecordsToJson(new Record[] { });
        Assert.AreEqual("""{"id":"Bimil","records":[]}""", exported);

        var imported = Document.ImportRecordsFromJson(exported);
        Assert.IsEmpty(imported);
    }


    [TestMethod]
    public void Document_Export_Basic() {
        var record1 = EntryRecord.Create();
        record1.Fields.Add(TextField.Create(FieldType.Title));
        record1.Fields.Add(TextField.Create(FieldType.Password));

        var exported = Document.ExportRecordsToJson(new Record[] { record1 });
        var exported2 = exported.Replace(record1.Uuid.ToString(), "01234567-89ab-cdef-0123-456789abcdef");
        Assert.AreEqual("""{"id":"Bimil","records":[{"fields":[{"type":"Uuid","uuid":"01234567-89ab-cdef-0123-456789abcdef"},{"type":"Title","text":""},{"type":"Password","text":""}]}]}""", exported2);

        var imported = Document.ImportRecordsFromJson(exported);
        Assert.AreEqual(1, imported.Length);

        var importedRecord = (EntryRecord)imported[0];
        Assert.AreEqual(3, importedRecord.Fields.Count);
        Assert.AreEqual(record1.Uuid, importedRecord.Uuid);
        Assert.AreEqual(record1.Title, importedRecord.Title);
        Assert.AreEqual(((TextField)(record1.Fields[FieldType.Password])).Text, ((TextField)(importedRecord.Fields[FieldType.Password])).Text);
    }

    [TestMethod]
    public void Document_ExportImport_AllTypes() {
        var record1 = EntryRecord.Create();
        record1.Uuid = Guid.Parse("01234567-89ab-cdef-0123-456789abcdef");
        record1.Group = "Group";
        record1.Title = "Title";
        record1.Fields.Add(Field.Create(FieldType.UserName, TextField.GetBytes("UserName")));
        record1.Fields.Add(Field.Create(FieldType.Notes, TextField.GetBytes("Notes")));
        record1.Fields.Add(Field.Create(FieldType.Password, TextField.GetBytes("Password")));
        record1.CreationTime = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        record1.Fields.Add(Field.Create(FieldType.PasswordModificationTime, TimestampField.GetBytes(new DateTime(2002, 1, 1, 0, 0, 0, DateTimeKind.Utc))));
        record1.Fields.Add(Field.Create(FieldType.LastAccessTime, TimestampField.GetBytes(new DateTime(2003, 1, 1, 0, 0, 0, DateTimeKind.Utc))));
        record1.Fields.Add(Field.Create(FieldType.PasswordExpiryTime, TimestampField.GetBytes(new DateTime(2004, 1, 1, 0, 0, 0, DateTimeKind.Utc))));
        record1.Fields.Add(Field.Create(FieldType.LastModificationTime, TimestampField.GetBytes(new DateTime(2005, 1, 1, 0, 0, 0, DateTimeKind.Utc))));
        record1.Fields.Add(Field.Create(FieldType.Url, TextField.GetBytes("http://example.com")));
        record1.Fields.Add(Field.Create(FieldType.EmailAddress, TextField.GetBytes("example@example.com")));
        record1.Fields.Add(Field.Create(FieldType.TwoFactorKey, [0, 1, 2, 3, 4, 5, 6, 7, 8, 9]));
        record1.Fields.Add(Field.Create(FieldType.CreditCardNumber, TextField.GetBytes("1234 5678 9012 3456")));
        record1.Fields.Add(Field.Create(FieldType.CreditCardExpiration, TextField.GetBytes("Title")));
        record1.Fields.Add(Field.Create(FieldType.CreditCardVerificationValue, TextField.GetBytes("0987")));
        record1.Fields.Add(Field.Create(FieldType.CreditCardPin, TextField.GetBytes("6543")));
        record1.Fields.Add(Field.Create(FieldType.QRCode, TextField.GetBytes("https://medo64.com/")));

        var exported = Document.ExportRecordsToJson(new Record[] { record1 });
        Assert.AreEqual("""{"id":"Bimil","records":[{"fields":[{"type":"Uuid","uuid":"01234567-89ab-cdef-0123-456789abcdef"},{"type":"Group","text":"Group"},{"type":"Title","text":"Title"},{"type":"UserName","text":"UserName"},{"type":"Notes","text":"Notes"},{"type":"Password","text":"Password"},{"type":"CreationTime","time":"2001-01-01T00:00:00Z"},{"type":"PasswordModificationTime","time":"2002-01-01T00:00:00Z"},{"type":"LastAccessTime","time":"2003-01-01T00:00:00Z"},{"type":"PasswordExpiryTime","time":"2004-01-01T00:00:00Z"},{"type":"LastModificationTime","time":"2005-01-01T00:00:00Z"},{"type":"Url","text":"http://example.com"},{"type":"EmailAddress","text":"example@example.com"},{"type":"TwoFactorKey","binary":"AAECAwQFBgcICQ=="},{"type":"CreditCardNumber","text":"1234 5678 9012 3456"},{"type":"CreditCardExpiration","text":"Title"},{"type":"CreditCardVerificationValue","text":"0987"},{"type":"CreditCardPin","text":"6543"},{"type":"QRCode","text":"https://medo64.com/"}]}]}""", exported);
    }

    [TestMethod]
    public void Document_ExportImport_CustomTextField() {
        var record1 = EntryRecord.Create();
        record1.Title = "Title";
        record1.Uuid = Guid.Parse("01234567-89ab-cdef-0123-456789abcdef");
        var ctf = Field.Create(FieldType.CustomTextField) as CustomTextField;
        ctf.Caption = "Caption";
        ctf.Text = "Text";
        record1.Fields.Add(ctf);

        var exported = Document.ExportRecordsToJson(new Record[] { record1 });
        Assert.AreEqual("""{"id":"Bimil","records":[{"fields":[{"type":"Uuid","uuid":"01234567-89ab-cdef-0123-456789abcdef"},{"type":"Title","text":"Title"},{"type":"CustomTextField","caption":"Caption","text":"Text"}]}]}""", exported);

        var imported = Document.ImportRecordsFromJson(exported);
        var exported2 = Document.ExportRecordsToJson(imported);

        Assert.AreEqual(exported, exported2);
    }

    [TestMethod]
    public void Document_ExportImport_CustomTextFieldSensitive() {
        var record1 = EntryRecord.Create();
        record1.Title = "Title";
        record1.Uuid = Guid.Parse("01234567-89ab-cdef-0123-456789abcdef");
        var ctf = Field.Create(FieldType.CustomTextField) as CustomTextField;
        ctf.Caption = "Caption";
        ctf.Text = "Text";
        ctf.IsSensitive = true;
        record1.Fields.Add(ctf);

        var exported = Document.ExportRecordsToJson(new Record[] { record1 });
        Assert.AreEqual("""{"id":"Bimil","records":[{"fields":[{"type":"Uuid","uuid":"01234567-89ab-cdef-0123-456789abcdef"},{"type":"Title","text":"Title"},{"type":"CustomTextField","caption":"Caption","text":"Text","isSensitive":true}]}]}""", exported);

        var imported = Document.ImportRecordsFromJson(exported);
        var exported2 = Document.ExportRecordsToJson(imported);

        Assert.AreEqual(exported, exported2);
    }

    public void Document_ExportImport_TryImport() {
        var record1 = EntryRecord.Create();
        record1.Title = "Title";
        record1.Uuid = Guid.Parse("01234567-89ab-cdef-0123-456789abcdef");
        var ctf = Field.Create(FieldType.CustomTextField) as CustomTextField;
        ctf.Caption = "Caption";
        ctf.Text = "Text";
        ctf.IsSensitive = true;
        record1.Fields.Add(ctf);

        var exported = Document.ExportRecordsToJson(new Record[] { record1 });
        Assert.AreEqual("""{"id":"Bimil","records":[{"fields":[{"type":"Uuid","uuid":"01234567-89ab-cdef-0123-456789abcdef"},{"type":"Title","text":"Title"},{"type":"CustomTextField","caption":"Caption","text":"Text","isSensitive":true}]}]}""", exported);

        Assert.IsTrue(Document.TryImportRecordsFromJson(exported, out var imported));
        var exported2 = Document.ExportRecordsToJson(imported);

        Assert.AreEqual(exported, exported2);
    }

    [TestMethod]
    public void Document_ExportImport_TryImport2() {
        var record1 = EntryRecord.Create();
        record1.Title = "Title1";
        record1.Uuid = Guid.Parse("01234567-89ab-cdef-0123-456789abcdef");
        var ctf1 = Field.Create(FieldType.CustomTextField) as CustomTextField;
        ctf1.Caption = "Caption1";
        ctf1.Text = "Text1";
        ctf1.IsSensitive = true;
        record1.Fields.Add(ctf1);

        var record2 = EntryRecord.Create();
        record2.Title = "Title2";
        record2.Uuid = Guid.Parse("fedcba98-7654-3210-fedc-ba9876543210");
        var ctf2 = Field.Create(FieldType.CustomTextField) as CustomTextField;
        ctf2.Caption = "Caption2";
        ctf2.Text = "Text2";
        ctf2.IsSensitive = false;
        record2.Fields.Add(ctf2);

        var exported = Document.ExportRecordsToJson(new Record[] { record1, record2 });
        Assert.AreEqual("""{"id":"Bimil","records":[{"fields":[{"type":"Uuid","uuid":"01234567-89ab-cdef-0123-456789abcdef"},{"type":"Title","text":"Title1"},{"type":"CustomTextField","caption":"Caption1","text":"Text1","isSensitive":true}]},{"fields":[{"type":"Uuid","uuid":"fedcba98-7654-3210-fedc-ba9876543210"},{"type":"Title","text":"Title2"},{"type":"CustomTextField","caption":"Caption2","text":"Text2"}]}]}""", exported);

        Assert.IsTrue(Document.TryImportRecordsFromJson(exported, out var imported));
        var exported2 = Document.ExportRecordsToJson(imported);

        Assert.AreEqual(exported, exported2);
    }

    [TestMethod]
    public void Document_ExportPrettyPrint() {
        var record1 = EntryRecord.Create();
        record1.Title = "Title";
        record1.Uuid = Guid.Parse("01234567-89ab-cdef-0123-456789abcdef");
        var ctf = Field.Create(FieldType.CustomTextField) as CustomTextField;
        ctf.Caption = "Caption";
        ctf.Text = "Text";
        ctf.IsSensitive = true;
        record1.Fields.Add(ctf);

        var exported = Document.ExportRecordsToJson(new Record[] { record1 }, prettyPrint: true).Replace("\r", "");
        Assert.AreEqual("{\n  \"id\": \"Bimil\",\n  \"records\": [\n    {\n      \"fields\": [\n        {\n          \"type\": \"Uuid\",\n          \"uuid\": \"01234567-89ab-cdef-0123-456789abcdef\"\n        },\n        {\n          \"type\": \"Title\",\n          \"text\": \"Title\"\n        },\n        {\n          \"type\": \"CustomTextField\",\n          \"caption\": \"Caption\",\n          \"text\": \"Text\",\n          \"isSensitive\": true\n        }\n      ]\n    }\n  ]\n}", exported);

        Assert.IsTrue(Document.TryImportRecordsFromJson(exported, out var imported));
        var exported2 = Document.ExportRecordsToJson(imported, prettyPrint: true).Replace("\r", "");

        Assert.AreEqual(exported, exported2);
    }

    [TestMethod]
    public void Document_ExportImport_WrongKind() {
        var exported = """{"id":"NotBimil","records":[{"fields":[{"type":"Uuid","uuid":"01234567-89ab-cdef-0123-456789abcdef"},{"type":"Title","text":""},{"type":"Password","text":""}]}]}""";
        Assert.ThrowsExactly<InvalidDataException>(() => {
            Document.ImportRecordsFromJson(exported);
        });
    }

    [TestMethod]
    public void Document_ExportImport_NoData() {
        var exported = "";
        Assert.ThrowsExactly<InvalidDataException>(() => {
            Document.ImportRecordsFromJson(exported);
        });
    }

    [TestMethod]
    public void Document_ExportImport_WrongData() {
        var exported = "[]";
        Assert.Throws<InvalidDataException>(() => {
            Document.ImportRecordsFromJson(exported);
        });
    }

    [TestMethod]
    public void Document_ExportImport_NonJson() {
        var exported = "C";
        Assert.Throws<InvalidDataException>(() => {
            Document.ImportRecordsFromJson(exported);
        });
        Assert.IsFalse(Document.TryImportRecordsFromJson(exported, out _));
    }

}
