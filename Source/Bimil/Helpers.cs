using Medo.Security.Cryptography.PasswordSafe;
using System.Windows.Forms;

namespace Bimil {
    internal static class Helpers {

        #region ToolStripBorderlessProfessionalRenderer

        internal static ToolStripBorderlessProfessionalRenderer ToolStripBorderlessSystemRendererInstance { get { return new ToolStripBorderlessProfessionalRenderer(); } }

        internal class ToolStripBorderlessProfessionalRenderer : ToolStripProfessionalRenderer {

            protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e) {
            }

        }

        #endregion


        public static string GetRecordCaption(Record record) {
            return GetRecordCaption(record.RecordType);
        }

        public static string GetRecordCaption(RecordType recordType) {
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

                case RecordType.TemporaryBimilTwoFactorKey: return "Two-factor key*";
                case RecordType.TemporaryBimilCreditCardNumber: return "Card number*";
                case RecordType.TemporaryBimilCreditCardExpiration: return "Card expiration*";
                case RecordType.TemporaryBimilCreditCardSecurityCode: return "Card security code*";
                case RecordType.TemporaryBimilCreditCardPin: return "Card PIN*";

                default: return null; //all other fields are not really supported
            }
        }

    }
}
