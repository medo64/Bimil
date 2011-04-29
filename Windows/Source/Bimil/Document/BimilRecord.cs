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
        /// One-line text.
        /// </summary>
        Text = 0,
        /// <summary>
        /// Multiline text.
        /// </summary>
        MultilineText = 1,
        /// <summary>
        /// URL.
        /// </summary>
        Url = 10,
        /// <summary>
        /// Password text.
        /// </summary>
        Password = 20,
    }

}
