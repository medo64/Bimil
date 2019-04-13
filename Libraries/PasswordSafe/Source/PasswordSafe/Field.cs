using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using static PasswordSafe.Helpers;

namespace Medo.Security.Cryptography.PasswordSafe {
    /// <summary>
    /// Abstract field.
    /// </summary>
    public abstract class Field {

        /// <summary>
        /// Create a new instance.
        /// </summary>
        protected Field() {
        }


        /// <summary>
        /// Gets/sets version data.
        /// -1 will be returned if conversion cannot be performed.
        /// For unknown field types, conversion will always be attempted.
        /// </summary>
        public int Version {
            get {
                if ((DataType == PasswordSafeFieldDataType.Version) || (DataType == PasswordSafeFieldDataType.Unknown)) {
                    var data = RawData;
                    try {
                        if (data.Length == 2) {
                            return BitConverter.ToUInt16(data, 0);
                        }
                    } finally {
                        Array.Clear(data, 0, data.Length);
                    }
                }
                return -1; //unknown version
            }
            set {
                if ((DataType != PasswordSafeFieldDataType.Version) && (DataType != PasswordSafeFieldDataType.Unknown)) { throw new FormatException("Field type mismatch."); }
                if ((value < 0) || (value > ushort.MaxValue)) { throw new ArgumentOutOfRangeException(nameof(value), "Value outside of range."); }
                RawData = BitConverter.GetBytes((ushort)value);
            }
        }

        /// <summary>
        /// Gets/sets UUID data.
        /// Guid.Empty will be returned if conversion cannot be performed.
        /// For unknown field types, conversion will always be attempted.
        /// </summary>
        public Guid Uuid {
            get {
                if ((DataType == PasswordSafeFieldDataType.Uuid) || (DataType == PasswordSafeFieldDataType.Unknown)) {
                    var data = RawData;
                    try {
                        if (data.Length == 16) {
                            return new Guid(data);
                        }
                    } finally {
                        Array.Clear(data, 0, data.Length);
                    }
                }
                return Guid.Empty; //unknown guid
            }
            set {
                if ((DataType != PasswordSafeFieldDataType.Uuid) && (DataType != PasswordSafeFieldDataType.Unknown)) { throw new FormatException("Field type mismatch."); }
                RawData = value.ToByteArray();
            }
        }


        private static readonly Encoding Utf8Encoding = new UTF8Encoding(false);

        /// <summary>
        /// Gets/sets text data.
        /// Null will be returned if conversion cannot be performed.
        /// For unknown field types, conversion will always be attempted.
        /// </summary>
        public virtual string Text {
            get {
                if ((DataType == PasswordSafeFieldDataType.Text) || (DataType == PasswordSafeFieldDataType.Unknown)) {
                    var data = RawData;
                    try {
                        return Utf8Encoding.GetString(data);
                    } finally {
                        Array.Clear(data, 0, data.Length);
                    }
                }
                return null;
            }
            set {
                if ((DataType != PasswordSafeFieldDataType.Text) && (DataType != PasswordSafeFieldDataType.Unknown)) { throw new FormatException("Field type mismatch."); }
                if (value == null) { throw new ArgumentNullException(nameof(value), "Value cannot be null."); }
                RawData = Utf8Encoding.GetBytes(value);
            }
        }


        private static readonly DateTime TimeMin = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly DateTime TimeMax = TimeMin.AddSeconds(uint.MaxValue);

        /// <summary>
        /// Gets/sets time data.
        /// DateTime.MinValue will be returned if conversion cannot be performed.
        /// For unknown field types, conversion will always be attempted.
        /// </summary>
        public DateTime Time {
            get {
                if ((DataType == PasswordSafeFieldDataType.Time) || (DataType == PasswordSafeFieldDataType.Unknown)) {
                    var data = RawData;
                    try {
                        if (data.Length == 4) {
                            var seconds = BitConverter.ToUInt32(RawData, 0);
                            return TimeMin.AddSeconds(seconds);
                        } else if (data.Length == 8) { //try hexadecimal
                            if (uint.TryParse(Utf8Encoding.GetString(data), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var seconds)) {
                                return TimeMin.AddSeconds(seconds);
                            } else {
                                return DateTime.MinValue;
                            }
                        }
                    } finally {
                        Array.Clear(data, 0, data.Length);
                    }
                }
                return DateTime.MinValue;
            }
            set {
                if ((DataType != PasswordSafeFieldDataType.Time) && (DataType != PasswordSafeFieldDataType.Unknown)) { throw new FormatException("Field type mismatch."); }
                if ((value < TimeMin) || (value > TimeMax)) { throw new ArgumentNullException(nameof(value), "Time outside of allowable range."); }
                var seconds = (uint)((value.ToUniversalTime() - TimeMin).TotalSeconds);
                RawData = BitConverter.GetBytes(seconds);
            }
        }


