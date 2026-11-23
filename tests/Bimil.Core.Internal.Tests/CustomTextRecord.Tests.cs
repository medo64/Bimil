namespace Tests;

using System;
using System.Text;
using System.Text.Unicode;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bimil;

[TestClass]
public class CustomTextField_Tests {

    [TestMethod]
    public void CustomTextField_NonSensitive() {
        var data = new ProtectedBytes(ASCIIEncoding.ASCII.GetBytes("010009WiFi SSID020004Test"));
        var field = new CustomTextField(FieldType.CustomTextField, data);
        Assert.AreEqual("WiFi SSID", field.Caption);
        Assert.AreEqual("Test", field.Text);
        Assert.IsFalse(field.IsSensitive);
    }

    [TestMethod]
    public void CustomTextField_Sensitive() {
        var data = new ProtectedBytes(ASCIIEncoding.ASCII.GetBytes("01000DWiFi Password02000Dbobsyouruncle0300011"));
        var field = new CustomTextField(FieldType.CustomTextField, data);
        Assert.AreEqual("WiFi Password", field.Caption);
        Assert.AreEqual("bobsyouruncle", field.Text);
        Assert.IsTrue(field.IsSensitive);
    }

    [TestMethod]
    public void CustomTextField_OnlyParseTheFirst() {  // didn't implement parsing of all entries, just take the first
        var data = new ProtectedBytes(ASCIIEncoding.ASCII.GetBytes("010009WiFi SSID020004Test00000001000DWiFi Password02000Dbobsyouruncle0300011"));
        var field = new CustomTextField(FieldType.CustomTextField, data);
        Assert.AreEqual("WiFi SSID", field.Caption);
        Assert.AreEqual("Test", field.Text);
        Assert.IsFalse(field.IsSensitive);
    }

    [TestMethod]
    public void CustomTextField_CaptionTypeTooShort() {
        var data = new ProtectedBytes(ASCIIEncoding.ASCII.GetBytes("0"));
        var field = new CustomTextField(FieldType.CustomTextField, data);
        Assert.AreEqual("", field.Caption);
        Assert.AreEqual("", field.Text);
        Assert.IsFalse(field.IsSensitive);
    }

    [TestMethod]
    public void CustomTextField_CaptionLengthTooShort() {
        var data = new ProtectedBytes(ASCIIEncoding.ASCII.GetBytes("01000"));
        var field = new CustomTextField(FieldType.CustomTextField, data);
        Assert.AreEqual("", field.Caption);
        Assert.AreEqual("", field.Text);
        Assert.IsFalse(field.IsSensitive);
    }

    [TestMethod]
    public void CustomTextField_CaptionContentTooShort() {
        var data = new ProtectedBytes(ASCIIEncoding.ASCII.GetBytes("010009WiFi SSI"));
        var field = new CustomTextField(FieldType.CustomTextField, data);
        Assert.AreEqual("", field.Caption);
        Assert.AreEqual("", field.Text);
        Assert.IsFalse(field.IsSensitive);
    }


    [TestMethod]
    public void CustomTextField_TextTypeTooShort() {
        var data = new ProtectedBytes(ASCIIEncoding.ASCII.GetBytes("010009WiFi SSID0"));
        var field = new CustomTextField(FieldType.CustomTextField, data);
        Assert.AreEqual("WiFi SSID", field.Caption);
        Assert.AreEqual("", field.Text);
        Assert.IsFalse(field.IsSensitive);
    }

    [TestMethod]
    public void CustomTextField_TextLengthTooShort() {
        var data = new ProtectedBytes(ASCIIEncoding.ASCII.GetBytes("010009WiFi SSID02000"));
        var field = new CustomTextField(FieldType.CustomTextField, data);
        Assert.AreEqual("WiFi SSID", field.Caption);
        Assert.AreEqual("", field.Text);
        Assert.IsFalse(field.IsSensitive);
    }

    [TestMethod]
    public void CustomTextField_TextContentTooShort() {
        var data = new ProtectedBytes(ASCIIEncoding.ASCII.GetBytes("010009WiFi SSID02000"));
        var field = new CustomTextField(FieldType.CustomTextField, data);
        Assert.AreEqual("WiFi SSID", field.Caption);
        Assert.AreEqual("", field.Text);
        Assert.IsFalse(field.IsSensitive);
    }

    [TestMethod]
    public void CustomTextField_SensitiveTypeTooShort() {
        var data = new ProtectedBytes(ASCIIEncoding.ASCII.GetBytes("010009WiFi SSID020004Test0"));
        var field = new CustomTextField(FieldType.CustomTextField, data);
        Assert.AreEqual("WiFi SSID", field.Caption);
        Assert.AreEqual("Test", field.Text);
        Assert.IsFalse(field.IsSensitive);
    }

    [TestMethod]
    public void CustomTextField_SensitiveLengthTooShort() {
        var data = new ProtectedBytes(ASCIIEncoding.ASCII.GetBytes("010009WiFi SSID020004Test0000000"));
        var field = new CustomTextField(FieldType.CustomTextField, data);
        Assert.AreEqual("WiFi SSID", field.Caption);
        Assert.AreEqual("Test", field.Text);
        Assert.AreEqual(false, field.IsSensitive);
    }

    [TestMethod]
    public void CustomTextField_SensitiveContentTooShort() {
        var data = new ProtectedBytes(ASCIIEncoding.ASCII.GetBytes("010009WiFi SSID020004Test00000001"));
        var field = new CustomTextField(FieldType.CustomTextField, data);
        Assert.AreEqual("WiFi SSID", field.Caption);
        Assert.AreEqual("Test", field.Text);
        Assert.IsFalse(field.IsSensitive);
    }

    [TestMethod]
    public void CustomTextField_SetData() {
        var data = new ProtectedBytes();
        var field = new CustomTextField(FieldType.CustomTextField, data) {
            Caption = "Test",
            Text = "Value",
        };
        Assert.AreEqual("Test", field.Caption);
        Assert.AreEqual("Value", field.Text);
        Assert.IsFalse(field.IsSensitive);
        var content = UTF8Encoding.UTF8.GetString(field.Data.GetBytes());
        Assert.AreEqual("010004Test020005Value", content);
    }

    [TestMethod]
    public void CustomTextField_SetSensitiveData() {
        var data = new ProtectedBytes();
        var field = new CustomTextField(FieldType.CustomTextField, data) {
            Caption = "Test",
            Text = "Value",
            IsSensitive = true,
        };
        Assert.AreEqual("Test", field.Caption);
        Assert.AreEqual("Value", field.Text);
        Assert.IsTrue(field.IsSensitive);
        var content = UTF8Encoding.UTF8.GetString(field.Data.GetBytes());
        Assert.AreEqual("010004Test020005Value0300011", content);
    }

}
