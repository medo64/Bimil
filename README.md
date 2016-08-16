### Bimil ###

Small password manager.

Saved files are based on Password Safe file format with a few custom fields.



#### Auto-type ####

Auto-type functionality will simulate keyboard presses. Following escape
characters are supported:

    \u      User name field.
    \p      Password field.
    \2      Two-factor authentication code.
    \cn     Credit card number.
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



#### Word-based password generator ####

Bimil uses word list extracted from WordNet database using the following
commands:

    egrep -v "^  " data.* | cut -d' ' -f5 | egrep "^[a-z]{4,7}$" | sort | uniq > Words.txt

This extracts all unique words between 4 and 7 characters.


It also uses list of the most common names for the year 2000 onward, available
from United States Social Security Administration
(https://www.ssa.gov/oact/babynames/limits.html), extracted using the following
commands:

    awk 'BEGIN {FS=","} {if ($3>23) print tolower($1)}' yob20*.txt | egrep "^[a-z]{4,7}$" | grep -v '\(.\)\1' | sort | uniq > Names.txt

This extracts unique names used more than 23 times in any given year with length
between 4 and 7 characters without any consecutive repeating characters.


Additionally, it uses list of US geographical feature as found on US Board On
Geographic Names (http://geonames.usgs.gov/domestic/download_data.htm),
extracted using the following commands:

    cut -d"|" -f2 NationalFile_*.txt | tr '[:upper:]' '[:lower:]' | egrep "^[a-z]{4,7}$" | grep -v '\(.\)\1' | sort | uniq > GeoFeatures.txt

This extracts unique names with length between 4 and 7 characters without any
consecutive repeating characters.


Moreover, King James Bible (https://www.gutenberg.org/ebooks/30) is also a
source of words. It has been included as it it offers nice selection of archaic
but recognizable English words not really found elsewhere and it is a public
domain. Words have been extracted using the following commands:

    cat pg30.txt | tr -cs '[:alnum:]' '\n' | tr '[:upper:]' '[:lower:]' | egrep "^[a-z]{4,7}$" | grep -v '\(.\)\1' | sort | uniq > Bible.txt

This extracts unique words with length between 4 and 7 characters without any
consecutive repeating characters.


Further, the works of William Shakespeare (https://www.gutenberg.org/ebooks/100)
have been used as additional word source. These books are in the public domain
and offer nice selection of less commonly used words that everybody finds
recognizable nonetheless. Words have been extracted using the following command:

    cat pg100.txt | tr -cs '[:alnum:]' '\n' | tr '[:upper:]' '[:lower:]' | egrep "^[a-z]{4,7}$" | sort | uniq > WilliamShakespeare.txt

This extracts unique words with length between 4 and 7 characters.


Lastly, the works of Jane Austen (https://www.gutenberg.org/ebooks/31100) have
been used as a word source. Books are also in the public domain and offer small
but nice selection of the less commonly used words. Words have been extracted
using the following command:

    cat pg31100.txt | tr -cs '[:alnum:]' '\n' | tr '[:upper:]' '[:lower:]' | egrep "^[a-z]{4,7}$" | sort | uniq > JaneAusten.txt

This extracts unique words with length between 4 and 7 characters.



#### External code copyrights and references ####

QR Coder:
Copyright (c) 2013-2015 Raffael Herrmann

WordNet 3.0:
Copyright 2006 by Princeton University. All rights reserved.

Popular Baby Names (United States Social Security Administration):
Public Domain

US Board On Geographic Names (States, Territories, Associated Areas of the United States):
Public Domain

The Bible, King James Version, Complete (Project Gutenberg):
Public Domain, The Project Gutenberg License

The Complete Works of William Shakespeare (Project Gutenberg):
Public Domain, The Project Gutenberg License, Produced by World Library, Inc., from their Library of the Future

The Complete Works of Jane Austen (Project Gutenberg):
Public Domain, The Project Gutenberg License
