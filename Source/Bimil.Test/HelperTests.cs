using Medo.Security.Cryptography.PasswordSafe;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;

namespace Bimil.Test {
    [TestClass]
    public class HelperTests {

        [TestMethod]
        public void GetAutoTypeTokens_Default() {
            var tokens = Helpers.GetAutoTypeTokens(null);
            Assert.AreEqual(@"\u \t \p \t \n", string.Join(" ", tokens));
        }

        [TestMethod]
        public void GetAutoTypeTokens_Default_WithEntry() {
            var tokens = Helpers.GetAutoTypeTokens(null, ExampleEntry);
            Assert.AreEqual(@"User {Tab} Password {Tab} {Enter}", string.Join(" ", tokens));
        }


        [TestMethod]
        public void GetAutoTypeTokens_TwoFactor() {
            var tokens = Helpers.GetAutoTypeTokens(@"\u\t\p\t\2\t\n");
            Assert.AreEqual(@"\u \t \p \t \2 \t \n", string.Join(" ", tokens));
        }


        [TestMethod]
        public void GetAutoTypeTokens_SomeText1() {
            var tokens = Helpers.GetAutoTypeTokens(@"admin\n\p\n");
            Assert.AreEqual(@"admin \n \p \n", string.Join(" ", tokens));
        }

        [TestMethod]
        public void GetAutoTypeTokens_SomeText1_WithEntry() {
            var tokens = Helpers.GetAutoTypeTokens(@"admin\n\p\n", ExampleEntry);
            Assert.AreEqual(@"admin {Enter} Password {Enter}", string.Join(" ", tokens));
        }


        [TestMethod]
        public void GetAutoTypeTokens_SomeText2() {
            var tokens = Helpers.GetAutoTypeTokens(@"\badmin\n\p\n");
            Assert.AreEqual(@"\b admin \n \p \n", string.Join(" ", tokens));
        }

        [TestMethod]
        public void GetAutoTypeTokens_SomeText2_WithEntry() {
            var tokens = Helpers.GetAutoTypeTokens(@"\badmin\n\p\n", ExampleEntry);
            Assert.AreEqual(@"{Backspace} admin {Enter} Password {Enter}", string.Join(" ", tokens));
        }


        [TestMethod]
        public void GetAutoTypeTokens_SomeText3() {
            var tokens = Helpers.GetAutoTypeTokens(@"admin\n\p\nXXX");
            Assert.AreEqual(@"admin \n \p \n XXX", string.Join(" ", tokens));
        }

        [TestMethod]
        public void GetAutoTypeTokens_SomeText3_WithEntry() {
            var tokens = Helpers.GetAutoTypeTokens(@"admin\n\p\nXXX", ExampleEntry);
            Assert.AreEqual(@"admin {Enter} Password {Enter} XXX", string.Join(" ", tokens));
        }


        [TestMethod]
        public void GetAutoTypeTokens_CreditCard() {
            var tokens = Helpers.GetAutoTypeTokens(@"\cn\t\ce\t\cv\n");
            Assert.AreEqual(@"\cn \t \ce \t \cv \n", string.Join(" ", tokens));
        }

        [TestMethod]
        public void GetAutoTypeTokens_CreditCard_WithEntry() {
            var tokens = Helpers.GetAutoTypeTokens(@"\cn\t\ce\t\cv\n", ExampleEntry);
            Assert.AreEqual(@"1234567890123456 {Tab} 01/79 {Tab} 123 {Enter}", string.Join(" ", tokens));
        }


        [TestMethod]
        public void GetAutoTypeTokens_OptionalNumberNotUsed() {
            var tokens = Helpers.GetAutoTypeTokens(@"\oTest");
            Assert.AreEqual(@"\o Test", string.Join(" ", tokens));
        }

        [TestMethod]
        public void GetAutoTypeTokens_OptionalNumberNotUsed_WithEntry() {
            var tokens = Helpers.GetAutoTypeTokens(@"\oTest", ExampleEntry);
            Assert.AreEqual(@"1{Enter}2{Enter}3{Enter}{^}{Enter} Test", string.Join(" ", tokens));
        }


