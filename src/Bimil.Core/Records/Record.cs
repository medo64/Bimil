namespace Bimil;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

/// <summary>
/// Record.
/// </summary>
public abstract class Record {

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="fields">Fields.</param>
    private protected Record(FieldCollection fields) {
        ArgumentNullException.ThrowIfNull(fields);
        Fields = fields;
    }


    /// <summary>
    /// Gets field collection.
    /// </summary>
    public FieldCollection Fields { get; }


    /// <summary>
    /// Gets if values are read-only.
    /// </summary>
    public virtual bool IsReadOnly {
        get;
        internal set {
            field = value;
            Fields.IsReadOnly = value;
        }
    }


    #region Export/Import

    /// <summary>
    /// Exports entry to a JSON object.
    /// </summary>
    public string ExportToJson() {
        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream);

        writer.WriteStartObject();
        writer.WriteString("kind", "Bimil.Record");
        writer.WritePropertyName("fields");
        writer.WriteStartArray();

        foreach (var field in Fields) {
            writer.WriteStartObject();
            writer.WriteString("type", field.Type.ToString());
            if (field is BinaryField binaryField) {
                var bytes = field.Data.GetBytes();
                try {
                    writer.WriteString("binary", Convert.ToBase64String(bytes));
                } finally {
                    CryptographicOperations.ZeroMemory(bytes);
                }
            } else if (field is TextField textField) {
                writer.WriteString("text", textField.Text);
            } else if (field is TimestampField timestampField) {
                writer.WriteString("time", timestampField.Timestamp);
            } else if (field is UuidField uuidField) {
                writer.WriteString("uuid", uuidField.Uuid);
            } else {
                Debug.WriteLine("Unknown field type.");
            }
            writer.WriteEndObject();
        }

        writer.WriteEndArray();
        writer.WriteEndObject();
        writer.Flush();

