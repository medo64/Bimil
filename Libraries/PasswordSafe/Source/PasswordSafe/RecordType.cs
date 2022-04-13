using System;

namespace Medo.Security.Cryptography.PasswordSafe {
    /// <summary>
    /// Record field types.
    /// </summary>
    public enum RecordType {
        /// <summary>
        /// UUID.
        /// </summary>
        Uuid = 0x01,
        /// <summary>
        /// Group.
        /// </summary>
        Group = 0x02,
        /// <summary>
        /// Title.
        /// </summary>
        Title = 0x03,
        /// <summary>
        /// User name.
        /// </summary>
        UserName = 0x04,
        /// <summary>
        /// Notes.
        /// </summary>
        Notes = 0x05,
        /// <summary>
        /// Password.
        /// </summary>
        Password = 0x06,
        /// <summary>
        /// Creation time.
        /// </summary>
        CreationTime = 0x07,
        /// <summary>
        /// Password modification time.
        /// </summary>
        PasswordModificationTime = 0x08,
        /// <summary>
        /// Last access time.
        /// </summary>
        LastAccessTime = 0x09,
        /// <summary>
        /// Password expiry time.
        /// </summary>
        PasswordExpiryTime = 0x0a,
        /// <summary>
        /// Last modification time.
        /// </summary>
        LastModificationTime = 0x0c,
        /// <summary>
        /// URL.
        /// </summary>
        Url = 0x0d,
        /// <summary>
        /// Autotype.
        /// </summary>
        Autotype = 0x0e,
        /// <summary>
        /// Password history.
        /// </summary>
        PasswordHistory = 0x0f,
        /// <summary>
        /// Password policy.
        /// </summary>
        PasswordPolicy = 0x10,
        /// <summary>
        /// Password expiry interval.
        /// </summary>
        PasswordExpiryInterval = 0x11,
        /// <summary>
        /// Run command.
        /// </summary>
        RunCommand = 0x12,
        /// <summary>
        /// Double-click action.
        /// </summary>
        DoubleClickAction = 0x13,
        /// <summary>
        /// E-mail address.
        /// </summary>
        EmailAddress = 0x14,
        /// <summary>
        /// Protected entry.
        /// </summary>
        ProtectedEntry = 0x15,
        /// <summary>
        /// Own symbols for password.
        /// </summary>
        OwnSymbolsForPassword = 0x16,
        /// <summary>
        /// Shift double-click action.
        /// </summary>
        ShiftDoubleClickAction = 0x17,
        /// <summary>
        /// Password policy name.
        /// </summary>
        PasswordPolicyName = 0x18,
        /// <summary>
        /// Entry keyboard shortcut.
        /// </summary>
        EntryKeyboardShortcut = 0x19,

        /// <summary>
        /// Two-factor authentication key.
        /// This is the shared secret for sites using Time-Based One-Time Password Algorithm (per RFC6238) such as Google Authenticator. At least 10 bytes.
        /// </summary>
        TwoFactorKey = 0x1b,
        /// <summary>
        /// Credit card number.
        /// Number should consist of digits and spaces.
        /// </summary>
        CreditCardNumber = 0x1c,
        /// <summary>
        /// Credit card expiration.
        /// Expiration should be MM/YY, where MM is 01-12, and YY 00-99.
        /// </summary>
        CreditCardExpiration = 0x1d,
        /// <summary>
        /// Credit card verification value.
        /// CVV (CVV2) is three or four digits.
        /// </summary>
        CreditCardVerificationValue = 0x1e,
        /// <summary>
        /// Credit card PIN.
        /// PIN is four to twelve digits long (ISO-9564).
        /// </summary>
        CreditCardPin = 0x1f,

        /// <summary>
        /// UTF-8 encoded text used for QR code generation.
        /// </summary>
        QRCode = 0x20,

        /// <summary>
        /// End of entries.
        /// </summary>
        EndOfEntry = 0xFF,
    }
}
