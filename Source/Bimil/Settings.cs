namespace Bimil {
    internal static class Settings {

        public static bool CloseOnEscape {
            get { return Medo.Configuration.Settings.Read("CloseOnEscape", false); }
            set { Medo.Configuration.Settings.Write("CloseOnEscape", value); }
        }

        public static string LastGoogleEmail {
            get { return Medo.Configuration.Settings.Read("LastGoogleEmail", ""); }
            set { Medo.Configuration.Settings.Write("LastGoogleEmail", value); }
        }

    }
}
