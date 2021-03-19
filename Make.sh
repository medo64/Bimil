#!/bin/bash

FILE_PROJECT="Bimil.pro"
FILE_EXECUTABLE="Bimil.exe"
FILE_DISTRIBUTION="Makefile Make.sh *.md src/"

QT_PATH='/c/Qt'
QT_VERSION=6.0.2

CERTIFICATE_THUMBPRINT="e9b444fffb1375ece027e40d8637b6da3fdaaf0e"
TIMESTAMP_URL="http://timestamp.digicert.com"  #http://timestamp.comodoca.com/rfc3161


if [ -t 1 ]; then
    ANSI_RESET="$(tput sgr0)"
    ANSI_UNDERLINE="$(tput smul)"
    ANSI_RED="`[ $(tput colors) -ge 16 ] && tput setaf 9 || tput setaf 1 bold`"
    ANSI_YELLOW="`[ $(tput colors) -ge 16 ] && tput setaf 11 || tput setaf 3 bold`"
    ANSI_CYAN="`[ $(tput colors) -ge 16 ] && tput setaf 14 || tput setaf 6 bold`"
    ANSI_WHITE="`[ $(tput colors) -ge 16 ] && tput setaf 15 || tput setaf 7 bold`"

    ANSI_ERROR="$ANSI_RED"
    ANSI_WARNING="$ANSI_YELLOW"
    ANSI_INFO="$ANSI_WHITE"
    ANSI_RESULT="$ANSI_CYAN"
fi

while getopts ":h" OPT; do
    case $OPT in
        h)
            echo
            echo    "  SYNOPSIS"
            echo -e "  $(basename "$0") [${ANSI_UNDERLINE}operation${ANSI_RESET}]"
            echo
            echo -e "    ${ANSI_UNDERLINE}operation${ANSI_RESET}"
            echo    "    Operation to perform (all, clean, debug, release, test)."
            echo
            echo    "  DESCRIPTION"
            echo    "  Make script compatible with both Windows and Linux."
            echo
            echo    "  SAMPLES"
            echo    "  $(basename "$0")"
            echo    "  $(basename "$0") all"
            echo
            exit 0
        ;;

        \?) echo "${ANSI_ERROR}Invalid option: -$OPTARG!${ANSI_RESET}" >&2 ; exit 1 ;;
        :)  echo "${ANSI_ERROR}Option -$OPTARG requires an argument!${ANSI_RESET}" >&2 ; exit 1 ;;
    esac
done


DIST_TARGET=`egrep '^TARGET[ =]' src/$FILE_PROJECT | head -1 | cut -d'=' -f2 | awk '{print $$1}' | tr -d '"' | tr -d '\r' | xargs`
DIST_VERSION=`egrep '^APP_VERSION[ =]' src/$FILE_PROJECT | head -1 | cut -d'=' -f2 | awk '{print $$1}' | tr -d '"' | tr -d '\r' | xargs`


trap "exit 255" SIGHUP SIGINT SIGQUIT SIGPIPE SIGTERM
trap "echo -n \"$ANSI_RESET\"" EXIT

BASE_DIRECTORY="$( cd "$(dirname "$0")" >/dev/null 2>&1 ; pwd -P )"


function clean() {
    rm -r "$BASE_DIRECTORY/bin/" 2>/dev/null
    rm -r "$BASE_DIRECTORY/build/" 2>/dev/null
    rm -r "$BASE_DIRECTORY/build-*/" 2>/dev/null
    return 0
}

function distclean() {
    rm -r "$BASE_DIRECTORY/dist/" 2>/dev/null
    rm -r "$BASE_DIRECTORY/target/" 2>/dev/null
    return 0
}

