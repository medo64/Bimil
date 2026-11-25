namespace Bimil;

using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Text record field.
/// </summary>
public sealed class CustomTextField : Field, IReadableCaption, IWritableCaption, IReadableText, IWritableText, IReadableIsSensitive, IWritableIsSensitive {

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="type">Type.</param>
    /// <param name="data">Data.</param>
    internal CustomTextField(FieldType type, ProtectedBytes data)
       : base(type, data) {
        Caption = Field.GetDefaultCaption(type);
    }

    /// <summary>
    /// Gets/sets caption.
    /// </summary>
    public override string Caption {
        get {
            ParseData(Data, out var caption, out var _, out var _);
            return caption;
        }
        set {
            SetData(value, Text, IsSensitive);
        }
    }


    /// <summary>
    /// Gets/sets the text.
    /// </summary>
    public string Text {
        get {
            ParseData(Data, out var _, out var text, out var _);
            return text;
        }
        set {
            SetData(Caption, value, IsSensitive);
        }
    }

    /// <summary>
    /// Gets/sets if field is sensitive.
    /// </summary>
    public bool IsSensitive {
        get {
            ParseData(Data, out var _, out var _, out var isSensitive);
            return isSensitive;
        }
        set {
            SetData(Caption, Text, value);
        }
    }


    #region Parsing

    private static bool ParseData(ProtectedBytes data, out string caption, out string text, out bool isSensitive) {
        caption = "";
        text = "";
        isSensitive = false;
        if (data == null) { return false; }

        var rawData = data.GetBytes();
        try {
            var rawText = UTF8Encoding.UTF8.GetString(rawData);
            var index = 0;
            while (index < rawData.Length) {
                if (index + 2 > rawData.Length) { return false; }
#if NETSTANDARD2_0
            if (!int.TryParse(rawText.Substring(index, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var propId)) { return false; }
#else
                if (!int.TryParse(rawText.AsSpan(index, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var propId)) { return false; }
#endif
                index += 2;

                if (index + 4 > rawData.Length) { return false; }
#if NETSTANDARD2_0
            if (!int.TryParse(rawText.Substring(index, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var propLength)) { return false; }
#else
                if (!int.TryParse(rawText.AsSpan(index, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var propLength)) { return false; }
#endif
                index += 4;

                if (index + propLength > rawData.Length) { return false; }
                var propText = rawText.Substring(index, propLength);
                index += propLength;

                switch (propId) {
                    case 0x00: return true;  // code currently doesn't support parsing of multiple entries, only the first one is taken
                    case 0x01: caption = propText; break;
                    case 0x02: text = propText; break;
                    case 0x03: isSensitive = propText.Equals("1", StringComparison.Ordinal); break;
                    default: break;
                }
            }
            return true;
        } finally {
            CryptographicOperations.ZeroMemory(rawData);
        }
    }

    private void SetData(string? caption, string? text, bool isSensitive) {
        var sb = new StringBuilder();
        if (caption != null) {
            sb.Append("01");
            sb.Append(caption.Length.ToString("X4", CultureInfo.InvariantCulture));
            sb.Append(caption);
        }
        if (text != null) {
            sb.Append("02");
            sb.Append(text.Length.ToString("X4", CultureInfo.InvariantCulture));
            sb.Append(text);
        }
        if (isSensitive != false) {
            sb.Append("03");
            sb.Append("0001");
            sb.Append("1");
        }
        var rawData = UTF8Encoding.UTF8.GetBytes(sb.ToString());
        Data.SetBytes(rawData, zeroBytes: true);

        sb.Length = 0;
        sb = null;
        GC.Collect();  // try cleaning from memory
    }

    #endregion Parsing

}
