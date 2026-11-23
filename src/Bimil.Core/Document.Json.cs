namespace Bimil;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

public sealed partial class Document {

    #region Export

    /// <summary>
    /// Exports record to a JSON text.
    /// </summary>
    /// <param name="record">Record to export.</params>
    public static string ExportRecordsToJson(Record record) {
        return ExportRecordsToJson(new Record[] { record }, prettyPrint: false);
    }

    /// <summary>
    /// Exports records to a JSON text.
    /// </summary>
    /// <param name="records">Records to export.</params>
    public static string ExportRecordsToJson(RecordCollection records) {
        return ExportRecordsToJson(records.ToArray(), prettyPrint: false);
    }

    /// <summary>
    /// Exports records to a JSON text.
    /// </summary>
    /// <param name="records">Records to export.</params>
    public static string ExportRecordsToJson(IEnumerable records) {
        return ExportRecordsToJson(records, prettyPrint: false);
    }

    /// <summary>
    /// Exports record to a JSON text.
    /// </summary>
    /// <param name="record">Record to export.</params>
    /// <param name="prettyPrint">If true, output will be indented.</param>
    public static string ExportRecordsToJson(Record record, bool prettyPrint) {
        return ExportRecordsToJson(new Record[] { record }, prettyPrint);
    }

    /// <summary>
    /// Exports records to a JSON text.
    /// </summary>
    /// <param name="records">Records to export.</params>
    /// <param name="prettyPrint">If true, output will be indented.</param>
    public static string ExportRecordsToJson(RecordCollection records, bool prettyPrint) {
        return ExportRecordsToJson(records.ToArray(), prettyPrint);
    }

    /// <summary>
    /// Exports records to a JSON text.
    /// </summary>
    /// <param name="records">Records to export.</params>
    /// <param name="prettyPrint">If true, output will be indented.</param>
    public static string ExportRecordsToJson(IEnumerable records, bool prettyPrint) {
        var options = new JsonWriterOptions();
        if (prettyPrint) {
            options.Indented = true;
            options.IndentSize = 2;
        }
        ;

        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream, options);

        writer.WriteStartObject();
        writer.WriteString("id", "Bimil");

        writer.WritePropertyName("records");
        writer.WriteStartArray();
        foreach (Record record in records) {
            writer.WriteStartObject();

            writer.WritePropertyName("fields");
            writer.WriteStartArray();
            foreach (var field in record.Fields) {
                writer.WriteStartObject();
                writer.WriteString("type", field.Type.ToString());
                if (field is BinaryField binaryField) {
                    var bytes = field.Data.GetBytes();
                    try {
                        writer.WriteString("binary", Convert.ToBase64String(bytes));
                    } finally {
                        CryptographicOperations.ZeroMemory(bytes);
                    }
                } else if (field is CustomTextField customTextField) {
                    writer.WriteString("caption", customTextField.Caption);
                    writer.WriteString("text", customTextField.Text);
                    if (customTextField.IsSensitive) {
                        writer.WriteBoolean("isSensitive", customTextField.IsSensitive);
                    }
                } else if (field is TextField textField) {
                    writer.WriteString("text", textField.Text);
                } else if (field is TimestampField timestampField) {
                    writer.WriteString("time", timestampField.Timestamp.ToString("yyyy-MM-dd'T'HH:mm:ssZ"));
                } else if (field is UuidField uuidField) {
                    writer.WriteString("uuid", uuidField.Uuid);
                } else {
                    Debug.WriteLine("Unknown field type.");
                }
                writer.WriteEndObject();
            }
            writer.WriteEndArray();

            writer.WriteEndObject();
        }

        writer.WriteEndArray();  // records
        writer.WriteEndObject();  // root
        writer.Flush();