        [TestMethod]
        public void GetAutoTypeTokens_OptionalNumberOneDigit() {
            var tokens = Helpers.GetAutoTypeTokens(@"\o1Test");
            Assert.AreEqual(@"\o1 Test", string.Join(" ", tokens));
        }

        [TestMethod]
        public void GetAutoTypeTokens_OptionalNumberOneDigit_WithEntry_Line1() {
            var tokens = Helpers.GetAutoTypeTokens(@"\o1Test", ExampleEntry);
            Assert.AreEqual(@"1 Test", string.Join(" ", tokens));
        }

        [TestMethod]
        public void GetAutoTypeTokens_OptionalNumberOneDigit_WithEntry_Line2() {
            var tokens = Helpers.GetAutoTypeTokens(@"\o2Test", ExampleEntry);
            Assert.AreEqual(@"2 Test", string.Join(" ", tokens));
        }

        [TestMethod]
        public void GetAutoTypeTokens_OptionalNumberOneDigit_WithEntry_Line3() {
            var tokens = Helpers.GetAutoTypeTokens(@"\o3Test", ExampleEntry);
            Assert.AreEqual(@"3 Test", string.Join(" ", tokens));
        }

        [TestMethod]
        public void GetAutoTypeTokens_OptionalNumberOneDigit_WithEntry_Line4() {
            var tokens = Helpers.GetAutoTypeTokens(@"\o4Test", ExampleEntry);
            Assert.AreEqual(@"{^} Test", string.Join(" ", tokens));
        }

        [TestMethod]
        public void GetAutoTypeTokens_OptionalNumberOneDigit_WithEntry_Line5() {
            var tokens = Helpers.GetAutoTypeTokens(@"\o5Test", ExampleEntry);
            Assert.AreEqual(@"Test", string.Join(" ", tokens));
        }


        [TestMethod]
        public void GetAutoTypeTokens_OptionalNumberTwoDigit() {
            var tokens = Helpers.GetAutoTypeTokens(@"\o12Test");
            Assert.AreEqual(@"\o12 Test", string.Join(" ", tokens));
        }

        [TestMethod]
        public void GetAutoTypeTokens_OptionalNumberTwoDigit_WithEntry() {
            var tokens = Helpers.GetAutoTypeTokens(@"\o12Test", ExampleEntry);
            Assert.AreEqual(@"Test", string.Join(" ", tokens));
        }


        [TestMethod]
        public void GetAutoTypeTokens_OptionalNumberThreeDigit() {
            var tokens = Helpers.GetAutoTypeTokens(@"\o123Test");
            Assert.AreEqual(@"\o123 Test", string.Join(" ", tokens));
        }

        [TestMethod]
        public void GetAutoTypeTokens_OptionalNumberThreeDigit_WithEntry() {
            var tokens = Helpers.GetAutoTypeTokens(@"\o123Test", ExampleEntry);
            Assert.AreEqual(@"Test", string.Join(" ", tokens));
        }


        [TestMethod]
        public void GetAutoTypeTokens_OptionalNumberNoSuffix() {
            var tokens = Helpers.GetAutoTypeTokens(@"\o12");
            Assert.AreEqual(@"\o12", string.Join(" ", tokens));
        }

        [TestMethod]
        public void GetAutoTypeTokens_OptionalNumberNoSuffix_WithEntry() {
            var tokens = Helpers.GetAutoTypeTokens(@"\o12", ExampleEntry);
            Assert.AreEqual(@"", string.Join(" ", tokens));
        }


        [TestMethod]
        public void GetAutoTypeTokens_MandatoryNumberOneDigit() {
            var tokens = Helpers.GetAutoTypeTokens(@"\W1Test");
            Assert.AreEqual(@"\W1 Test", string.Join(" ", tokens));
        }

