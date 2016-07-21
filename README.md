### Bimil ###

Small password manager.

Saved files are based on Password Safe file format with a few custom fields.



#### Word-based password generator ####

Bimil uses word list extracted from WordNet 3.0 using following commands:

    egrep -v "^  " data.* | cut -d' ' -f5 | egrep "^[a-z]{5,7}$" | sort | uniq > words.txt

This extracts all unique words between 5 and 7 characters.



#### External code copyrights ####

QR Coder: Copyright (c) 2013-2015 Raffael Herrmann
WordNet 3.0: Copyright 2006 by Princeton University. All rights reserved.
