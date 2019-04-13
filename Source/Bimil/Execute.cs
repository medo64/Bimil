using System;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace Bimil {
    internal static class Execute {

        public static void StartCommand(string commandText, IWin32Window owner) {
            var startInfo = GetStartInfo(commandText);

            try {
                Process.Start(startInfo);
            } catch (InvalidOperationException ex) { //inproper call
                if (owner != null) {
                    Medo.MessageBox.ShowError(owner, "Cannot parse command.\n\n" + ex.Message);
                } else {
                    throw;
                }
            } catch (SystemException ex) {
                if (owner != null) {
                    Medo.MessageBox.ShowError(owner, ex.Message);
                } else {
                    throw;
                }
            }
        }

        public static ProcessStartInfo GetStartInfo(string commandText) {
            var sbFileName = new StringBuilder();
            var sbArguments = new StringBuilder();

            var state = StartInfoState.Default;
            foreach (var ch in commandText.Trim()) {
                switch (state) {
                    case StartInfoState.Default:
                        if (ch == '\"') {
                            state = StartInfoState.QuotedFileName;
                        } else {
                            sbFileName.Append(ch);
                            state = StartInfoState.FileName;
                        }
                        break;

                    case StartInfoState.FileName:
                        if ((ch == ' ') || (ch == '\"')) {
                            state = StartInfoState.Arguments;
                        } else {
                            sbFileName.Append(ch);
                        }
                        break;

                    case StartInfoState.QuotedFileName:
                        if (ch == '\"') {
                            state = StartInfoState.Arguments;
                        } else {
                            sbFileName.Append(ch);
                        }
                        break;

                    case StartInfoState.Arguments:
                        sbArguments.Append(ch);
                        break;
                }
            }

            var fileName = FillEnvironment(sbFileName.ToString().Trim());
            var arguments = FillEnvironment(sbArguments.ToString().Trim());

            return new ProcessStartInfo(fileName, arguments);
        }

        private enum StartInfoState {
            Default,
            FileName,
            QuotedFileName,
            Arguments,
        }

        private static string FillEnvironment(string text) {
            var sb = new StringBuilder();
            var sbVariable = new StringBuilder();

            var state = EnvironmentState.Default;
            foreach (var ch in text) {
                switch (state) {
                    case EnvironmentState.Default:
                        if (ch == '%') {
                            state = EnvironmentState.PercentVariable;
                        } else {
                            sb.Append(ch);
                        }
                        break;

                    case EnvironmentState.PercentVariable:
                        if (ch == '%') {
                            if (sbVariable.Length == 0) { //just double percent
                                sb.Append("%");
                            } else {
                                var value = Environment.GetEnvironmentVariable(sbVariable.ToString().Trim());
                                if (value != null) {
                                    sb.Append(value);
                                } else { //just copy it all if environment variable is not found
                                    sb.Append("%" + sbVariable.ToString() + "%");
                                }
                                sbVariable.Length = 0;
                            }
                            state = EnvironmentState.Default;
                        } else {
                            sbVariable.Append(ch);
                        }
                        break;
                }
            }

            return sb.ToString();
        }

        private enum EnvironmentState {
            Default,
            PercentVariable,
        }

        public static void StartUrl(string urlText) {
            var url = NormalizeUrl(urlText);
            if (url != "") { Process.Start(url); }
        }

        public static string NormalizeUrl(string urlText) {
            var url = urlText.Trim();
            if (url.Length > 0) {
                return (url.IndexOf("://", StringComparison.OrdinalIgnoreCase) > 0) ? url : ("http://" + url);
            } else {
                return "";
            }
        }
    }
}
