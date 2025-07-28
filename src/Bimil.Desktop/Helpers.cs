namespace Bimil;

using System;
using Medo.Security.Cryptography.PasswordSafe;

internal static class Helpers {

    public static string? GetRecordCaption(RecordType recordType) {
        switch (recordType) {
            case RecordType.UserName: return "User name";
            case RecordType.Password: return "Password";
            case RecordType.Url: return "URL";
            case RecordType.EmailAddress: return "E-mail";
            case RecordType.Notes: return "Notes";

            case RecordType.TwoFactorKey: return "Two-factor key";
            case RecordType.CreditCardNumber: return "Card number";
            case RecordType.CreditCardExpiration: return "Card expiration";
            case RecordType.CreditCardVerificationValue: return "Card security code";
            case RecordType.CreditCardPin: return "Card PIN";

            case RecordType.QRCode: return "QR Code";

            case RecordType.RunCommand: return "Run command";

            default: return null; //all other fields are not really supported
        }
    }

}
