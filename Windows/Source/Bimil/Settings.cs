namespace Bimil {
    internal static class Settings {

        public static bool UseNotificationArea {
            get { return Medo.Configuration.Settings.Read("UseNotificationArea", true); }
            set { Medo.Configuration.Settings.Write("UseNotificationArea", value); }
        }

    }
}
