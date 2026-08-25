namespace Bimil;

/// <summary>
/// Record field types.
/// </summary>
public enum FieldType : byte {

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
    /// TOTP config.
    /// </summary>
    TotpConfig = 0x21,

    /// <summary>
    /// TOTP length.
    /// </summary>
    TotpLength = 0x22,

    /// <summary>
    /// TOTP time step.
    /// </summary>
    TotpTimeStep = 0x23,

    /// <summary>
    /// TOTP start time.
    /// </summary>
    TotpStartTime = 0x24,

    /// <summary>
    /// Attachment title.
    /// </summary>
    Attachment3Title = 0x25,

    /// <summary>
    /// Attachment media type.
    /// </summary>
    Attachment3MediaType = 0x26,

    /// <summary>
    /// Attachment file name.
    /// </summary>
    Attachment3FileName = 0x27,

    /// <summary>
    /// Attachment modification time.
    /// </summary>
    Attachment3ModificationTime = 0x28,

    /// <summary>
    /// Attachment content.
    /// </summary>
    Attachment3Content = 0x29,

    /// <summary>
    /// Passkey credential ID.
    /// </summary>
    PasskeyCredentialID = 0x2a,

    /// <summary>
    /// Passkey relying party ID.
    /// </summary>
    PasskeyRelyingPartyID = 0x2b,

    /// <summary>
    /// Passkey user handle.
    /// </summary>
    PasskeyUserHandle = 0x2c,

    /// <summary>
    /// Passkey algorithm ID.
    /// </summary>
    PasskeyAlgorithmID = 0x2d,

    /// <summary>
    /// Passkey private key.
    /// </summary>
    PasskeyPrivateKey = 0x2e,

    /// <summary>
    /// Passkey sign count.
    /// </summary>
    PasskeySignCount = 0x2f,

    /// <summary>
    /// Custom fields consist of property entries with property name, value, and sensitivity data.
    /// </summary>
    CustomTextField = 0x30,

    /// <summary>
    /// Base UUID.
    /// </summary>
    BaseUuid = 0x41,

    /// <summary>
    /// Alias UUID.
    /// </summary>
    AliasUuid = 0x42,

    /// <summary>
    /// Shortcut UUID.
    /// </summary>
    ShortcutUuid = 0x43,

    /// <summary>
    /// Attachment UUID.
    /// </summary>
    Attachment4Uuid = 0x60,

    /// <summary>
    /// Attachment title.
    /// </summary>
    Attachment4Title = 0x61,
    /// <summary>
    /// Attachment creation time.
    /// </summary>
    Attachment4CreationTime = 0x62,

    /// <summary>
    /// Attachment media type.
    /// </summary>
    Attachment4MediaType = 0x63,

    /// <summary>
    /// Attachment file name.
    /// </summary>
    Attachment4FileName = 0x64,

    /// <summary>
    /// Attachment file path.
    /// </summary>
    Attachment4FilePath = 0x65,

    /// <summary>
    /// Attachment file creation time.
    /// </summary>
    Attachment4FileCreationTime = 0x66,

    /// <summary>
    /// Attachment file modification time.
    /// </summary>
    Attachment4FileModificationTime = 0x67,

    /// <summary>
    /// Attachment file access time.
    /// </summary>
    Attachment4FileAccessTime = 0x68,

    /// <summary>
    /// Attachment encryption key.
    /// </summary>
    Attachment4EK = 0x70,

    /// <summary>
    /// Attachment authentication key.
    /// </summary>
    Attachment4AK = 0x71,

    /// <summary>
    /// Attachment initialization vector.
    /// </summary>
    Attachment4IV = 0x72,

    /// <summary>
    /// Attachment content.
    /// </summary>
    Attachment4Content = 0x73,

    /// <summary>
    /// Attachment content HMAC.
    /// </summary>
    Attachment4ContentHMAC = 0x74,


    /// <summary>
    /// End of entries.
    /// </summary>
    EndOfEntry = 0xFF,

}
