using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Medo.Security.Cryptography.PasswordSafe {
    /// <summary>
    /// Token representing either one key or command.
    /// </summary>
    [DebuggerDisplay("{Text}")]
    public class AutotypeToken {

        internal AutotypeToken(string content)
            : this(content, AutotypeTokenKind.Key) {
        }

        internal AutotypeToken(string content, AutotypeTokenKind type) {
            this.Content = content;
            this.Kind = type;
        }


        /// <summary>
        /// Gets text.
        /// </summary>
        public string Content { get; }

        /// <summary>
        /// Gets token type.
        /// </summary>
        public AutotypeTokenKind Kind { get; }


        /// <summary>
        /// Returns Text if token is Key, otherwise Command:Argument.
        /// </summary>
        public override string ToString() {
            return this.Content;
        }


        /// <summary>
        /// Returns individual tokens from the text.
        /// </summary>
        /// <param name="content">Text.</param>
        public static IEnumerable<AutotypeToken> GetIndividualKeyTokens(string content) {
            foreach (var ch in content) {
                switch (ch) {
                    case '+':
                    case '^':
                    case '%':
                    case '~':
                    case '(':
                    case ')':
                    case '{':
                    case '}':
                    case '[':
                    case ']': yield return new AutotypeToken("{" + ch + "}"); break;
                    case '\b': yield return new AutotypeToken("{Backspace}"); break;
                    case '\n':
                    case '\r': yield return new AutotypeToken("{Enter}"); break;
                    case '\t': yield return new AutotypeToken("{Tab}"); break;
                    default: yield return new AutotypeToken(ch.ToString()); break;
                }
            }
        }

    }
}