        return UTF8Encoding.UTF8.GetString(stream.ToArray());
    }

    #endregion Export

    #region Import

    /// <summary>
    /// Returns imported entry based on JSON object text.
    /// </summary>
    /// <param name="jsonText">JSON object.</param>
    public static Record[] ImportRecordsFromJson(string jsonText) {
        (var records, var exceptionText) = ImportRecordsFromJsonInt(jsonText);
        if (records != null) {
            return records;
        } else {
            throw new InvalidDataException(exceptionText ?? "Unknown error.");
        }
    }

    /// <summary>
    /// Returns true if entry based on JSON object text could be parsed.
    /// </summary>
    /// <param name="jsonText">JSON object.</param>
    /// <param name="record">Output record.</param>
    public static bool TryImportRecordsFromJson(string jsonText, [NotNullWhen(true)] out Record[]? records) {
        (records, var exception) = ImportRecordsFromJsonInt(jsonText);
        if (records != null) {
            return true;
        } else {
            return false;
        }
    }

    private static (Record[]? records, string? exceptionText) ImportRecordsFromJsonInt(string jsonText) {
        if (string.IsNullOrWhiteSpace(jsonText)) { return (null, "No data."); }

        var records = new List<Record>();
        try {
            var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(jsonText));
            if (!reader.Read() || (reader.TokenType != JsonTokenType.StartObject)) { return (null, "Cannot find JSON start object."); }

            while (reader.Read()) {
                if (reader.TokenType == JsonTokenType.PropertyName) {
                    var prop1Name = reader.GetString();
                    if (prop1Name == null) {
                        return (null, "Unexpected null property name.");
                    } else if (prop1Name.Equals("id", StringComparison.Ordinal)) {
                        reader.Read();
                        if (reader.TokenType == JsonTokenType.String) {
                            var content = reader.GetString() ?? throw new InvalidDataException("Unexpected null id.");
                            if (!content.Equals("Bimil", StringComparison.Ordinal)) { throw new InvalidDataException("Unknown id."); }
                        } else {
                            return (null, "Cannot identify JSON.");
                        }
                    } else if (prop1Name.Equals("records", StringComparison.Ordinal)) {
                        if (!reader.Read() || reader.TokenType != JsonTokenType.StartArray) { continue; }

                        while (reader.Read()) {
                            if (reader.TokenType == JsonTokenType.EndArray) { break; }
                            if (reader.TokenType != JsonTokenType.StartObject) { continue; }

                            var fields = new List<Field>();
                            while (reader.Read()) {
                                if (reader.TokenType == JsonTokenType.PropertyName) {
                                    var prop2Name = reader.GetString();
                                    if (prop2Name == null) {
                                        return (null, "Unexpected null property name.");
                                    } else if (prop2Name.Equals("kind", StringComparison.Ordinal)) {
                                        reader.Read();
                                        if (reader.TokenType == JsonTokenType.String) {
                                            var content = reader.GetString() ?? throw new InvalidDataException("Unexpected null kind.");
                                            if (!content.Equals("Bimil.Record", StringComparison.Ordinal)) { throw new InvalidDataException("Unknown kind."); }
                                        } else {
                                            return (null, "Cannot determine kind.");
                                        }
                                    } else if (prop2Name.Equals("fields", StringComparison.Ordinal)) {
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
                                            bool? isSensitive = null;

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
                                                            return (null, "Unexpected caption value.");
                                                        }
                                                        break;

                                                    case "text":
                                                        if (reader.TokenType == JsonTokenType.String) {
                                                            text = reader.GetString();
                                                        } else {
                                                            return (null, "Unexpected text value.");
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
                                                            return (null, "Unexpected UUID value.");
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
                                                            return (null, "Unexpected time value.");
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
                                                            return (null, "Unexpected version value.");
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
                                                            return (null, "Unexpected binary value.");
                                                        }
                                                        break;

                                                    case "isSensitive":
                                                        if (reader.TokenType == JsonTokenType.True) {
                                                            isSensitive = true;
                                                        } else if (reader.TokenType == JsonTokenType.False) {
                                                            isSensitive = false;
                                                        } else {
                                                            return (null, "Unexpected isSensitive value.");
                                                        }
                                                        break;

                                                    default: return (null, $"Unexpected '{name}' property.");
                                                }
                                            }

                                            if (type != null) {
                                                var field = Field.Create(type.Value);
                                                if (field is BinaryField binaryField) {
                                                    if (binary == null) { return (null, "Missing binary data."); }
                                                    binaryField.Data.SetBytes(binary);
                                                } else if (field is CustomTextField customTextField) {
                                                    if (caption == null) { return (null, "Missing caption data."); }
                                                    if (text == null) { return (null, "Missing text data."); }
                                                    customTextField.Caption = caption;
                                                    customTextField.Text = text;
                                                    customTextField.IsSensitive = isSensitive ?? false;
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
                                        return (null, $"Unexpected '{prop2Name}' property.");
                                    }
                                } else if (reader.TokenType == JsonTokenType.EndObject) {
                                    break;
                                }
                            }

                            records.Add(Record.Create(fields));
                        }
                    } else {
                        return (null, $"Unexpected '{prop1Name}' property.");
                    }
                } else if (reader.TokenType == JsonTokenType.EndObject) {
                    break;
                }

            }
        } catch (JsonException ex) {
            return (null, ex.Message);
        }
        return (records.ToArray(), exceptionText: null);
    }

    #endregion Import

}
