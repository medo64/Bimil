namespace Medo.Security.Cryptography.Bimil {
    public class BimilRecord {

        private readonly BimilDocument Document;

        internal BimilRecord(BimilDocument document, string key, string value, BimilRecordFormat format) {
            this.Document = document;
            this.Key = new BimilValue(document) { Text = key };
            this.Value = new BimilValue(document) { Text = value };
            this.Format = format;
        }


        /// <summary>
        /// Gets key.
        /// </summary>
        public BimilValue Key { get; private set; }

        /// <summary>
        /// Gets value.
        /// </summary>
        public BimilValue Value { get; private set; }

        /// <summary>
        /// Gets/sets type.
        /// </summary>
        public BimilRecordFormat Format { get; set; }


        /// <summary>
        /// Returns key.
        /// </summary>
        public override string ToString() {
            return this.Key.ToString();
        }

    }



    /// <summary>
    /// Bimil record formats.
    /// </summary>
    public enum BimilRecordFormat : int {
        /// <summary>
        /// System parameter.
        /// </summary>
        System = 0,
        /// <summary>
        /// One-line text.
        /// </summary>
        Text = 10,
        /// <summary>
        /// Multiline text.
        /// </summary>
        MultilineText = 11,
        /// <summary>
        /// Fixed-font text.
        /// </summary>
        MonospacedText = 12,
        /// <summary>
        /// URL.
        /// </summary>
        Url = 20,
        /// <summary>
        /// Password text.
        /// </summary>
        Password = 30,
    }

}
