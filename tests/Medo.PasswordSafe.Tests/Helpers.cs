using System.IO;
using System.Reflection;
using PwSafe = Medo.Security.Cryptography.PasswordSafe;

namespace Tests;

internal static class Helpers {

    public static MemoryStream GetResourceStream(string fileName) {
        var resStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Tests.Resources." + fileName);
        if (resStream == null) { resStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Tests." + fileName); }
        var buffer = new byte[(int)resStream.Length];
        resStream.Read(buffer, 0, buffer.Length);
        return new MemoryStream(buffer) { Position = 0 };
    }

    public static PwSafe.Entry GetExampleEntry(string autotypeText, bool amexCard = false) {
        var entry = new PwSafe.Entry() {
            Title = "Example",
            Group = "Examples",
            UserName = "Default",
            Password = "Passw0rd",
            CreditCardNumber = amexCard ? "123 4567 8901 2345" : "1234 5678 9012 3456",
            CreditCardExpiration = "01/79",
            CreditCardVerificationValue = "123",
            CreditCardPin = "1234",
            TwoFactorKey = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
            Email = "test@example.com",
            Url = "medo64.com",
            Notes = "1\r\n2\n3\r^\n",
        };
        if (autotypeText != null) { entry.Autotype = autotypeText; }
        return entry;
    }

}

