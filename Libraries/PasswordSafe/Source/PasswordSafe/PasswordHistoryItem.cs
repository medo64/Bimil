using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using static PasswordSafe.Helpers;

namespace Medo.Security.Cryptography.PasswordSafe {
    /// <summary>
    /// One entry in password history.
    /// </summary>
    [DebuggerDisplay("{Password}")]
    public class PasswordHistoryItem {

        internal PasswordHistoryItem(PasswordHistoryCollection owner, DateTime firstTimeUsed, string historicalPassword) {
            Owner = owner;
            TimeFirstUsed = firstTimeUsed;

            byte[] historicalPasswordBytes = null;
            try {
                historicalPasswordBytes = Utf8Encoding.GetBytes(historicalPassword);
                RawHistoricalPasswordData = historicalPasswordBytes;
            } finally {
                if (historicalPasswordBytes != null) { Array.Clear(historicalPasswordBytes, 0, historicalPasswordBytes.Length); }
            }
        }


        private readonly PasswordHistoryCollection Owner;


        /// <summary>
        /// Used to mark document as changed.
        /// </summary>
        protected void MarkAsChanged() {
            if (Owner != null) { Owner.MarkAsChanged(); }
        }


        /// <summary>
        /// Gets time when password was first used.
        /// </summary>
        public DateTime TimeFirstUsed { get; }

        /// <summary>
        /// Gets historical password.
        /// </summary>
        public string HistoricalPassword {
            get {
                var data = RawHistoricalPasswordData;
                try {
                    return Utf8Encoding.GetString(data);
                } finally {
                    Array.Clear(data, 0, data.Length);
                }
            }
        }


        private static readonly Encoding Utf8Encoding = new UTF8Encoding(false);
        private static readonly RandomNumberGenerator Rnd = RandomNumberGenerator.Create();
        private readonly byte[] RawHistoricalPasswordDataEntropy = new byte[16];

        private byte[] _rawHistoricalPasswordData = null;
        /// <summary>
        /// Gets/sets raw data.
        /// Bytes are kept encrypted in memory until accessed.
        /// </summary>
        private byte[] RawHistoricalPasswordData {
            get {
                if (_rawHistoricalPasswordData == null) { return new byte[0]; } //return empty array if no value has been set so far
                return UnprotectData(_rawHistoricalPasswordData, RawHistoricalPasswordDataEntropy);
            }
            set {
                Rnd.GetBytes(RawHistoricalPasswordDataEntropy); //new entropy every save
                _rawHistoricalPasswordData = ProtectData(value, RawHistoricalPasswordDataEntropy);
                Array.Clear(value, 0, value.Length);
            }
        }

    }
}
