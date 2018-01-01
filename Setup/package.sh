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


if [[ ! "$(uname -a)" =~ "GNU/Linux" ]]
then
    echo "Must run under Linux!" >&2
    exit 1
fi

which dpkg-deb &> /dev/null
if [ ! $? -eq 0 ]
then
    echo "Package dpkg-deb not installed!" >&2
    exit 1
fi


DIRECTORY_ROOT=`mktemp -d`

terminate() {
    rm -R $DIRECTORY_ROOT 2> /dev/null
}
trap terminate INT EXIT


if [ "$EUID" -ne 0 ]
then
    echo "Must run as root (try sudo)!" >&2
    exit 1
fi


DIRECTORY_RELEASE="../Releases"
FILE_RELEASE="$1"
PATH_RELEASE="$DIRECTORY_RELEASE/$FILE_RELEASE"

if [[ ! $PATH_RELEASE =~ \.zip$ ]]
then
    echo "Release file $FILE_RELEASE does not end in .zip!" >&2
    exit 1
fi

if [ ! -e "$PATH_RELEASE" ]
then
    echo "Cannot find release file $FILE_RELEASE!" >&2
    exit 1
fi


RELEASE_VERSION=`echo $FILE_RELEASE | sed -e s/[^0-9]//g`

if (( ${#RELEASE_VERSION} != 3 ))
then
    echo "Cannot extract version from $FILE_RELEASE!" >&2
    exit 1
fi

RELEASE_VERSION_MAJOR=`echo -n $RELEASE_VERSION | head -c 1`
RELEASE_VERSION_MINOR=`echo -n $RELEASE_VERSION | tail -c 2`
RELEASE_ARCHITECTURE=`grep "Architecture:" ./DEBIAN/control | sed "s/Architecture://" | sed "s/[^a-z]//g"`

PACKAGE_NAME="bimil_${RELEASE_VERSION_MAJOR}.${RELEASE_VERSION_MINOR}_${RELEASE_ARCHITECTURE}"
DIRECTORY_PACKAGE="$DIRECTORY_ROOT/$PACKAGE_NAME"

mkdir $DIRECTORY_PACKAGE
cp -R ./DEBIAN $DIRECTORY_PACKAGE/
cp -R ./usr $DIRECTORY_PACKAGE/

find $DIRECTORY_PACKAGE -type d -exec chmod 755 {} +
find $DIRECTORY_PACKAGE -type f -exec chmod 644 {} +
chmod 755 $DIRECTORY_PACKAGE/DEBIAN/*inst
chmod 755 $DIRECTORY_PACKAGE/DEBIAN/*rm

sed -i "s/MAJOR/$RELEASE_VERSION_MAJOR/" $DIRECTORY_PACKAGE/DEBIAN/control
sed -i "s/MINOR/$RELEASE_VERSION_MINOR/" $DIRECTORY_PACKAGE/DEBIAN/control

mkdir -p "$DIRECTORY_PACKAGE/opt/bimil"
unzip -LL "$PATH_RELEASE" -d "$DIRECTORY_PACKAGE/opt/bimil" > /dev/null
if [ ! $? -eq 0 ]
then
    echo "Cannot extract archive!" >&2
    exit 1
fi
chmod 755 "$DIRECTORY_PACKAGE/opt/bimil/bimil.exe"

dpkg-deb --build $DIRECTORY_PACKAGE > /dev/null

cp "$DIRECTORY_ROOT/$PACKAGE_NAME.deb" $DIRECTORY_RELEASE
if [ $? -eq 0 ]
then
    echo "Package $DIRECTORY_RELEASE/$PACKAGE_NAME.deb successfully created." >&2
else
    echo "Didn't find output Debian package!" >&2
    exit 1
fi