function dist() {
    rm -r "$BASE_DIRECTORY/build/dist/" 2>/dev/null
    mkdir -p "$BASE_DIRECTORY/build/dist/$DIST_TARGET-$DIST_VERSION/"
    cp -r $FILE_DISTRIBUTION "$BASE_DIRECTORY/build/dist/$DIST_TARGET-$DIST_VERSION/"
    tar -cz -C "$BASE_DIRECTORY/build/dist/"  --owner=0 --group=0 -f "$BASE_DIRECTORY/build/dist/$DIST_TARGET-$DIST_VERSION.tar.gz" "$DIST_TARGET-$DIST_VERSION/"
    mkdir -p "$BASE_DIRECTORY/dist/"
    mv "$BASE_DIRECTORY/build/dist/$DIST_TARGET-$DIST_VERSION.tar.gz" "$BASE_DIRECTORY/dist/"

    if [[ -f "$BASE_DIRECTORY/dist/$DIST_TARGET-$DIST_VERSION.tar.gz" ]]; then
        echo "${ANSI_RESULT}Output in '$BASE_DIRECTORY/dist/$DIST_TARGET-$DIST_VERSION.tar.gz'${ANSI_RESET}"
        return 0
    else
        return 1
    fi
}

function build() {  # configuration
    BUILD_CONFIG=${1-release}

    rm bin/* 2> /dev/null
    rm -r bin/platforms 2> /dev/null
    rm -r bin/styles 2> /dev/null
    mkdir -p bin
    rm -R build/ 2> /dev/null
    mkdir -p build
    cd build

    if [[ `uname -o` == "Msys" ]]; then  # assume Windows

        CMD_QMAKE=`ls $QT_PATH/$QT_VERSION/mingw*/bin/qmake.exe 2>/dev/null | sort | tail -1`
        CMD_MAKE=`ls $QT_PATH/Tools/**/bin/mingw32-make.exe 2>/dev/null | sort | tail -1`

        if [[ ! -f "$CMD_QMAKE" ]]; then
            echo -e "${ANSI_ERROR}Cannot find qmake!${ANSI_RESET}" >&2
            return 1
        fi
        QMAKE_DIR=`dirname $CMD_QMAKE`

        if [[ ! -f "$CMD_MAKE" ]]; then
            echo -e "${ANSI_ERROR}Cannot find make!${ANSI_RESET}" >&2
            return 1
        fi

        OPENSSL_DIR=$QT_PATH/Tools/QtCreator/bin
        if [[ ! -f "$OPENSSL_DIR/libcrypto-1_1-x64.dll" ]] || [[ ! -f "$OPENSSL_DIR/libssl-1_1-x64.dll" ]]; then
            echo -e "${ANSI_ERROR}Cannot find OpenSSL files.${ANSI_RESET}" >&2
            return 1
        fi

        CMD_SIGNTOOL=""
        for SIGNTOOL_PATH in "/c/Program Files (x86)/Microsoft SDKs/ClickOnce/SignTool/signtool.exe" \
                             "/c/Program Files (x86)/Windows Kits/10/App Certification Kit/signtool.exe" \
                             "/c/Program Files (x86)/Windows Kits/10/bin/x86/signtool.exe"; do
            if [[ -f "$SIGNTOOL_PATH" ]]; then
                CMD_SIGNTOOL="$SIGNTOOL_PATH"
                break
            fi
        done

        if [[ ! -f "$CMD_SIGNTOOL" ]]; then
            echo -e "${ANSI_WARNING}Cannot find signtool!${ANSI_RESET}" >&2
        fi

        echo "QMake directory ...: ${ANSI_INFO}$QMAKE_DIR${ANSI_RESET}"
        echo "QMake executable ..: ${ANSI_INFO}$CMD_QMAKE${ANSI_RESET}"
        echo "Make executable ...: ${ANSI_INFO}$CMD_MAKE${ANSI_RESET}"
        echo "OpenSSL libraries .: ${ANSI_INFO}$OPENSSL_DIR${ANSI_RESET}"
        echo "SignTool executable: ${ANSI_INFO}$CMD_SIGNTOOL${ANSI_RESET}"
        echo

        PATH=$PATH:`dirname $CMD_MAKE`

        $CMD_QMAKE --version
        $CMD_QMAKE -spec win32-g++ CONFIG+=release ../src/$FILE_PROJECT
        if [[ $? -ne 0 ]]; then
            echo -e "${ANSI_ERROR}QMake failed!${ANSI_RESET}" >&2
            return 1
        fi

        $CMD_MAKE -f Makefile
        if [[ $? -ne 0 ]]; then
            echo -e "${ANSI_ERROR}Make failed!${ANSI_RESET}" >&2
            return 1
        fi

        cp release/$FILE_EXECUTABLE                  ../bin/$FILE_EXECUTABLE
        cp $QMAKE_DIR/libgcc_s_seh-1.dll             ../bin/
        cp $QMAKE_DIR/libstdc++-6.dll                ../bin/
        cp $QMAKE_DIR/libwinpthread-1.dll            ../bin/
        cp $QMAKE_DIR/Qt${QT_VERSION:0:1}Core.dll    ../bin/
        cp $QMAKE_DIR/Qt${QT_VERSION:0:1}Gui.dll     ../bin/
        cp $QMAKE_DIR/Qt${QT_VERSION:0:1}Network.dll ../bin/
        cp $QMAKE_DIR/Qt${QT_VERSION:0:1}Widgets.dll ../bin/
        cp $OPENSSL_DIR/libcrypto-1_1-x64.dll        ../bin/
        cp $OPENSSL_DIR/libssl-1_1-x64.dll           ../bin/

        mkdir ../bin/platforms
        cp $QMAKE_DIR/../plugins/platforms/qwindows.dll ../bin/platforms/

        mkdir ../bin/styles
        cp $QMAKE_DIR/../plugins/styles/qwindowsvistastyle.dll ../bin/styles/

        if [[ "$BUILD_CONFIG" == "release" ]]; then
            if [[ "$CERTIFICATE_THUMBPRINT" != "" ]] && [[ -f "$CMD_SIGNTOOL" ]]; then
                echo
                if [[ "$TIMESTAMP_URL" != "" ]]; then
                    "$CMD_SIGNTOOL" sign -s "My" -sha1 $CERTIFICATE_THUMBPRINT -tr $TIMESTAMP_URL -v ../bin/$FILE_EXECUTABLE
                else
                    "$CMD_SIGNTOOL" sign -s "My" -sha1 $CERTIFICATE_THUMBPRINT -v ../bin/$FILE_EXECUTABLE
                fi
            fi
        fi

    else  # Linux

        if command -v qmake >/dev/null; then
            CMD_QMAKE="qmake"
        else
            echo "${ANSI_ERROR}No 'qmake' in path, consider installing 'qtbase5-dev' package!${ANSI_RESET}" >&2
            return 1
        fi

        if ! test -d /usr/share/doc/libqt5x11extras5-dev >/dev/null; then
            echo "${ANSI_ERROR}X11 extras not found, consider installing 'libqt5x11extras5-dev' package!${ANSI_RESET}" >&2
            return 1
        fi

        rm -r "$BASE_DIRECTORY/build/releases/" 2>/dev/null
        mkdir -p "$BASE_DIRECTORY/build/release/"
        cd "$BASE_DIRECTORY/build/release/" \
            && qmake -qt=qt5 CONFIG+=$BUILD_CONFIG "../../src/$FILE_PROJECT" ; make
        mkdir -p "$BASE_DIRECTORY/bin/"
        cp "$BASE_DIRECTORY/build/release/$DIST_TARGET" "$BASE_DIRECTORY/bin/$DIST_TARGET"

    fi

    echo
    echo "${ANSI_RESULT}Output in 'bin/'${ANSI_RESET}"
    return 0
}


if [ $# -gt 1 ]; then
    echo "${ANSI_ERROR}Too many arguments!${ANSI_RESET}" >&2
    exit 1
fi

OK=0
OPERATION="$1"
if [[ "$OPERATION" == "" ]]; then OPERATION="all"; fi

case "$OPERATION" in
    all)        clean && OK=1 ; build 'release' && OK=1 ;;
    clean)      clean && OK=1 ;;
    distclean)  distclean && OK=1 ;;
    dist)       distclean && dist && OK=1 ;;
    debug)      build 'debug' && OK=1 ;;
    release)    build 'release' && OK=1 ;;

    *)  echo "${ANSI_ERROR}Unknown operation '$OPERATION'!${ANSI_RESET}" >&2 ; exit 1 ;;
esac

if ! [[ $OK  ]]; then
    echo "${ANSI_ERROR}Error performing '$OPERATION' operation!${ANSI_RESET}" >&2
    exit 1
fi