        [TestMethod]
        public void GetAutoTypeTokens_MandatoryNumberOneDigit_WithEntry() {
            var tokens = Helpers.GetAutoTypeTokens(@"\W1Test", ExampleEntry);
            Assert.AreEqual(@"*Wait:1000* Test", string.Join(" ", tokens));
        }


        [TestMethod]
        public void GetAutoTypeTokens_MandatoryNumberTwoDigit() {
            var tokens = Helpers.GetAutoTypeTokens(@"\w12Test");
            Assert.AreEqual(@"\w12 Test", string.Join(" ", tokens));
        }

        [TestMethod]
        public void GetAutoTypeTokens_MandatoryNumberTwoDigit_WithEntry() {
            var tokens = Helpers.GetAutoTypeTokens(@"\w12Test", ExampleEntry);
            Assert.AreEqual(@"*Wait:12* Test", string.Join(" ", tokens));
        }


        [TestMethod]
        public void GetAutoTypeTokens_MandatoryNumberThreeDigit() {
            var tokens = Helpers.GetAutoTypeTokens(@"\d123Test");
            Assert.AreEqual(@"\d123 Test", string.Join(" ", tokens));
        }

        [TestMethod]
        public void GetAutoTypeTokens_MandatoryNumberThreeDigit_WithEntry() {
            var tokens = Helpers.GetAutoTypeTokens(@"\d123Test", ExampleEntry);
            Assert.AreEqual(@"*Delay:123* Test", string.Join(" ", tokens));
        }


        [TestMethod]
        public void GetAutoTypeTokens_MandatoryNumberNoSuffix() {
            var tokens = Helpers.GetAutoTypeTokens(@"\d12");
            Assert.AreEqual(@"\d12", string.Join(" ", tokens));
        }

        [TestMethod]
        public void GetAutoTypeTokens_MandatoryNumberNoSuffix_WithEntry() {
            var tokens = Helpers.GetAutoTypeTokens(@"\d12", ExampleEntry);
            Assert.AreEqual(@"*Delay:12*", string.Join(" ", tokens));
        }


        [TestMethod]
        public void GetAutoTypeTokens_Example1() {
            var tokens = Helpers.GetAutoTypeTokens(@"\z\u\t\p\n");
            Assert.AreEqual(@"\z \u \t \p \n", string.Join(" ", tokens));
        }

        [TestMethod]
        public void GetAutoTypeTokens_Example1_WithEntry() {
            var tokens = Helpers.GetAutoTypeTokens(@"\z\u\t\p\n", ExampleEntry);
            Assert.AreEqual(@"*CopyPaste* User {Tab} Password {Enter}", string.Join(" ", tokens));
        }


        [TestMethod]
        public void GetAutoTypeTokens_Example2() {
            var tokens = Helpers.GetAutoTypeTokens(@"\2");
            Assert.AreEqual(@"\2", string.Join(" ", tokens));
        }

        [TestMethod]
        public void GetAutoTypeTokens_Example2_WithEntry() {
            var tokens = Helpers.GetAutoTypeTokens(@"\2", ExampleEntry);
            Assert.IsTrue(Regex.IsMatch(string.Join(" ", tokens), @"\d\d\d\d\d\d"));
        }


        [TestMethod]
        public void GetAutoTypeTokens_Example3() {
            var tokens = Helpers.GetAutoTypeTokens(@"\i\g\l\m");
            Assert.AreEqual(@"\i \g \l \m", string.Join(" ", tokens));
        }

        [TestMethod]
        public void GetAutoTypeTokens_Example3_WithEntry() {
            var tokens = Helpers.GetAutoTypeTokens(@"\i\g\l\m", ExampleEntry);
            Assert.AreEqual(@"Title Group medo64.com test@example.com", string.Join(" ", tokens));
        }


        #region Typos

        [TestMethod]
        public void GetAutoTypeTokens_TypoNoEscape() {
            var tokens = Helpers.GetAutoTypeTokens(@"\x");
            Assert.AreEqual(@"x", string.Join(" ", tokens));
        }

