#!/bin/bash

while getopts ":h" OPT
do
    case $OPT in
        h)
            echo
            echo    "  SYNOPSIS"
            echo -e "  `echo $0 | xargs basename` \033[4mfile\033[0m" | fmt
            echo
            echo -e "    \033[4mfile\033[0m"
            echo    "    File from release directory." | fmt
            echo
            echo    "  DESCRIPTION"
            echo    "  Creates debian package based on zipped release file." | fmt
            echo
            echo    "  SAMPLES"
            echo    "  $0 bimil210.zip" | fmt
            echo
            exit 0
        ;;

        \?)
            echo "Invalid option: -$OPTARG!" >&2
            exit 1
        ;;

        :)
            echo "Option -$OPTARG requires an argument!" >&2
            exit 1
        ;;
    esac
done

shift $(( OPTIND - 1 ))

if [[ "$2" != "" ]]
then
    echo "Too many arguments!" >&2
    exit 1
fi


DIRECTORY_RELEASE="../Releases"
FILE_RELEASE="$1"

if [ ! -f "$DIRECTORY_RELEASE/$FILE_RELEASE" ]
then
    echo "Cannot find release file $FILE_RELEASE!" >&2
    exit 1
fi


FILE_RELEASE_VERSION=`echo $FILE_RELEASE | sed -e s/[^0-9]//g`

if (( ${#FILE_RELEASE_VERSION} != 3 ))
then
    echo "Cannot extract version from $FILE_RELEASE!" >&2
    exit 1
fi

FILE_RELEASE_VERSION_MAJOR=`echo -n $FILE_RELEASE_VERSION | head -c 1`
FILE_RELEASE_VERSION_MINOR=`echo -n $FILE_RELEASE_VERSION | tail -c 2`


DIRECTORY_ROOT=`mktemp -d`
PACKAGE_NAME="bimil-$FILE_RELEASE_VERSION_MAJOR.$FILE_RELEASE_VERSION_MINOR"
DIRECTORY_PACKAGE="$DIRECTORY_ROOT/$PACKAGE_NAME"

mkdir $DIRECTORY_PACKAGE
cp -R ./DEBIAN $DIRECTORY_PACKAGE/
cp -R ./usr $DIRECTORY_PACKAGE/
chmod -R 755 $DIRECTORY_PACKAGE

sed -i "s/MAJOR/$FILE_RELEASE_VERSION_MAJOR/" $DIRECTORY_PACKAGE/DEBIAN/control
sed -i "s/MINOR/$FILE_RELEASE_VERSION_MINOR/" $DIRECTORY_PACKAGE/DEBIAN/control

mkdir -p "$DIRECTORY_PACKAGE/opt/bimil"
unzip -LL "$DIRECTORY_RELEASE/$FILE_RELEASE" -d "$DIRECTORY_PACKAGE/opt/bimil"
chmod +x "$DIRECTORY_PACKAGE/opt/bimil/bimil.exe"

dpkg-deb --build $DIRECTORY_PACKAGE > /dev/null

cp "$DIRECTORY_ROOT/$PACKAGE_NAME.deb" $DIRECTORY_RELEASE

rm -R $DIRECTORY_ROOT
