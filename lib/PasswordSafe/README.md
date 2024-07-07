This library allows for reading and writing Password Safe version 3 files.

For example usage do check [Bimil][bimil].

You can find packaged library at [NuGet][nuget] and add it you your application
using the following command:

    dotnet add package PasswordSafe



#### Auto-type ####

Auto-type functionality will simulate keyboard presses. Following escape
characters are supported:

    \u      User name field.
    \p      Password field.
    \2      Two-factor authentication code.
    \cn     Credit card number.
    \ct     Credit card number (tab separated).
    \ce     Credit card expiration.
    \cv     Credit card security code.
    \cp     Credit card pin number.
    \g      Group field.
    \i      Title field.
    \l      URL field.
    \m      E-mail field.
    \o      Notes field.
    \o###   Nth line of Notes field. If line doesn't exist, it has no effect.
    \b      Backpace key.
    \t      Tab key.
    \s      Shift+Tab key.
    \n      Enter key.
    \\      Backslash (\) key.
    \d###   Delay between characters in milliseconds, instead of 10 (default).
    \w###   Wait in milliseconds.
    \W###   Wait in seconds.
    \z      Invokes the alternative SendKeys method.
            All other text is typed as-is. 


[bimil]: https://www.medo64.com/bimil
[nuget]: https://www.nuget.org/packages/PasswordSafe
