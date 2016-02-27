namespace Bimil {
    internal static class Settings {

        public static bool CloseOnEscape {
            get { return Medo.Configuration.Settings.Read("CloseOnEscape", false); }
            set { Medo.Configuration.Settings.Write("CloseOnEscape", value); }
        }


        public static double ScaleBoost {
            get { return Medo.Configuration.Settings.Read("ScaleBoost", 0.00); }
            set {
                if ((value < -1) || (value > 4)) { return; }
                Medo.Configuration.Settings.Write("ScaleBoost", value);
            }
        }

    }
}
