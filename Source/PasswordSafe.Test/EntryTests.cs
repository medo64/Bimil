using System;
using System.IO;
using Xunit;
using PwSafe = Medo.Security.Cryptography.PasswordSafe;

namespace PasswordSafe.Test {
    public class EntryTests {

        [Fact(DisplayName = "PasswordSafe: Entry: New")]
        public void Entry_New() {
            var entry = new PwSafe.Entry();
            Assert.Equal(3, entry.Records.Count);
            Assert.True(entry.Records.Contains(PwSafe.RecordType.Uuid));
            Assert.True(entry.Records.Contains(PwSafe.RecordType.Title));
            Assert.True(entry.Records.Contains(PwSafe.RecordType.Password));
            Assert.True(entry.Uuid != Guid.Empty);
            Assert.Equal("", entry.Title);
            Assert.Equal("", entry.Password);
        }

        [Fact(DisplayName = "PasswordSafe: Entry: New with Title")]
        public void Entry_New_WithTitle() {
            var entry = new PwSafe.Entry("Test");
            Assert.Equal(3, entry.Records.Count);
            Assert.True(entry.Records.Contains(PwSafe.RecordType.Uuid));
            Assert.True(entry.Records.Contains(PwSafe.RecordType.Title));
            Assert.True(entry.Records.Contains(PwSafe.RecordType.Password));
            Assert.True(entry.Uuid != Guid.Empty);
            Assert.Equal("Test", entry.Title);
            Assert.Equal("", entry.Password);
        }


        [Fact(DisplayName = "PasswordSafe: Entry: Clone")]
        public void Entry_Clone() {
            var entry = new PwSafe.Entry("Test");
            Assert.Equal(3, entry.Records.Count);
            Assert.True(entry.Records.Contains(PwSafe.RecordType.Uuid));
            Assert.True(entry.Records.Contains(PwSafe.RecordType.Title));
            Assert.True(entry.Records.Contains(PwSafe.RecordType.Password));
            Assert.True(entry.Uuid != Guid.Empty);
            Assert.Equal("Test", entry.Title);
            Assert.Equal("", entry.Password);

            var clone = entry.Clone();
            Assert.Equal(3, clone.Records.Count);
            Assert.True(clone.Records.Contains(PwSafe.RecordType.Uuid));
            Assert.True(clone.Records.Contains(PwSafe.RecordType.Title));
            Assert.True(clone.Records.Contains(PwSafe.RecordType.Password));
            Assert.True(clone.Uuid != Guid.Empty);
            Assert.Equal("Test", clone.Title);
            Assert.Equal("", clone.Password);
        }

        [Fact(DisplayName = "PasswordSafe: Entry: Clone (in document)")]
        public void Entry_Clone_Document() {
            var doc = new PwSafe.Document("Password");
            doc.Entries.Add(new PwSafe.Entry("Test"));
            doc.Save(new MemoryStream());

            doc.Entries[0].Clone();
            Assert.False(doc.HasChanged);
        }


        [Fact(DisplayName = "PasswordSafe: Entry: Change (read-only)")]
        public void Entry_ReadOnly() {
            Assert.Throws<NotSupportedException>(() => {
                var doc = new PwSafe.Document("Password");
                doc.Entries["Test"].Password = "Old";

                doc.IsReadOnly = true;
                doc.Entries["Test"].Password = "New";
            });
        }


        [Fact(DisplayName = "PasswordSafe: Entry: Indexer Get via Type")]
        public void Entry_AccessByRecordType() {
            var doc = new PwSafe.Document("Password");

            doc.Entries["Test"].Password = "Old";
            Assert.True(doc.Entries["Test"][PwSafe.RecordType.Uuid].Uuid != Guid.Empty);
            Assert.Equal("Old", doc.Entries["Test"][PwSafe.RecordType.Password].Text);

            doc.Entries["Test"][PwSafe.RecordType.Password].Text = "New";
            Assert.Equal("New", doc.Entries["Test"][PwSafe.RecordType.Password].Text);
        }