        /// <summary>
        /// Returns data as bytes.
        /// </summary>
        public byte[] GetBytes() {
            var data = RawData;
            try {
                var dataCopy = new byte[data.Length];
                Buffer.BlockCopy(data, 0, dataCopy, 0, dataCopy.Length);
                return dataCopy;
            } finally {
                Array.Clear(data, 0, data.Length);
            }
        }

        /// <summary>
        /// Returns data as bytes without marking the field as accessed.
        /// </summary>
        internal byte[] GetBytesSilently() {
            return RawDataDirect;
        }

        /// <summary>
        /// Sets byte data.
        /// </summary>
        /// <param name="value">Bytes.</param>
        /// <exception cref="ArgumentNullException">Value cannot be null.</exception>
        public void SetBytes(byte[] value) {
            if (value == null) { throw new ArgumentNullException(nameof(value), "Value cannot be null."); }
            var valueCopy = new byte[value.Length];
            try {
                Buffer.BlockCopy(value, 0, valueCopy, 0, valueCopy.Length);
                RawData = valueCopy;
            } finally {
                Array.Clear(valueCopy, 0, valueCopy.Length);
            }
        }


        private static readonly RandomNumberGenerator Rnd = RandomNumberGenerator.Create();
        private readonly byte[] RawDataEntropy = new byte[16];


        private byte[] _rawData = null;
        /// <summary>
        /// Gets/sets raw data.
        /// Bytes are kept encrypted in memory until accessed.
        /// </summary>
        internal byte[] RawData {
            get {
                MarkAsAccessed();
                return RawDataDirect;
            }
            set {
                if (IsReadOnly) { throw new NotSupportedException("Object is read-only."); }

                var oldValue = RawDataDirect;
                try {
                    if (oldValue.Length == value.Length) { //skip writing the same value (to avoid marking document changed without reason)
                        var areSame = true;
                        for (var i = 0; i < value.Length; i++) {
                            if (oldValue[i] != value[i]) {
                                areSame = false;
                                break;
                            }
                        }
                        if (areSame) { return; }
                    }
                } finally {
                    Array.Clear(oldValue, 0, oldValue.Length);
                }

                Rnd.GetBytes(RawDataEntropy); //new entropy every save
                _rawData = ProtectData(value, RawDataEntropy);
                Array.Clear(value, 0, value.Length);

                MarkAsChanged();
            }
        }
        /// <summary>
        /// Gets raw data without marking the field as accessed.
        /// Bytes are kept encrypted in memory until accessed.
        /// </summary>
        protected byte[] RawDataDirect {
            get {
                if (_rawData == null) { return new byte[0]; } //return empty array if no value has been set so far
                return UnprotectData(_rawData, RawDataEntropy);
            }
        }

        /// <summary>
        /// Used to mark document as changed.
        /// </summary>
        protected abstract void MarkAsChanged();

        /// <summary>
        /// Used to mark document as accessed.
        /// </summary>
        protected abstract void MarkAsAccessed();

        /// <summary>
        /// Gets if object is read-only.
        /// </summary>
        protected abstract bool IsReadOnly { get; }

        /// <summary>
        /// Gets underlying data type for field.
        /// </summary>
        protected abstract PasswordSafeFieldDataType DataType { get; }


        /// <summary>
        /// Underlying data type enumeration used for data parsing.
        /// </summary>
        protected enum PasswordSafeFieldDataType {
            /// <summary>
            /// Unknown data type.
            /// </summary>
            Unknown,
            /// <summary>
            /// Version.
            /// </summary>
            Version,
            /// <summary>
            /// UUID.
            /// </summary>
            Uuid,
            /// <summary>
            /// Text.
            /// </summary>
            Text,
            /// <summary>
            /// Time.
            /// </summary>
            Time,
            /// <summary>
            /// Bytes.
            /// </summary>
            Binary,
        }


        /// <summary>
        /// Returns a string representation of an object.
        /// </summary>
        public override string ToString() {
            switch (DataType) {
                case PasswordSafeFieldDataType.Version: return Version.ToString("X4", CultureInfo.InvariantCulture);
                case PasswordSafeFieldDataType.Uuid: return Uuid.ToString();
                case PasswordSafeFieldDataType.Text: return Text;
                case PasswordSafeFieldDataType.Time: return Time.ToLocalTime().ToString("yyyy'-'MM'-'dd HH':'mm':'ss K", CultureInfo.InvariantCulture);
                default: return "0x" + BitConverter.ToString(RawData).Replace("-", "");
            }
        }

    }
}
