namespace Bimil;

using System;

/// <summary>
/// Record field.
/// </summary>
public abstract class Field {

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
    /// Returns record field based on type.
    /// </summary>
    /// <param name="type">Type.</param>
    internal static Field Create(FieldType type) {
        return Create(type, new ProtectedBytes());
    }

    /// <summary>
    /// Returns record field based on type.
    /// </summary>    
    /// <param name="type">Type.</param>
    /// <param name="data">Data.</param>
    internal static Field Create(FieldType type, ProtectedBytes data) {
        return type switch {
            FieldType.Uuid => new UuidField(type, data, "UUID"),
            FieldType.Group => new TextField(type, data, "Group"),
            FieldType.Title => new TextField(type, data, "Title"),
            FieldType.UserName => new TextField(type, data, "Username"),
            FieldType.Notes => new TextField(type, data, "Notes"),
            FieldType.Password => new TextField(type, data, "Password"),
            FieldType.CreationTime => new TimestampField(type, data, "Creation time"),
            FieldType.PasswordModificationTime => new TimestampField(type, data, "Password modification time"),
            FieldType.LastAccessTime => new TimestampField(type, data, "Last access time"),
            FieldType.PasswordExpiryTime => new TimestampField(type, data, "Password expiry time"),
            FieldType.LastModificationTime => new TimestampField(type, data, "Last modification time"),
            FieldType.Url => new TextField(type, data, "URL"),
            FieldType.Autotype => new TextField(type, data, "Autotype"),
            FieldType.PasswordHistory => new TextField(type, data, "Password history"),
            FieldType.PasswordPolicy => new TextField(type, data, "Password policy"),
            FieldType.PasswordExpiryInterval => new TimestampField(type, data, "Password expiry interval"),
            FieldType.RunCommand => new TextField(type, data, "Run command"),
            FieldType.DoubleClickAction => new UnknownField(type, data, "Double-click action"),
            FieldType.EmailAddress => new TextField(type, data, "EMail address"),
            FieldType.ProtectedEntry => new UnknownField(type, data, "Protected entry"),
            FieldType.OwnSymbolsForPassword => new TextField(type, data, "Own symbols for password"),
            FieldType.ShiftDoubleClickAction => new UnknownField(type, data, "Shift double-click action"),
            FieldType.PasswordPolicyName => new TextField(type, data, "Password policy name"),
            FieldType.EntryKeyboardShortcut => new UnknownField(type, data, "Entry keyboard shortcut"),
            FieldType.TwoFactorKey => new BinaryField(type, data, "Two-factor key"),
            FieldType.CreditCardNumber => new TextField(type, data, "Credit card number"),
            FieldType.CreditCardExpiration => new TextField(type, data, "Credit card expiration"),
            FieldType.CreditCardVerificationValue => new TextField(type, data, "Credit card verification value"),
            FieldType.CreditCardPin => new TextField(type, data, "Credit card PIN"),
            FieldType.QRCode => new TextField(type, data, "QR code"),
            FieldType.TotpConfig => new UnknownField(type, data, "TOTP config"),
            FieldType.TotpLength => new UnknownField(type, data, "TOTP length"),
            FieldType.TotpTimeStep => new UnknownField(type, data, "TOTP time step"),
            FieldType.TotpStartTime => new TimestampField(type, data, "TOTP start time"),
            FieldType.AttachmentTitle => new TextField(type, data, "Attachment title"),
            FieldType.AttachmentMediaType => new TextField(type, data, "Attachment media type"),
            FieldType.AttachmentFileName => new TextField(type, data, "Attachment file name"),
            FieldType.AttachmentModificationTime => new TimestampField(type, data, "Attachment modification time"),
            FieldType.AttachmentContent => new BinaryField(type, data, "Attachment content"),
            FieldType.PasskeyCredentialID => new BinaryField(type, data, "Passkey credential ID"),
            FieldType.PasskeyRelyingPartyID => new TextField(type, data, "Passkey relying party ID"),
            FieldType.PasskeyUserHandle => new BinaryField(type, data, "Passkey user handle"),
            FieldType.PasskeyAlgorithmID => new UnknownField(type, data, "Passkey algorithm ID"),
            FieldType.PasskeyPrivateKey => new BinaryField(type, data, "Passkey private key"),
            FieldType.PasskeySignCount => new UnknownField(type, data, "Passkey sign count"),
            FieldType.CustomTextField => new UnknownField(type, data, "Custom text field"),  // TODO
            FieldType.BaseUuid => new UuidField(type, data, "Base UUID"),
            FieldType.AliasUuid => new UuidField(type, data, "Alias UUID"),
            FieldType.ShortcutUuid => new UuidField(type, data, "Shortcut UUID"),
            FieldType.EndOfEntry => throw new NotSupportedException("Cannot create EndOfEntry field."),
            _ => new UnknownField(type, data, string.Empty),
        };
    }

}
