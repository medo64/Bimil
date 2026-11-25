namespace Bimil;

using System;
using System.Collections.Generic;
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
    private protected Field(FieldType type, ProtectedBytes data) {
        ArgumentNullException.ThrowIfNull(data);

        Type = type;
        Data = data;
        _caption = Field.GetDefaultCaption(type);
    }


    /// <summary>
    /// Gets field type.
    /// </summary>
    public FieldType Type { get; }

    /// <summary>
    /// Gets data.
    /// </summary>
    public ProtectedBytes Data { get; }

    private string _caption;
    /// <summary>
    /// Gets caption.
    /// Caption might be static and in English.
    /// </summary>
    public virtual string Caption {
        get { return _caption; }
        set { throw new NotSupportedException("Caption cannot be set."); }
    }


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
    /// Returns clone of the field.
    /// </summary>
    public abstract Field Clone();


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
            FieldType.Uuid => new UuidField(type, data),
            FieldType.Group => new TextField(type, data, isSensitive: false),
            FieldType.Title => new TextField(type, data, isSensitive: false),
            FieldType.UserName => new TextField(type, data, isSensitive: false),
            FieldType.Notes => new TextField(type, data, isSensitive: false),
            FieldType.Password => new TextField(type, data, isSensitive: true),
            FieldType.CreationTime => new TimestampField(type, data),
            FieldType.PasswordModificationTime => new TimestampField(type, data),
            FieldType.LastAccessTime => new TimestampField(type, data),
            FieldType.PasswordExpiryTime => new TimestampField(type, data),
            FieldType.LastModificationTime => new TimestampField(type, data),
            FieldType.Url => new TextField(type, data, isSensitive: false),
            FieldType.Autotype => new TextField(type, data, isSensitive: false),
            FieldType.PasswordHistory => new TextField(type, data, isSensitive: true),
            FieldType.PasswordPolicy => new TextField(type, data, isSensitive: false),
            FieldType.PasswordExpiryInterval => new TimestampField(type, data),
            FieldType.RunCommand => new TextField(type, data, isSensitive: false),
            FieldType.DoubleClickAction => new UnknownField(type, data),
            FieldType.EmailAddress => new TextField(type, data, isSensitive: false),
            FieldType.ProtectedEntry => new UnknownField(type, data),
            FieldType.OwnSymbolsForPassword => new TextField(type, data, isSensitive: false),
            FieldType.ShiftDoubleClickAction => new UnknownField(type, data),
            FieldType.PasswordPolicyName => new TextField(type, data, isSensitive: false),
            FieldType.EntryKeyboardShortcut => new UnknownField(type, data),
            FieldType.TwoFactorKey => new BinaryField(type, data),
            FieldType.CreditCardNumber => new TextField(type, data, isSensitive: true),
            FieldType.CreditCardExpiration => new TextField(type, data, isSensitive: true),
            FieldType.CreditCardVerificationValue => new TextField(type, data, isSensitive: true),
            FieldType.CreditCardPin => new TextField(type, data, isSensitive: true),
            FieldType.QRCode => new TextField(type, data, isSensitive: false),
            FieldType.TotpConfig => new UnknownField(type, data),
            FieldType.TotpLength => new UnknownField(type, data),
            FieldType.TotpTimeStep => new UnknownField(type, data),
            FieldType.TotpStartTime => new TimestampField(type, data),
            FieldType.Attachment3Title => new TextField(type, data, isSensitive: false),
            FieldType.Attachment3MediaType => new TextField(type, data, isSensitive: false),
            FieldType.Attachment3FileName => new TextField(type, data, isSensitive: false),
            FieldType.Attachment3ModificationTime => new TimestampField(type, data),
            FieldType.Attachment3Content => new BinaryField(type, data),
            FieldType.PasskeyCredentialID => new BinaryField(type, data),
            FieldType.PasskeyRelyingPartyID => new TextField(type, data, isSensitive: false),
            FieldType.PasskeyUserHandle => new BinaryField(type, data),
            FieldType.PasskeyAlgorithmID => new UnknownField(type, data),
            FieldType.PasskeyPrivateKey => new BinaryField(type, data),
            FieldType.PasskeySignCount => new UnknownField(type, data),
            FieldType.CustomTextField => new CustomTextField(type, data) { Caption = Field.GetDefaultCaption(type) },
            FieldType.BaseUuid => new UuidField(type, data),
            FieldType.AliasUuid => new UuidField(type, data),
            FieldType.ShortcutUuid => new UuidField(type, data),
            FieldType.Attachment4Uuid => new UuidField(type, data),
            FieldType.Attachment4Title => new TextField(type, data, isSensitive: false),
            FieldType.Attachment4CreationTime => new TimestampField(type, data),
            FieldType.Attachment4MediaType => new TextField(type, data, isSensitive: false),
            FieldType.Attachment4FileName => new TextField(type, data, isSensitive: false),
            FieldType.Attachment4FilePath => new TextField(type, data, isSensitive: false),
            FieldType.Attachment4FileCreationTime => new TimestampField(type, data),
            FieldType.Attachment4FileModificationTime => new TimestampField(type, data),
            FieldType.Attachment4FileAccessTime => new TimestampField(type, data),
            FieldType.Attachment4EK => new UnknownField(type, data),
            FieldType.Attachment4AK => new UnknownField(type, data),
            FieldType.Attachment4IV => new UnknownField(type, data),
            FieldType.Attachment4Content => new BinaryField(type, data),
            FieldType.Attachment4ContentHMAC => new UnknownField(type, data),
            FieldType.EndOfEntry => throw new NotSupportedException("Cannot create EndOfEntry field."),
            _ => new UnknownField(type, data),
        };
    }

    /// <summary>
    /// Returns default caption for a field type.
    /// </summary>
    /// <param name="type">Type.</param>
    internal static string GetDefaultCaption(FieldType type) {
        return type switch {
            FieldType.Uuid => "UUID",
            FieldType.Group => "Group",
            FieldType.Title => "Title",
            FieldType.UserName => "User name",
            FieldType.Notes => "Notes",
            FieldType.Password => "Password",
            FieldType.CreationTime => "Creation time",
            FieldType.PasswordModificationTime => "Password modification time",
            FieldType.LastAccessTime => "Last access time",
            FieldType.PasswordExpiryTime => "Password expiry time",
            FieldType.LastModificationTime => "Last modification time",
            FieldType.Url => "URL",
            FieldType.Autotype => "Autotype",
            FieldType.PasswordHistory => "Password history",
            FieldType.PasswordPolicy => "Password policy",
            FieldType.PasswordExpiryInterval => "Password expiry interval",
            FieldType.RunCommand => "Run command",
            FieldType.DoubleClickAction => "Double-click action",
            FieldType.EmailAddress => "EMail address",
            FieldType.ProtectedEntry => "Protected entry",
            FieldType.OwnSymbolsForPassword => "Own symbols for password",
            FieldType.ShiftDoubleClickAction => "Shift double-click action",
            FieldType.PasswordPolicyName => "Password policy name",
            FieldType.EntryKeyboardShortcut => "Entry keyboard shortcut",
            FieldType.TwoFactorKey => "Two-factor key",
            FieldType.CreditCardNumber => "Credit card number",
            FieldType.CreditCardExpiration => "Credit card expiration",
            FieldType.CreditCardVerificationValue => "Credit card verification value",
            FieldType.CreditCardPin => "Credit card PIN",
            FieldType.QRCode => "QR code",
            FieldType.TotpConfig => "TOTP config",
            FieldType.TotpLength => "TOTP length",
            FieldType.TotpTimeStep => "TOTP time step",
            FieldType.TotpStartTime => "TOTP start time",
            FieldType.Attachment3Title => "Attachment title",
            FieldType.Attachment3MediaType => "Attachment media type",
            FieldType.Attachment3FileName => "Attachment file name",
            FieldType.Attachment3ModificationTime => "Attachment modification time",
            FieldType.Attachment3Content => "Attachment content",
            FieldType.PasskeyCredentialID => "Passkey credential ID",
            FieldType.PasskeyRelyingPartyID => "Passkey relying party ID",
            FieldType.PasskeyUserHandle => "Passkey user handle",
            FieldType.PasskeyAlgorithmID => "Passkey algorithm ID",
            FieldType.PasskeyPrivateKey => "Passkey private key",
            FieldType.PasskeySignCount => "Passkey sign count",
            FieldType.CustomTextField => "Custom text field",
            FieldType.BaseUuid => "Base UUID",
            FieldType.AliasUuid => "Alias UUID",
            FieldType.ShortcutUuid => "Shortcut UUID",
            FieldType.Attachment4Uuid => "Attachment UUID",
            FieldType.Attachment4Title => "Attachment title",
            FieldType.Attachment4CreationTime => "Attachment creation time",
            FieldType.Attachment4MediaType => "Attachment media type",
            FieldType.Attachment4FileName => "Attachment file name",
            FieldType.Attachment4FilePath => "Attachment file path",
            FieldType.Attachment4FileCreationTime => "Attachment file creation time",
            FieldType.Attachment4FileModificationTime => "Attachment file modification time",
            FieldType.Attachment4FileAccessTime => "Attachment file access time",
            FieldType.Attachment4EK => "Attachment encryption key",
            FieldType.Attachment4AK => "Attachment authentication key",
            FieldType.Attachment4IV => "Attachment initialization vector",
            FieldType.Attachment4Content => "Attachment content",
            FieldType.Attachment4ContentHMAC => "Attachment content HMAC",
            FieldType.EndOfEntry => "End of entry",
            _ => string.Empty,
        };
    }

}
