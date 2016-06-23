using System;
using System.ComponentModel;

namespace Bimil {
    internal static class Settings {

        [Category("Behavior")]
        [DisplayName("Close on escape")]
        [Description("If true, escape will close application.")]
        [DefaultValue(false)]
        public static bool CloseOnEscape {
            get { return Medo.Configuration.Settings.Read("CloseOnEscape", false); }
            set { Medo.Configuration.Settings.Write("CloseOnEscape", value); }
        }

        [Category("Behavior")]
        [DisplayName("Show start")]
        [Description("If true, Start window will be shown.")]
        [DefaultValue(true)]
        public static bool ShowStart {
            get { return Medo.Configuration.Settings.Read("ShowStart", true); }
            set { Medo.Configuration.Settings.Write("ShowStart", value); }
        }

        [Category("Behavior")]
        [DisplayName("Auto-close application timeout")]
        [Description("Time in seconds before window will close if it loses focus. Value 0 disables auto-close.")]
        [DefaultValue(900)]
        public static int AutoCloseTimeout {
            get { return LimitBetween(Medo.Configuration.Settings.Read("AutoCloseTimeout", 900), minValue: 10, maxValue: 3600, allowZero: true); }
            set { Medo.Configuration.Settings.Write("AutoCloseTimeout", LimitBetween(value, minValue: 10, maxValue: 3600, allowZero: true)); }
        }

        [Category("Behavior")]
        [DisplayName("Auto-close window timeout")]
        [Description("Time in seconds before item window will close if it loses focus. Value 0 disables auto-close. Note that auto-close will cancel any edit in progress.")]
        [DefaultValue(120)]
        public static int AutoCloseItemTimeout {
            get { return LimitBetween(Medo.Configuration.Settings.Read("AutoCloseItemTimeout", 120), minValue: 10, maxValue: 3600, allowZero: true); }
            set { Medo.Configuration.Settings.Write("AutoCloseItemTimeout", LimitBetween(value, minValue: 10, maxValue: 3600, allowZero: true)); }
        }


        [Category("Visual")]
        [DisplayName("Scale boost")]
        [Description("Additional value to determine toolbar scaling.")]
        [DefaultValue(120)]
        public static double ScaleBoost {
            get { return Medo.Configuration.Settings.Read("ScaleBoost", 0.00); }
            set {
                if ((value < -1) || (value > 4)) { return; }
                Medo.Configuration.Settings.Write("ScaleBoost", value);
            }
        }



        private static int LimitBetween(int value, int minValue, int maxValue, bool allowZero) {
            if (allowZero && (value == 0)) { return 0; }
            if (value < minValue) { return minValue; }
            if (value > maxValue) { return maxValue; }
            return value;
        }

    }
}
