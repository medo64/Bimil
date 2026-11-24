namespace Bimil;

internal static class Filtering {

    /// <summary>
    /// Gets if field can be added as an entry field.
    /// </summary>
    /// <param name="field">Field to check.</param>
    public static bool IsFieldAppendable(Field field) {
        if (field is TextField textField) {
            return field.Type switch {
                FieldType.Title => false,
                FieldType.Group => false,
                FieldType.Autotype => false,
                FieldType.PasswordHistory => false,
                FieldType.PasswordPolicy => false,
                FieldType.OwnSymbolsForPassword => false,
                FieldType.PasswordPolicyName => false,
                FieldType.Attachment3Title => false,
                FieldType.Attachment3MediaType => false,
                FieldType.Attachment3FileName => false,
                FieldType.PasskeyRelyingPartyID => false,
                FieldType.Attachment4Title => false,
                FieldType.Attachment4MediaType => false,
                FieldType.Attachment4FileName => false,
                FieldType.Attachment4FilePath => false,
                _ => true,
            };
        } else if (field is CustomTextField) {
            return true;
        } else {
            return false;
        }
    }

}