        [Fact(DisplayName = "PasswordSafe: Entry: Add")]
        public void Entry_TestNamed() {
            var guid = Guid.NewGuid();

            var doc = new PwSafe.Document("Password") { TrackAccess = false, TrackModify = false };
            var entry = new PwSafe.Entry();
            doc.Entries.Add(entry);

            entry.Uuid = guid;
            entry.Group = "Group";
            entry.Title = "Title";
            entry.UserName = "UserName";
            entry.Notes = "Notes";
            entry.Password = "Password";
            entry.CreationTime = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            entry.PasswordModificationTime = new DateTime(2002, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            entry.LastAccessTime = new DateTime(2003, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            entry.PasswordExpiryTime = new DateTime(2004, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            entry.LastModificationTime = new DateTime(2005, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            entry.Url = "http://example.com";
            entry.Email = "example@example.com";
            entry.TwoFactorKey = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            entry.CreditCardNumber = "1234 5678 9012 3456";
            entry.CreditCardExpiration = "Title";
            entry.CreditCardVerificationValue = "0987";
            entry.CreditCardPin = "6543";
            entry.QRCode = "https://medo64.com/";

            Assert.Equal(guid, entry.Uuid);
            Assert.Equal("Group", (string)entry.Group);
            Assert.Equal("Title", entry.Title);
            Assert.Equal("UserName", entry.UserName);
            Assert.Equal("Notes", entry.Notes);
            Assert.Equal("Password", entry.Password);
            Assert.Equal(new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc), entry.CreationTime);
            Assert.Equal(new DateTime(2002, 1, 1, 0, 0, 0, DateTimeKind.Utc), entry.PasswordModificationTime);
            Assert.Equal(new DateTime(2003, 1, 1, 0, 0, 0, DateTimeKind.Utc), entry.LastAccessTime);
            Assert.Equal(new DateTime(2004, 1, 1, 0, 0, 0, DateTimeKind.Utc), entry.PasswordExpiryTime);
            Assert.Equal(new DateTime(2005, 1, 1, 0, 0, 0, DateTimeKind.Utc), entry.LastModificationTime);
            Assert.Equal("http://example.com", entry.Url);
            Assert.Equal("example@example.com", entry.Email);
            Assert.Equal("00-01-02-03-04-05-06-07-08-09", BitConverter.ToString(entry.TwoFactorKey));
            Assert.Equal("1234 5678 9012 3456", entry.CreditCardNumber);
            Assert.Equal("Title", entry.CreditCardExpiration);
            Assert.Equal("0987", entry.CreditCardVerificationValue);
            Assert.Equal("6543", entry.CreditCardPin);
            Assert.Equal("https://medo64.com/", entry.QRCode);

            Assert.Equal(guid, entry[PwSafe.RecordType.Uuid].Uuid);
            Assert.Equal("Group", entry[PwSafe.RecordType.Group].Text);
            Assert.Equal("Title", entry[PwSafe.RecordType.Title].Text);
            Assert.Equal("UserName", entry[PwSafe.RecordType.UserName].Text);
            Assert.Equal("Notes", entry[PwSafe.RecordType.Notes].Text);
            Assert.Equal("Password", entry[PwSafe.RecordType.Password].Text);
            Assert.Equal(new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc), entry[PwSafe.RecordType.CreationTime].Time);
            Assert.Equal(new DateTime(2002, 1, 1, 0, 0, 0, DateTimeKind.Utc), entry[PwSafe.RecordType.PasswordModificationTime].Time);
            Assert.Equal(new DateTime(2003, 1, 1, 0, 0, 0, DateTimeKind.Utc), entry[PwSafe.RecordType.LastAccessTime].Time);
            Assert.Equal(new DateTime(2004, 1, 1, 0, 0, 0, DateTimeKind.Utc), entry[PwSafe.RecordType.PasswordExpiryTime].Time);
            Assert.Equal(new DateTime(2005, 1, 1, 0, 0, 0, DateTimeKind.Utc), entry[PwSafe.RecordType.LastModificationTime].Time);
            Assert.Equal("http://example.com", entry[PwSafe.RecordType.Url].Text);
            Assert.Equal("example@example.com", entry[PwSafe.RecordType.EmailAddress].Text);
            Assert.Equal("00-01-02-03-04-05-06-07-08-09", BitConverter.ToString(entry[PwSafe.RecordType.TwoFactorKey].GetBytes()));
            Assert.Equal("1234 5678 9012 3456", entry[PwSafe.RecordType.CreditCardNumber].Text);
            Assert.Equal("Title", entry[PwSafe.RecordType.CreditCardExpiration].Text);
            Assert.Equal("0987", entry[PwSafe.RecordType.CreditCardVerificationValue].Text);
            Assert.Equal("6543", entry[PwSafe.RecordType.CreditCardPin].Text);
            Assert.Equal("https://medo64.com/", entry[PwSafe.RecordType.QRCode].Text);
        }



        [Fact(DisplayName = "PasswordSafe: Entry: Autotype (default tokens)")]
        public void Entry_Autotype_Tokens_Default() {
            Assert.Equal("UserName {Tab} Password {Enter}", string.Join(" ", PwSafe.AutotypeToken.GetUnexpandedAutotypeTokens(null)));
            Assert.Equal("D e f a u l t {Tab} P a s s w 0 r d {Enter}", string.Join(" ", GetExampleEntry(null).AutotypeTokens));
        }

        [Fact(DisplayName = "PasswordSafe: Entry: Autotype (two-factor)")]
        public void Entry_Autotype_Tokens_TwoFactor() {
            var autoTypeText = @"\u\t\p\t\2\t\n";
            Assert.Equal("UserName {Tab} Password {Tab} TwoFactorCode {Tab} {Enter}", string.Join(" ", PwSafe.AutotypeToken.GetUnexpandedAutotypeTokens(autoTypeText)));
            Assert.Equal("D e f a u l t {Tab} P a s s w 0 r d {Tab} TwoFactorCode {Tab} {Enter}", string.Join(" ", GetExampleEntry(autoTypeText).AutotypeTokens));
        }


        [Fact(DisplayName = "PasswordSafe: Entry: Autotype (some text)")]
        public void Entry_Autotype_Tokens_SomeText1() {
            var autoTypeText = @"admin\n\p\n";
            Assert.Equal("a d m i n {Enter} Password {Enter}", string.Join(" ", PwSafe.AutotypeToken.GetUnexpandedAutotypeTokens(autoTypeText)));
            Assert.Equal("a d m i n {Enter} P a s s w 0 r d {Enter}", string.Join(" ", GetExampleEntry(autoTypeText).AutotypeTokens));
        }

        [Fact(DisplayName = "PasswordSafe: Entry: Autotype (some text 2)")]
        public void Entry_Autotype_Tokens_SomeText2() {
            var autoTypeText = @"\badmin\n\p\n";
            Assert.Equal("{Backspace} a d m i n {Enter} Password {Enter}", string.Join(" ", PwSafe.AutotypeToken.GetUnexpandedAutotypeTokens(autoTypeText)));
            Assert.Equal("{Backspace} a d m i n {Enter} P a s s w 0 r d {Enter}", string.Join(" ", GetExampleEntry(autoTypeText).AutotypeTokens));
        }

        [Fact(DisplayName = "PasswordSafe: Entry: Autotype (some text 3)")]
        public void Entry_Autotype_Tokens_SomeText3() {
            var autoTypeText = @"admin\n\p\nXXX";
            Assert.Equal("a d m i n {Enter} Password {Enter} X X X", string.Join(" ", PwSafe.AutotypeToken.GetUnexpandedAutotypeTokens(autoTypeText)));
            Assert.Equal("a d m i n {Enter} P a s s w 0 r d {Enter} X X X", string.Join(" ", GetExampleEntry(autoTypeText).AutotypeTokens));
        }


        [Fact(DisplayName = "PasswordSafe: Entry: Autotype (credit card)")]
        public void Entry_Autotype_Tokens_CreditCard() {
            var autoTypeText = @"\cn\t\ce\t\cv\t\cp";
            Assert.Equal("CreditCardNumber {Tab} CreditCardExpiration {Tab} CreditCardVerificationValue {Tab} CreditCardPin", string.Join(" ", PwSafe.AutotypeToken.GetUnexpandedAutotypeTokens(autoTypeText)));
            Assert.Equal("1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 {Tab} 0 1 / 7 9 {Tab} 1 2 3 {Tab} 1 2 3 4", string.Join(" ", GetExampleEntry(autoTypeText).AutotypeTokens));
        }

        [Fact(DisplayName = "PasswordSafe: Entry: Autotype (credit card, tabbed)")]
        public void Entry_Autotype_Tokens_CreditCardTabbed() {
            var autoTypeText = @"\ct\t\ce\t\cv\t\cp";
            Assert.Equal("CreditCardNumberTabbed {Tab} CreditCardExpiration {Tab} CreditCardVerificationValue {Tab} CreditCardPin", string.Join(" ", PwSafe.AutotypeToken.GetUnexpandedAutotypeTokens(autoTypeText)));
            Assert.Equal("1 2 3 4 {Tab} 5 6 7 8 {Tab} 9 0 1 2 {Tab} 3 4 5 6 {Tab} 0 1 / 7 9 {Tab} 1 2 3 {Tab} 1 2 3 4", string.Join(" ", GetExampleEntry(autoTypeText).AutotypeTokens));
        }

        [Fact(DisplayName = "PasswordSafe: Entry: Autotype (credit card, tabbed Amex)")]
        public void Entry_Autotype_Tokens_CreditCardTabbedAmex() {
            var autoTypeText = @"\ct\t\ce\t\cv\t\cp";
            Assert.Equal("CreditCardNumberTabbed {Tab} CreditCardExpiration {Tab} CreditCardVerificationValue {Tab} CreditCardPin", string.Join(" ", PwSafe.AutotypeToken.GetUnexpandedAutotypeTokens(autoTypeText)));
            Assert.Equal("1 2 3 {Tab} 4 5 6 7 {Tab} 8 9 0 1 {Tab} 2 3 4 5 {Tab} 0 1 / 7 9 {Tab} 1 2 3 {Tab} 1 2 3 4", string.Join(" ", GetExampleEntry(autoTypeText, amexCard: true).AutotypeTokens));
        }


        [Fact(DisplayName = "PasswordSafe: Entry: Autotype (optional number, not used)")]
        public void Entry_Autotype_Tokens_OptionalNumberNotUsed() {
            var autoTypeText = @"\oTest";
            Assert.Equal("Notes T e s t", string.Join(" ", PwSafe.AutotypeToken.GetUnexpandedAutotypeTokens(autoTypeText)));
            Assert.Equal("1 {Enter} 2 {Enter} 3 {Enter} {^} {Enter} T e s t", string.Join(" ", GetExampleEntry(autoTypeText).AutotypeTokens));
        }


        [Fact(DisplayName = "PasswordSafe: Entry: Autotype (optional number, line 1)")]
        public void Entry_Autotype_Tokens_OptionalNumber_Line1() {
            var autoTypeText = @"\o1Test";
            Assert.Equal("Notes:1 T e s t", string.Join(" ", PwSafe.AutotypeToken.GetUnexpandedAutotypeTokens(autoTypeText)));
            Assert.Equal("1 T e s t", string.Join(" ", GetExampleEntry(autoTypeText).AutotypeTokens));
        }

        [Fact(DisplayName = "PasswordSafe: Entry: Autotype (optional number, line 2)")]
        public void Entry_Autotype_Tokens_OptionalNumber_Line2() {
            var autoTypeText = @"\o2Test";
            Assert.Equal("Notes:2 T e s t", string.Join(" ", PwSafe.AutotypeToken.GetUnexpandedAutotypeTokens(autoTypeText)));
            Assert.Equal("2 T e s t", string.Join(" ", GetExampleEntry(autoTypeText).AutotypeTokens));
        }

        [Fact(DisplayName = "PasswordSafe: Entry: Autotype (optional number, line 3)")]
        public void Entry_Autotype_Tokens_OptionalNumber_Line3() {
            var autoTypeText = @"\o3Test";
            Assert.Equal("Notes:3 T e s t", string.Join(" ", PwSafe.AutotypeToken.GetUnexpandedAutotypeTokens(autoTypeText)));
            Assert.Equal("3 T e s t", string.Join(" ", GetExampleEntry(autoTypeText).AutotypeTokens));
        }

        [Fact(DisplayName = "PasswordSafe: Entry: Autotype (optional number, line 4)")]
        public void Entry_Autotype_Tokens_OptionalNumber_Line4() {
            var autoTypeText = @"\o4Test";
            Assert.Equal("Notes:4 T e s t", string.Join(" ", PwSafe.AutotypeToken.GetUnexpandedAutotypeTokens(autoTypeText)));
            Assert.Equal("{^} T e s t", string.Join(" ", GetExampleEntry(autoTypeText).AutotypeTokens));
        }

        [Fact(DisplayName = "PasswordSafe: Entry: Autotype (optional number, line 5)")]
        public void Entry_Autotype_Tokens_OptionalNumber_Line5() {
            var autoTypeText = @"\o5Test";
            Assert.Equal("Notes:5 T e s t", string.Join(" ", PwSafe.AutotypeToken.GetUnexpandedAutotypeTokens(autoTypeText)));
            Assert.Equal("T e s t", string.Join(" ", GetExampleEntry(autoTypeText).AutotypeTokens));
        }


        [Fact(DisplayName = "PasswordSafe: Entry: Autotype (optional number, single digit)")]
        public void Entry_Autotype_Tokens_OptionalNumberOneDigit() {
            var autoTypeText = @"\o9Test";
            Assert.Equal("Notes:9 T e s t", string.Join(" ", PwSafe.AutotypeToken.GetUnexpandedAutotypeTokens(autoTypeText)));
            Assert.Equal("T e s t", string.Join(" ", GetExampleEntry(autoTypeText).AutotypeTokens));
        }

        [Fact(DisplayName = "PasswordSafe: Entry: Autotype (optional number, two digit)")]
        public void Entry_Autotype_Tokens_OptionalNumberTwoDigits() {
            var autoTypeText = @"\o98Test";
            Assert.Equal("Notes:98 T e s t", string.Join(" ", PwSafe.AutotypeToken.GetUnexpandedAutotypeTokens(autoTypeText)));
            Assert.Equal("T e s t", string.Join(" ", GetExampleEntry(autoTypeText).AutotypeTokens));
        }

        [Fact(DisplayName = "PasswordSafe: Entry: Autotype (optional number, three digits)")]
        public void Entry_Autotype_Tokens_OptionalNumberThreeDigits() {
            var autoTypeText = @"\o987Test";
            Assert.Equal("Notes:987 T e s t", string.Join(" ", PwSafe.AutotypeToken.GetUnexpandedAutotypeTokens(autoTypeText)));
            Assert.Equal("T e s t", string.Join(" ", GetExampleEntry(autoTypeText).AutotypeTokens));
        }


        [Fact(DisplayName = "PasswordSafe: Entry: Autotype (optional number, no suffix)")]
        public void Entry_Autotype_Tokens_OptionalNumberNoSuffix() {
            var autoTypeText = @"\o12";
            Assert.Equal("Notes:12", string.Join(" ", PwSafe.AutotypeToken.GetUnexpandedAutotypeTokens(autoTypeText)));
            Assert.Equal("", string.Join(" ", GetExampleEntry(autoTypeText).AutotypeTokens));
        }


        [Fact(DisplayName = "PasswordSafe: Entry: Autotype (mandatory number, single digit)")]
        public void Entry_Autotype_Tokens_MandatoryNumberOneDigit() {
            var autoTypeText = @"\W1Test";
            Assert.Equal("Wait:1000 T e s t", string.Join(" ", PwSafe.AutotypeToken.GetUnexpandedAutotypeTokens(autoTypeText)));
            Assert.Equal("Wait:1000 T e s t", string.Join(" ", GetExampleEntry(autoTypeText).AutotypeTokens));
        }

        [Fact(DisplayName = "PasswordSafe: Entry: Autotype (mandatory number, two digits)")]
        public void Entry_Autotype_Tokens_MandatoryNumberTwoDigit() {
            var autoTypeText = @"\w12Test";
            Assert.Equal("Wait:12 T e s t", string.Join(" ", PwSafe.AutotypeToken.GetUnexpandedAutotypeTokens(autoTypeText)));
            Assert.Equal("Wait:12 T e s t", string.Join(" ", GetExampleEntry(autoTypeText).AutotypeTokens));
        }

        [Fact(DisplayName = "PasswordSafe: Entry: Autotype (mandatory number, three digits)")]
        public void Entry_Autotype_Tokens_MandatoryNumberThreeDigit() {
            var autoTypeText = @"\d123Test";
            Assert.Equal("Delay:123 T e s t", string.Join(" ", PwSafe.AutotypeToken.GetUnexpandedAutotypeTokens(autoTypeText)));
            Assert.Equal("Delay:123 T e s t", string.Join(" ", GetExampleEntry(autoTypeText).AutotypeTokens));
        }

        [Fact(DisplayName = "PasswordSafe: Entry: Autotype (mandatory number, no suffix)")]
        public void Entry_Autotype_Tokens_MandatoryNumberNoSuffix() {
            var autoTypeText = @"\d12";
            Assert.Equal("Delay:12", string.Join(" ", PwSafe.AutotypeToken.GetUnexpandedAutotypeTokens(autoTypeText)));
            Assert.Equal("Delay:12", string.Join(" ", GetExampleEntry(autoTypeText).AutotypeTokens));
        }


        [Fact(DisplayName = "PasswordSafe: Entry: Autotype (example 1)")]
        public void Entry_Autotype_Tokens_Example1() {
            var autoTypeText = @"\z\u\t\p\n";
            Assert.Equal("Legacy UserName {Tab} Password {Enter}", string.Join(" ", PwSafe.AutotypeToken.GetUnexpandedAutotypeTokens(autoTypeText)));
            Assert.Equal("Legacy D e f a u l t {Tab} P a s s w 0 r d {Enter}", string.Join(" ", GetExampleEntry(autoTypeText).AutotypeTokens));
        }

        [Fact(DisplayName = "PasswordSafe: Entry: Autotype (example 2)")]
        public void Entry_Autotype_Tokens_Example2() {
            var autoTypeText = @"\i\g\l\m";
            Assert.Equal("Title Group Url Email", string.Join(" ", PwSafe.AutotypeToken.GetUnexpandedAutotypeTokens(autoTypeText)));
            Assert.Equal("E x a m p l e E x a m p l e s m e d o 6 4 . c o m t e s t @ e x a m p l e . c o m", string.Join(" ", GetExampleEntry(autoTypeText).AutotypeTokens));
        }


        #region Typos

        [Fact(DisplayName = "PasswordSafe: Entry: Autotype typo (no escape)")]
        public void Entry_Autotype_Tokens_TypoNoEscape() {
            var autoTypeText = @"\x";
            Assert.Equal("x", string.Join(" ", PwSafe.AutotypeToken.GetUnexpandedAutotypeTokens(autoTypeText)));
            Assert.Equal("x", string.Join(" ", GetExampleEntry(autoTypeText).AutotypeTokens));
        }

        [Fact(DisplayName = "PasswordSafe: Entry: Autotype typo (no escape, two char)")]
        public void Entry_Autotype_Tokens_TypoNoEscapeDouble() {
            var autoTypeText = @"\cx\p";
            Assert.Equal("c x Password", string.Join(" ", PwSafe.AutotypeToken.GetUnexpandedAutotypeTokens(autoTypeText)));
            Assert.Equal("c x P a s s w 0 r d", string.Join(" ", GetExampleEntry(autoTypeText).AutotypeTokens));
        }


        [Fact(DisplayName = "PasswordSafe: Entry: Autotype typo (no escape, hanging escape)")]
        public void Entry_Autotype_Tokens_HangingEscape() {
            var autoTypeText = @"admin\";
            Assert.Equal(@"a d m i n \", string.Join(" ", PwSafe.AutotypeToken.GetUnexpandedAutotypeTokens(autoTypeText)));
            Assert.Equal(@"a d m i n \", string.Join(" ", GetExampleEntry(autoTypeText).AutotypeTokens));
        }


        [Fact(DisplayName = "PasswordSafe: Entry: Autotype typo (optional number, too long)")]
        public void Entry_Autotype_Tokens_OptionalNumberTooLong() {
            var autoTypeText = @"\o1234";
            Assert.Equal(@"Notes:123 4", string.Join(" ", PwSafe.AutotypeToken.GetUnexpandedAutotypeTokens(autoTypeText)));
            Assert.Equal(@"4", string.Join(" ", GetExampleEntry(autoTypeText).AutotypeTokens));
        }


        [Fact(DisplayName = "PasswordSafe: Entry: Autotype typo (mandatory number, too long)")]
        public void Entry_Autotype_Tokens_MandatoryNumberTooLong() {
            var autoTypeText = @"\w1234";
            Assert.Equal(@"Wait:123 4", string.Join(" ", PwSafe.AutotypeToken.GetUnexpandedAutotypeTokens(autoTypeText)));
            Assert.Equal(@"Wait:123 4", string.Join(" ", GetExampleEntry(autoTypeText).AutotypeTokens));
        }

        [Fact(DisplayName = "PasswordSafe: Entry: Autotype typo (mandatory number, invalid number)")]
        public void Entry_Autotype_Tokens_MandatoryNumberNotPresent() {
            var autoTypeText = @"\dX";
            Assert.Equal(@"d X", string.Join(" ", PwSafe.AutotypeToken.GetUnexpandedAutotypeTokens(autoTypeText)));
            Assert.Equal(@"d X", string.Join(" ", GetExampleEntry(autoTypeText).AutotypeTokens));
        }

        [Fact(DisplayName = "PasswordSafe: Entry: Autotype typo (mandatory number, no number)")]
        public void Entry_Autotype_Tokens_MandatoryIncompleteCommand() {
            var autoTypeText = @"\W";
            Assert.Equal(@"W", string.Join(" ", PwSafe.AutotypeToken.GetUnexpandedAutotypeTokens(autoTypeText)));
            Assert.Equal(@"W", string.Join(" ", GetExampleEntry(autoTypeText).AutotypeTokens));
        }


        private static PwSafe.Entry GetExampleEntry(string autotypeText, bool amexCard = false) {
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

        #endregion

    }
}
