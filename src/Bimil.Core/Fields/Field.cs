namespace Bimil;

using System;
using System.Runtime.InteropServices;

/// <summary>
/// Record field.
/// </summary>
public abstract class Field : IReadableCaption {

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="type">Type.</param>
    /// <param name="data">Data.</param>
    /// <param name="caption">Caption.</param>
    private protected Field(FieldType type, ProtectedBytes data, string caption) {
        ArgumentNullException.ThrowIfNull(data);
        ArgumentNullException.ThrowIfNull(caption);

        Type = type;
        Data = data;
        Caption = caption;
    }


    /// <summary>
    /// Gets field type.
    /// </summary>
    public FieldType Type { get; }

    /// <summary>
    /// Gets data.
    /// </summary>
    public ProtectedBytes Data { get; }

    /// <summary>
    /// Gets caption.
    /// Caption might be static and in English.
    /// </summary>
    public string Caption { get; }


    /// <summary>
    /// Gets if values are read-only.
    /// </summary>
    public virtual bool IsReadOnly {
        get;
        internal set {
            field = value;
            Data.IsReadOnly = value;
        }
    }


    /// <summary>
    /// Returns record field based on type.
    /// </summary>
    /// <param name="type">Type.</param>
    public static Field Create(FieldType type) {
        return Create(type, new ProtectedBytes());
    }

    /// <summary>
    /// Returns record field based on type.
    /// </summary>
    /// <param name="type">Type.</param>
    /// <param name="bytes">Bytes.</param>
    public static Field Create(FieldType type, byte[] bytes) {
        return Create(type, new ProtectedBytes(bytes));
    }

    /// <summary>
    /// Returns record field based on type.
    /// </summary>
    /// <param name="type">Type.</param>
    /// <param name="bytes">Bytes.</param>
    /// <param name="zeroBytes">If true, input bytes will be zeroed after protection.</param>
    public static Field Create(FieldType type, byte[] bytes, bool zeroBytes) {
        return Create(type, new ProtectedBytes(bytes, zeroBytes));
    }


