## Word-based Password Generator Word Sources

### Banned Word List

Bimil uses list of banned words from [Banned Word List](http://www.bannedwordlist.com/)
website. [Downloaded file](http://www.bannedwordlist.com/lists/swearWords.txt)
is adjusted for case.

```bash
cat swearWords.txt | tr -d ' ' | tr -d "\r" | tr '[:upper:]' '[:lower:]' | sort | uniq > ../Bad.txt
```

### WordNet

Bimil uses word list extracted from [WordNet database](https://wordnet.princeton.edu/download/current-version)
website. [Downloaded file](https://wordnetcode.princeton.edu/3.0/WNprolog-3.0.tar.gz)
is extracted and only `data*` files are used. All unique words between 4 and 6
characters without any consecutive repeating characters and without more than
two consecutive consonants are then extracted.

```bash
egrep -v "^  " data.* | cut -d' ' -f5 | egrep "^[a-z]{4,6}$" | grep -v '\(.\)\1' | egrep -v "[^aeiou][^aeiou][^aeiouy]" | sort | uniq > ../English.tmp
comm -23 ../English.tmp ../Bad.txt > ../English.txt
```

### Popular Baby Names

Bimil also uses list of the most common names for the year 2000 onward, available
from United States [Social Security Administration](https://www.ssa.gov/OACT/babynames/limits.html)
website. [Downloaded file](https://www.ssa.gov/OACT/babynames/names.zip) is
extracted and only files from 2000 onward are used. Unique names used more than
113 times in any given year, with length between 4 and 6 characters, and without
any consecutive consonants or vowels, are extracted.

```bash
awk 'BEGIN {FS=","} {if ($3>113) print tolower($1)}' yob20*.txt | egrep "^[a-z]{4,6}$" | egrep -v "[^aeiou][^aeiou]" | egrep -v "[aeiou][aeiou]" | sort | uniq > ../Names.tmp
comm -23 ../Names.tmp ../Bad.txt > ../Names.txt
```

### Books

#### King James Bible

Moreover, [King James Bible](https://www.gutenberg.org/ebooks/30) is also a
source of words. It has been included as it it offers nice selection of archaic
but recognizable English words not really found elsewhere and it is a public
domain. [Downloaded file](https://www.gutenberg.org/ebooks/30.txt.utf-8) is
split into words and all unique words with length between 4 and 6 characters,
without any consecutive repeating characters, without an uppercase, and with
more than three appearances, are extracted. It also removes some character
combinations that might be a bit harder to pronounce.

```bash
cat pg30.txt | tr '[:blank:]' '\n' | egrep "^[a-z]{4,6}$" | grep -v '\(.\)\1' | egrep -v "[^aeiou][^aeiou][^aeiouy]" | egrep -v "bd|bh|cq|hk|hn|kh|rd|est|eth|tch" | sort | uniq -c | sort -nrk1 | awk '{if ($1>3) print $2}' | sort > ../Bible.tmp
comm -23 ../Bible.tmp ../Bad.txt > ../Bible.txt
```

#### William Shakespeare

Further, the works of [William Shakespeare](https://www.gutenberg.org/ebooks/100)
have been used as additional word source. These books are in the public domain
and offer nice selection of less commonly used words that everybody finds
recognizable nonetheless. Extraction from [downloaded file](https://www.gutenberg.org/ebooks/100.txt.utf-8)
includes unique words with length between 4 and 6 characters, without any
consecutive repeating characters, without an uppercase, and with more than three
appearances. It also removes some character combinations that might be a bit
harder to pronounce.

```bash
cat pg100.txt | tr '[:blank:]' '\n' | egrep "^[a-z]{4,6}$" | grep -v '\(.\)\1' | egrep -v "[^aeiou][^aeiou][^aeiouy]" | egrep -v "bd|bh|cq|hk|hn|kh|rd|est|eth|tch" | sort | uniq -c | sort -nrk1 | awk '{if ($1>3) print $2}' | sort > ../WilliamShakespeare.tmp
comm -23 ../WilliamShakespeare.tmp ../Bad.txt > ../WilliamShakespeare.txt
```

#### Jane Austen

Lastly, the works of [Jane Austen] (https://www.gutenberg.org/ebooks/31100) have
been used as a word source. Books are also in the public domain and offer small
but nice selection of the less commonly used words. Extraction from
[downloaded file](https://www.gutenberg.org/ebooks/31100.txt.utf-8) is extracted
for unique words with length between 4 and 6 characters, without any
consecutive repeating characters, without an uppercase, and with more than two
appearances. It also removes some character combinations that might be a bit
harder to pronounce.

```bash
cat pg31100.txt | tr '[:blank:]' '\n' | egrep "^[a-z]{4,6}$" | grep -v '\(.\)\1' | egrep -v "[^aeiou][^aeiou][^aeiouy]" | egrep -v "bd|bh|cq|hk|hn|kh|rd|est|eth|tch" | sort | uniq -c | sort -nrk1 | awk '{if ($1>2) print $2}' | sort > ../JaneAusten.tmp
comm -23 ../JaneAusten.tmp ../Bad.txt > ../JaneAusten.txt
```

### Common Passwords

#### 10K

List of 10,000 most used passwords is also part of Bimil that comes courtesy of
[Mark Burnett](https://xato.net). [Downloaded file](https://xato.net/today-i-am-releasing-ten-million-passwords-b6278bbe7495#.j5omx1nqb)
is used just as an additional filter and not to create any new passwords.

#### 100K

List of 100,000 most used passwords is also part of Bimil that comes courtesy of
[Have I Been Pwned](https://haveibeenpwned.com) project. [Downloaded file](https://www.ncsc.gov.uk/static-assets/documents/PwnedPasswordsTop100k.txt)
is used just as an additional filter and not to create any new passwords.

```bash
cat PwnedPasswordsTop100k.txt | tr -d '\r' | tr -d ' ' | sed '/^$/d' | sed '0,/--/d' | sort | uniq > ../CommonPasswords100K.tmp
(tr -d '\01-\11\13-\37' < ../CommonPasswords100K.tmp) | sed '/^$/d' > ../CommonPasswords100K.txt
```
