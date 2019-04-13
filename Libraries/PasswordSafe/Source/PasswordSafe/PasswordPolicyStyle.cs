using System;

namespace Medo.Security.Cryptography.PasswordSafe {
    /// <summary>
    /// Password policy flags.
    /// </summary>
    public enum PasswordPolicyStyle {
        /// <summary>
        /// Lowercase characters are to be used.
        /// </summary>
        UseLowercase = 0x8000,
        /// <summary>
        /// Uppercase characters are to be used.
        /// </summary>
        UseUppercase = 0x4000,
        /// <summary>
        /// Digits are to be used.
        /// </summary>
        UseDigits = 0x2000,
        /// <summary>
        /// Symbols are to be used.
        /// </summary>
        UseSymbols = 0x1000,
        /// <summary>
        /// Hexadecimal digits are to be used. If set, no other flags can be set.
        /// </summary>
        UseHexDigits = 0x0800,
        /// <summary>
        /// Easy-vision is to be used.
        /// </summary>
        UseEasyVision = 0x0400,
        /// <summary>
        /// Resulting password should be adjusted to be more pronounceable.
        /// </summary>
        MakePronounceable = 0x0200,
    }
}