    /// <summary>
    /// Returns record field based on type.
    /// </summary>
    /// <param name="type">Type.</param>
    /// <param name="data">Data.</param>
    internal static Field Create(FieldType type, ProtectedBytes data) {
        return type switch {
            FieldType.Uuid => new UuidField(type, data, "UUID"),
            FieldType.Group => new TextField(type, data, "Group", isSensitive: false),
            FieldType.Title => new TextField(type, data, "Title", isSensitive: false),
            FieldType.UserName => new TextField(type, data, "Username", isSensitive: false),
            FieldType.Notes => new TextField(type, data, "Notes", isSensitive: false),
            FieldType.Password => new TextField(type, data, "Password", isSensitive: true),
            FieldType.CreationTime => new TimestampField(type, data, "Creation time"),
            FieldType.PasswordModificationTime => new TimestampField(type, data, "Password modification time"),
            FieldType.LastAccessTime => new TimestampField(type, data, "Last access time"),
            FieldType.PasswordExpiryTime => new TimestampField(type, data, "Password expiry time"),
            FieldType.LastModificationTime => new TimestampField(type, data, "Last modification time"),
            FieldType.Url => new TextField(type, data, "URL", isSensitive: false),
            FieldType.Autotype => new TextField(type, data, "Autotype", isSensitive: false),
            FieldType.PasswordHistory => new TextField(type, data, "Password history", isSensitive: true),
            FieldType.PasswordPolicy => new TextField(type, data, "Password policy", isSensitive: false),
            FieldType.PasswordExpiryInterval => new TimestampField(type, data, "Password expiry interval"),
            FieldType.RunCommand => new TextField(type, data, "Run command", isSensitive: false),
            FieldType.DoubleClickAction => new UnknownField(type, data, "Double-click action"),
            FieldType.EmailAddress => new TextField(type, data, "EMail address", isSensitive: false),
            FieldType.ProtectedEntry => new UnknownField(type, data, "Protected entry"),
            FieldType.OwnSymbolsForPassword => new TextField(type, data, "Own symbols for password", isSensitive: false),
            FieldType.ShiftDoubleClickAction => new UnknownField(type, data, "Shift double-click action"),
            FieldType.PasswordPolicyName => new TextField(type, data, "Password policy name", isSensitive: false),
            FieldType.EntryKeyboardShortcut => new UnknownField(type, data, "Entry keyboard shortcut"),
            FieldType.TwoFactorKey => new BinaryField(type, data, "Two-factor key"),
            FieldType.CreditCardNumber => new TextField(type, data, "Credit card number", isSensitive: true),
            FieldType.CreditCardExpiration => new TextField(type, data, "Credit card expiration", isSensitive: true),
            FieldType.CreditCardVerificationValue => new TextField(type, data, "Credit card verification value", isSensitive: true),
            FieldType.CreditCardPin => new TextField(type, data, "Credit card PIN", isSensitive: true),
            FieldType.QRCode => new TextField(type, data, "QR code", isSensitive: false),
            FieldType.TotpConfig => new UnknownField(type, data, "TOTP config"),
            FieldType.TotpLength => new UnknownField(type, data, "TOTP length"),
            FieldType.TotpTimeStep => new UnknownField(type, data, "TOTP time step"),
            FieldType.TotpStartTime => new TimestampField(type, data, "TOTP start time"),
            FieldType.Attachment3Title => new TextField(type, data, "Attachment title", isSensitive: false),
            FieldType.Attachment3MediaType => new TextField(type, data, "Attachment media type", isSensitive: false),
            FieldType.Attachment3FileName => new TextField(type, data, "Attachment file name", isSensitive: false),
            FieldType.Attachment3ModificationTime => new TimestampField(type, data, "Attachment modification time"),
            FieldType.Attachment3Content => new BinaryField(type, data, "Attachment content"),
            FieldType.PasskeyCredentialID => new BinaryField(type, data, "Passkey credential ID"),
            FieldType.PasskeyRelyingPartyID => new TextField(type, data, "Passkey relying party ID", isSensitive: false),
            FieldType.PasskeyUserHandle => new BinaryField(type, data, "Passkey user handle"),
            FieldType.PasskeyAlgorithmID => new UnknownField(type, data, "Passkey algorithm ID"),
            FieldType.PasskeyPrivateKey => new BinaryField(type, data, "Passkey private key"),
            FieldType.PasskeySignCount => new UnknownField(type, data, "Passkey sign count"),
            FieldType.CustomTextField => new CustomTextField(type, data),
            FieldType.BaseUuid => new UuidField(type, data, "Base UUID"),
            FieldType.AliasUuid => new UuidField(type, data, "Alias UUID"),
            FieldType.ShortcutUuid => new UuidField(type, data, "Shortcut UUID"),
            FieldType.Attachment4Uuid => new UuidField(type, data, "Attachment UUID"),
            FieldType.Attachment4Title => new TextField(type, data, "Attachment title", isSensitive: false),
            FieldType.Attachment4CreationTime => new TimestampField(type, data, "Attachment creation time"),
            FieldType.Attachment4MediaType => new TextField(type, data, "Attachment media type", isSensitive: false),
            FieldType.Attachment4FileName => new TextField(type, data, "Attachment file name", isSensitive: false),
            FieldType.Attachment4FilePath => new TextField(type, data, "Attachment file path", isSensitive: false),
            FieldType.Attachment4FileCreationTime => new TimestampField(type, data, "Attachment file creation time"),
            FieldType.Attachment4FileModificationTime => new TimestampField(type, data, "Attachment file modification time"),
            FieldType.Attachment4FileAccessTime => new TimestampField(type, data, "Attachment file access time"),
            FieldType.Attachment4EK => new UnknownField(type, data, "Attachment encryption key"),
            FieldType.Attachment4AK => new UnknownField(type, data, "Attachment authentication key"),
            FieldType.Attachment4IV => new UnknownField(type, data, "Attachment initialization vector"),
            FieldType.Attachment4Content => new BinaryField(type, data, "Attachment content"),
            FieldType.Attachment4ContentHMAC => new UnknownField(type, data, "Attachment content HMAC"),
            FieldType.EndOfEntry => throw new NotSupportedException("Cannot create EndOfEntry field."),
            _ => new UnknownField(type, data, string.Empty),
        };
    }

}
