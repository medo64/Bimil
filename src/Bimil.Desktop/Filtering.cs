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
                _ => true,
            };
        } else if (field is CustomTextField) {
            return true;
        } else {
            return false;
        }
    }

}