        return UTF8Encoding.UTF8.GetString(stream.ToArray());
    }

    /// <summary>
    /// Returns imported entry based on JSON object text.
    /// </summary>
    /// <param name="jsonText">JSON object.</param>
    public static Record ImportFromJson(string jsonText) {
        (var record, var exceptionText) = ImportFromJsonInt(jsonText);
        if (record != null) {
            return record;
        } else {
            throw new InvalidDataException(exceptionText ?? "Unknown error.");
        }
    }

    /// <summary>
    /// Returns true if entry based on JSON object text could be parsed.
    /// </summary>
    /// <param name="jsonText">JSON object.</param>
    /// <param name="record">Output record.</param>
    public static bool TryImportFromJson(string jsonText, [NotNullWhen(true)] out Record? record) {
        (record, var exception) = ImportFromJsonInt(jsonText);
        if (record != null) {
            return true;
        } else {
            return false;
        }
    }

    /// <summary>
    /// Returns imported entry based on JSON object text.
    /// </summary>
    public static (Record? record, string? exceptionText) ImportFromJsonInt(string jsonText) {
        if (string.IsNullOrWhiteSpace(jsonText)) { return (null, "No data."); }

        try {
            var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(jsonText));
            if (!reader.Read() || (reader.TokenType != JsonTokenType.StartObject)) { return (null, "Cannot find JSON start object."); }

            var fields = new List<Field>();

            while (reader.Read()) {
                if (reader.TokenType == JsonTokenType.PropertyName) {
                    var propName = reader.GetString();
                    if (propName == null) {
                        return (null, "Unexpected null property name.");
                    } else if (propName.Equals("kind", StringComparison.Ordinal)) {
                        reader.Read();
                        if (reader.TokenType == JsonTokenType.String) {
                            var content = reader.GetString() ?? throw new InvalidDataException("Unexpected null kind.");
                            if (!content.Equals("Bimil.Record", StringComparison.Ordinal)) { throw new InvalidDataException("Unknown kind."); }
                        } else {
                            return (null, "Cannot determine kind.");
                        }
                    } else if (propName.Equals("fields", StringComparison.Ordinal)) {
                        if (!reader.Read() || reader.TokenType != JsonTokenType.StartArray) { continue; }

                        while (reader.Read()) {
                            if (reader.TokenType == JsonTokenType.EndArray) { break; }
                            if (reader.TokenType != JsonTokenType.StartObject) { continue; }

                            FieldType? type = null;
                            string? caption = null;
                            string? text = null;
                            Guid? uuid = null;
                            DateTime? time = null;
                            int? version = null;
                            byte[]? binary = null;
                            //bool? isSensitive = null;  // TODO: customtextfield

                            while (reader.Read()) {
                                if (reader.TokenType == JsonTokenType.EndObject) { break; }
                                if (reader.TokenType != JsonTokenType.PropertyName) { continue; }

                                var name = reader.GetString();
                                if (!reader.Read()) { break; }

                                switch (name) {
                                    case "type":
                                        if (reader.TokenType == JsonTokenType.String) {
                                            var content = reader.GetString();
                                            if (Enum.TryParse<FieldType>(content, out var parsed)) {
                                                type = parsed;
                                            } else {
                                                return (null, $"Cannot convert '{content}' to record type.");
                                            }
                                        } else {
                                            return (null, "Unexpected record type.");
                                        }
                                        break;

                                    case "caption":
                                        if (reader.TokenType == JsonTokenType.String) {
                                            caption = reader.GetString();
                                        } else {
                                            return (null, "Unexpected caption type.");
                                        }
                                        break;

                                    case "text":
                                        if (reader.TokenType == JsonTokenType.String) {
                                            text = reader.GetString();
                                        } else {
                                            return (null, "Unexpected text type.");
                                        }
                                        break;

                                    case "uuid":
                                        if (reader.TokenType == JsonTokenType.String) {
                                            var content = reader.GetString();
                                            if (Guid.TryParse(content, out var parsed)) {
                                                uuid = parsed;
                                            } else {
                                                return (null, $"Cannot convert '{content}' to UUID.");
                                            }
                                        } else {
                                            return (null, "Unexpected UUID type.");
                                        }
                                        break;

                                    case "time":
                                        if (reader.TokenType == JsonTokenType.String) {
                                            var content = reader.GetString();
                                            if (DateTime.TryParse(content, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var value)) {
                                                time = value;
                                            } else {
                                                return (null, $"Cannot convert '{content}' to time.");
                                            }
                                        } else {
                                            return (null, "Unexpected time type.");
                                        }
                                        break;

                                    case "version":
                                        if (reader.TokenType == JsonTokenType.String) {
                                            var content = reader.GetString();
                                            if (int.TryParse(content, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)) {
                                                version = value;
                                            } else {
                                                return (null, $"Cannot convert '{content}' to version.");
                                            }
                                        } else {
                                            return (null, "Unexpected version type.");
                                        }
                                        break;

                                    case "binary":
                                        if (reader.TokenType == JsonTokenType.String) {
                                            var content = reader.GetString() ?? "";
                                            var binaryBuffer = new byte[content.Length];
                                            if (Convert.TryFromBase64String(content, binaryBuffer, out var bytesInBuffer)) {
                                                binary = new byte[bytesInBuffer];
                                                Buffer.BlockCopy(binaryBuffer, 0, binary, 0, binary.Length);
                                            } else {
                                                return (null, $"Cannot convert '{content}' to binary.");
                                            }
                                        } else {
                                            return (null, "Unexpected binary type.");
                                        }
                                        break;

                                    // case "isSensitive":
                                    //     if (reader.TokenType == JsonTokenType.True) {
                                    //         isSensitive = true;
                                    //     } else if (reader.TokenType == JsonTokenType.False) {
                                    //         isSensitive = false;
                                    //     } else {
                                    //         return (null, "Unexpected isSensitive type.");
                                    //     }
                                    //     break;

                                    default: return (null, $"Unexpected '{name}' property.");
                                }
                            }

                            if (type != null) {
                                var field = Field.Create(type.Value);
                                if (field is BinaryField binaryField) {
                                    if (binary == null) { return (null, "Missing binary data."); }
                                    binaryField.Data.SetBytes(binary);
                                } else if (field is TextField textField) {
                                    if (text == null) { return (null, "Missing text data."); }
                                    textField.Text = text;
                                } else if (field is TimestampField timestampField) {
                                    if (time == null) { return (null, "Missing timestamp data."); }
                                    timestampField.Timestamp = time.Value;
                                } else if (field is UuidField uuidField) {
                                    if (uuid == null) { return (null, "Missing UUID data."); }
                                    uuidField.Uuid = uuid.Value;
                                } else {
                                    return (null, "Record data cannot be determined.");
                                }
                                fields.Add(field);
                            } else {
                                return (null, "Record type is missing.");
                            }
                        }
                    } else {
                        return (null, $"Unexpected '{propName}' property.");
                    }
                } else if (reader.TokenType == JsonTokenType.EndObject) {
                    break;
                }
            }

            return (new EntryRecord(fields), null);
        } catch (JsonException ex) {
            return (null, ex.Message);
        }
    }

    #endregion Export/Import


    /// <summary>
    /// Returns record based on type.
    /// </summary>    
    public static Record Create(ICollection<Field> fields) {
        foreach (var field in fields) {
            if (field.Type == FieldType.Uuid) {
                return new EntryRecord(fields);
            } else if (field.Type == FieldType.AliasUuid) {
                return new AliasRecord(fields);
            } else if (field.Type == FieldType.ShortcutUuid) {
                return new AliasRecord(fields);
            } else if (field.Type == FieldType.Attachment4Uuid) {
                return new AttachmentRecord(fields);
            }
        }
        return new UnknownRecord(fields);
    }

}
