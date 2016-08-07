### Bimil ###

Small password manager.

Saved files are based on Password Safe file format with a few custom fields.



#### Word-based password generator ####

Bimil uses word list extracted from WordNet database using the following
commands:

    egrep -v "^  " data.* | cut -d' ' -f5 | egrep "^[a-z]{4,7}$" | sort | uniq > Words.txt

This extracts all unique words between 4 and 7 characters.


It also uses list of the most common names for the year 2000 onward, available
from United States Social Security Administration
(https://www.ssa.gov/oact/babynames/limits.html), extracted using the following
commands:

    awk 'BEGIN {FS=","} {if ($3>23) print tolower($1)}' yob20*.txt | egrep "^[a-z]{4,7}$" | sort | uniq > Names.txt

This extracts unique names used more than 23 times in any given year with length
between 4 and 7 characters.

    

#### External code copyrights ####

QR Coder: Copyright (c) 2013-2015 Raffael Herrmann
WordNet 3.0: Copyright 2006 by Princeton University. All rights reserved.
