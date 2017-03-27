#### Word-based password generator ####

Bimil uses word list extracted from WordNet database using the following
commands:

	egrep -v "^  " data.* | cut -d' ' -f5 | egrep "^[a-z]{4,6}$" | grep -v '\(.\)\1' | sort | uniq > English.words

This extracts all unique words between 4 and 6 characters without any
consecutive repeating characters.


It also uses list of the most common names for the year 2000 onward, available
from United States Social Security Administration
(https://www.ssa.gov/OACT/babynames/limits.html), extracted using the following
commands:

    awk 'BEGIN {FS=","} {if ($3>42) print tolower($1)}' yob200*.txt yob201*.txt | egrep "^[a-z]{4,6}$" | grep -v '\(.\)\1' | egrep -v "bd|bh|hk|hn|kh" | sort | uniq > Names.words

This extracts unique names used more than 42 times in any given year with length
between 4 and 6 characters without any consecutive repeating characters. It also
removes some character combinations that might be a bit harder to pronounce.


Additionally, it uses list of US geographical feature as found on US Board On
Geographic Names (http://geonames.usgs.gov/domestic/download_data.htm),
extracted using the following commands:

	cut -d"|" -f2 NationalFile_*.txt | tr '[:upper:]' '[:lower:]' | egrep "^[a-z]{4,6}$" | grep -v '\(.\)\1' | sort | uniq -c | sort -nrk1 | awk '{if ($1>1) print $2}' | sort > GeoFeatures.words

This extracts unique names with length between 4 and 6 characters without any
consecutive repeating characters and with more than one appearance.


Moreover, King James Bible (https://www.gutenberg.org/ebooks/30) is also a
source of words. It has been included as it it offers nice selection of archaic
but recognizable English words not really found elsewhere and it is a public
domain. Words have been extracted using the following commands:

	cat pg30.txt | tr -cs '[:alnum:]' '\n' | tr '[:upper:]' '[:lower:]' | egrep "^[a-z]{4,6}$" | grep -v '\(.\)\1' | egrep -v "bd|bh|hk|hn|kh" | sort | uniq -c | sort -nrk1 | awk '{if ($1>1) print $2}' | sort > Bible.words

This extracts unique words with length between 4 and 6 characters without any
consecutive repeating characters and with more than one appearance. It also
removes some character combinations that might be a bit harder to pronounce.


Further, the works of William Shakespeare (https://www.gutenberg.org/ebooks/100)
have been used as additional word source. These books are in the public domain
and offer nice selection of less commonly used words that everybody finds
recognizable nonetheless. Words have been extracted using the following command:

	cat pg100.txt | tr -cs '[:alnum:]' '\n' | tr '[:upper:]' '[:lower:]' | egrep "^[a-z]{4,6}$" | egrep -v "bd|bh|hk|hn|kh" | sort | uniq -c | sort -nrk1 | awk '{if ($1>1) print $2}' | sort > WilliamShakespeare.words

This extracts unique words with length between 4 and 7 characters and with more
than one appearance. It also removes some character combinations that might be a
bit harder to pronounce.


Lastly, the works of Jane Austen (https://www.gutenberg.org/ebooks/31100) have
been used as a word source. Books are also in the public domain and offer small
but nice selection of the less commonly used words. Words have been extracted
using the following command:

	cat pg31100.txt | tr -cs '[:alnum:]' '\n' | tr '[:upper:]' '[:lower:]' | egrep "^[a-z]{4,6}$" | egrep -v "bd|bh|hk|hn|kh" | sort | uniq -c | sort -nrk1 | awk '{if ($1>1) print $2}' | sort > JaneAusten.words

This extracts unique words with length between 4 and 7 characters and with more
than one appearance. It also removes some character combinations that might be a
bit harder to pronounce.