        [TestMethod]
        public void GetAutoTypeTokens_TypoNoEscape_WithEntry() {
            var tokens = Helpers.GetAutoTypeTokens(@"\x", ExampleEntry);
            Assert.AreEqual(@"x", string.Join(" ", tokens));
        }


        [TestMethod]
        public void GetAutoTypeTokens_TypoNoEscapeDouble() {
            var tokens = Helpers.GetAutoTypeTokens(@"\cx\p");
            Assert.AreEqual(@"cx \p", string.Join(" ", tokens));
        }

        [TestMethod]
        public void GetAutoTypeTokens_TypoNoEscapeDouble_WithEntry() {
            var tokens = Helpers.GetAutoTypeTokens(@"\cx\p", ExampleEntry);
            Assert.AreEqual(@"cx Password", string.Join(" ", tokens));
        }


        [TestMethod]
        public void GetAutoTypeTokens_HangingEscape() {
            var tokens = Helpers.GetAutoTypeTokens(@"admin\");
            Assert.AreEqual(@"admin", string.Join(" ", tokens));
        }

        [TestMethod]
        public void GetAutoTypeTokens_HangingEscape_WithEntry() {
            var tokens = Helpers.GetAutoTypeTokens(@"admin\", ExampleEntry);
            Assert.AreEqual(@"admin", string.Join(" ", tokens));
        }


        [TestMethod]
        public void GetAutoTypeTokens_OptionalNumberTooLong() {
            var tokens = Helpers.GetAutoTypeTokens(@"\o1234");
            Assert.AreEqual(@"\o123 4", string.Join(" ", tokens));
        }

        [TestMethod]
        public void GetAutoTypeTokens_OptionalNumberTooLong_WithEntry() {
            var tokens = Helpers.GetAutoTypeTokens(@"\o1234", ExampleEntry);
            Assert.AreEqual(@"4", string.Join(" ", tokens));
        }


        [TestMethod]
        public void GetAutoTypeTokens_MandatoryNumberTooLong() {
            var tokens = Helpers.GetAutoTypeTokens(@"\w1234");
            Assert.AreEqual(@"\w123 4", string.Join(" ", tokens));
        }

        [TestMethod]
        public void GetAutoTypeTokens_MandatoryNumberTooLong_WithEntry() {
            var tokens = Helpers.GetAutoTypeTokens(@"\w1234", ExampleEntry);
            Assert.AreEqual(@"*Wait:123* 4", string.Join(" ", tokens));
        }


        [TestMethod]
        public void GetAutoTypeTokens_MandatoryNumberNotPresent() {
            var tokens = Helpers.GetAutoTypeTokens(@"\dX");
            Assert.AreEqual(@"dX", string.Join(" ", tokens));
        }

        [TestMethod]
        public void GetAutoTypeTokens_MandatoryNumberNotPresent_WithEntry() {
            var tokens = Helpers.GetAutoTypeTokens(@"\dX", ExampleEntry);
            Assert.AreEqual(@"dX", string.Join(" ", tokens));
        }


        [TestMethod]
        public void GetAutoTypeTokens_MandatoryIncompleteCommand() {
            var tokens = Helpers.GetAutoTypeTokens(@"\W");
            Assert.AreEqual(@"W", string.Join(" ", tokens));
        }

        [TestMethod]
        public void GetAutoTypeTokens_MandatoryIncompleteCommand_WithEntry() {
            var tokens = Helpers.GetAutoTypeTokens(@"\W", ExampleEntry);
            Assert.AreEqual(@"W", string.Join(" ", tokens));
        }

        #endregion



        private static Entry ExampleEntry = new Entry() {
            Title = "Title",
            Group = "Group",
            UserName = "User",
            Password = "Password",
            CreditCardNumber = "1234567890123456",
            CreditCardExpiration = "01/79",
            CreditCardVerificationValue = "123",
            CreditCardPin = "1234",
            TwoFactorKey = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
            Email = "test@example.com",
            Url = "medo64.com",
            Notes = "1\r\n2\n3\r^\n",
        };

    }
}
