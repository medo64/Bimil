#!/bin/bash
#2024-07-07
BASE_DIRECTORY="$( cd "$(dirname "$0")" >/dev/null 2>&1 ; pwd -P )"

PACKAGE_CONTENT_FILES="Makefile Make.sh CHANGELOG.md ICON.png LICENSE.md README.md .editorconfig"
PACKAGE_CONTENT_DIRECTORIES="src/ lib/"


if [ -t 1 ]; then
    ANSI_RESET="$(tput sgr0)"
    ANSI_UNDERLINE="$(tput smul)"
    ANSI_RED="`[ $(tput colors) -ge 16 ] && tput setaf 9 || tput setaf 1 bold`"
    ANSI_GREEN="`[ $(tput colors) -ge 16 ] && tput setaf 10 || tput setaf 2 bold`"
    ANSI_YELLOW="`[ $(tput colors) -ge 16 ] && tput setaf 11 || tput setaf 3 bold`"
    ANSI_MAGENTA="`[ $(tput colors) -ge 16 ] && tput setaf 13 || tput setaf 5 bold`"
    ANSI_CYAN="`[ $(tput colors) -ge 16 ] && tput setaf 14 || tput setaf 6 bold`"
    ANSI_WHITE="`[ $(tput colors) -ge 16 ] && tput setaf 15 || tput setaf 7 bold`"
    ANSI_PURPLE="$(tput setaf 5)"
fi

while getopts ":h" OPT; do
    case $OPT in
        h)
            echo
            echo    "  SYNOPSIS"
            echo -e "  $(basename "$0") [${ANSI_UNDERLINE}operation${ANSI_RESET}]"
            echo
            echo -e "    ${ANSI_UNDERLINE}operation${ANSI_RESET}"
            echo    "    Operation to perform."
            echo
            echo    "  DESCRIPTION"
            echo    "  Make script compatible with both Windows and Linux."
            echo
            echo    "  SAMPLES"
            echo    "  $(basename "$0")"
            echo    "  $(basename "$0") dist"
            echo
            exit 0
        ;;

        \?) echo "${ANSI_RED}Invalid option: -$OPTARG!${ANSI_RESET}" >&2 ; exit 1 ;;
        :)  echo "${ANSI_RED}Option -$OPTARG requires an argument!${ANSI_RESET}" >&2 ; exit 1 ;;
    esac
done

trap "exit 255" SIGHUP SIGINT SIGQUIT SIGPIPE SIGTERM
trap "echo -n \"$ANSI_RESET\"" EXIT


if ! command -v dotnet >/dev/null; then
    echo "${ANSI_RED}No dotnet found!${ANSI_RESET}" >&2
    exit 1
fi

case `uname` in
    Linux)        RUNTIME="linux-x64" ;;
    MINGW64_NT-*) RUNTIME="win-x64"   ;;
    *)
        echo "${ANSI_RED}Unsupported runtime (`uname`)!${ANSI_RESET}" >&2
        exit 1
esac

echo ".NET `dotnet --version` ($RUNTIME)"


for PROJECT_FILE in $(find $BASE_DIRECTORY/src/ -name "*.csproj" | sort); do
    if [[ "$PROJECT_NAME" == "" ]]; then
        PROJECT_NAME=`cat "$PROJECT_FILE" | grep "<Product>" | sed 's^</\?Product>^^g' | xargs`
        if [[ "$PROJECT_NAME" == "" ]]; then continue; fi
        PROJECT_ASSEMBLY=`cat "$PROJECT_FILE" | grep "<AssemblyName>" | sed 's^</\?AssemblyName>^^g' | xargs`
        PROJECT_OUTPUTTYPE=`cat "$PROJECT_FILE" | grep "<OutputType>" | sed 's^</\?OutputType>^^g' | xargs`
        PROJECT_VERSION=`cat "$PROJECT_FILE" | grep "<Version>" | sed 's^</\?Version>^^g' | xargs`
        PROJECT_FRAMEWORK=`cat "$PROJECT_FILE" | grep "<TargetFramework>" | sed 's^</\?TargetFramework\?>^^g' | tr ';' ' ' | xargs`
        echo "$PROJECT_FILE"
        echo "$PROJECT_NAME $PROJECT_VERSION ($PROJECT_FRAMEWORK)"
        break
    fi
done
if [[ "$PROJECT_NAME" == "" ]] || [[ "$PROJECT_ASSEMBLY" == "" ]] || [[ "$PROJECT_VERSION" == "" ]] || [[ "$PROJECT_FRAMEWORK" == "" ]]; then
    echo "${ANSI_RED}Cannot determine project data!${ANSI_RESET}" >&2
    exit 1
fi


function clean() {
    echo
    echo "${ANSI_MAGENTA}clean${ANSI_RESET}"

    find "$BASE_DIRECTORY/bin/" -mindepth 1 -delete
    find "$BASE_DIRECTORY/build/" -mindepth 1 -delete
    find "$BASE_DIRECTORY/src" -type d \( -name "bin" -o -name "obj" \) -exec rm -rf {} + 2>/dev/null
    find "$BASE_DIRECTORY/test" -type d \( -name "bin" -o -name "obj" -o -name "TestResults" \) -exec rm -rf {} + 2>/dev/null

    echo "${ANSI_CYAN}Done${ANSI_RESET}"
    return 0
}

function distclean() {
    echo
    echo "${ANSI_MAGENTA}distclean${ANSI_RESET}"

    rm -r "$BASE_DIRECTORY/bin/" 2>/dev/null
    rm -r "$BASE_DIRECTORY/build/" 2>/dev/null
    rm -r "$BASE_DIRECTORY/dist/" 2>/dev/null
    rm -r "$BASE_DIRECTORY/target/" 2>/dev/null

    echo "${ANSI_CYAN}Done${ANSI_RESET}"
    return 0
}

function dist() {
    echo
    echo "${ANSI_MAGENTA}dist${ANSI_RESET}"

    DIST_DIRECTORY="$BASE_DIRECTORY/build/dist"
    DIST_SUBDIRECTORY="$DIST_DIRECTORY/$PROJECT_NAME-$PROJECT_VERSION"

    DIST_FILE=
    rm -r "$DIST_SUBDIRECTORY/" 2>/dev/null
    mkdir -p "$DIST_SUBDIRECTORY/"
    for DIRECTORY in $PACKAGE_CONTENT_FILES $PACKAGE_CONTENT_DIRECTORIES; do
        cp -r "$BASE_DIRECTORY/$DIRECTORY" "$DIST_SUBDIRECTORY/"
    done
    find "$DIST_SUBDIRECTORY/" -name ".vs" -type d -exec rm -rf {} \; 2>/dev/null
    find "$DIST_SUBDIRECTORY/" -name "bin" -type d -exec rm -rf {} \; 2>/dev/null
    find "$DIST_SUBDIRECTORY/" -name "obj" -type d -exec rm -rf {} \; 2>/dev/null
    find "$DIST_SUBDIRECTORY/" -name "TestResults" -type d -exec rm -rf {} \; 2>/dev/null

    tar -cz -C "$BASE_DIRECTORY/build/dist/" \
        --owner=0 --group=0 \
        -f "$DIST_SUBDIRECTORY.tar.gz" \
        "$PROJECT_NAME-$PROJECT_VERSION/" || return 1
    mkdir -p "$BASE_DIRECTORY/dist/"
    mv "$DIST_SUBDIRECTORY.tar.gz" "$BASE_DIRECTORY/dist/" || return 1

    echo "${ANSI_GREEN}Output at ${ANSI_CYAN}dist/$PROJECT_NAME-$PROJECT_VERSION.tar.gz${ANSI_RESET}"
    return 0
}

function build() {
    BUILD_CONFIG="$1"
    if [[ "$BUILD_CONFIG" == "" ]]; then BUILD_CONFIG="debug"; fi

    echo
    echo "${ANSI_MAGENTA}$BUILD_CONFIG${ANSI_RESET}"

    BASE_NAME=$(basename "$PROJECT_FILE" | sed 's/.csproj$//')
    BASE_DIR=$(dirname "$PROJECT_FILE")

    mkdir -p "$BASE_DIRECTORY/build/$RUNTIME/$BUILD_CONFIG/"
    find "$BASE_DIRECTORY/build/$RUNTIME/$BUILD_CONFIG/" -mindepth 1 -delete

	dotnet publish                             \
        "$BASE_DIR"                            \
        --configuration $BUILD_CONFIG          \
        --output build/$RUNTIME/$BUILD_CONFIG/ \
        --self-contained true                  \
        -r $RUNTIME                            \
        -p:DebugType=embedded                  \
        -p:PublishReadyToRun=true              \
        -p:PublishSingleFile=true              \
        || return 1

    mkdir -p "$BASE_DIRECTORY/bin/$RUNTIME/"
    find "$BASE_DIRECTORY/bin/$RUNTIME/" -mindepth 1 -delete
    find "$BASE_DIRECTORY/build/$RUNTIME/$BUILD_CONFIG/" -type f -exec cp {} "$BASE_DIRECTORY/bin/$RUNTIME/" \; 2>/dev/null
    if [[ "$RUNTIME" == "linux-x64" ]]; then
        mv "$BASE_DIRECTORY/bin/$RUNTIME/$PROJECT_ASSEMBLY" "$BASE_DIRECTORY/bin/$RUNTIME/${PROJECT_ASSEMBLY,,}"
        PROJECT_ASSEMBLY="${PROJECT_ASSEMBLY,,}"
    fi

    echo
    echo "${ANSI_GREEN}Output in ${ANSI_CYAN}$BASE_DIRECTORY/bin/$RUNTIME/${ANSI_RESET}"

    if [[ "$PROJECT_OUTPUTTYPE" == "WinExe" ]]; then
        EXECUTABLE=
        if [[ -e "$BASE_DIRECTORY/bin/$RUNTIME/$PROJECT_ASSEMBLY.exe" ]]; then
            EXECUTABLE="$BASE_DIRECTORY/bin/$RUNTIME/$PROJECT_ASSEMBLY.exe"
        elif [[ -e "$BASE_DIRECTORY/bin/$RUNTIME/$PROJECT_ASSEMBLY" ]]; then
            EXECUTABLE="$BASE_DIRECTORY/bin/$RUNTIME/$PROJECT_ASSEMBLY"
        else
            echo "${ANSI_RED}Executable not found!${ANSI_RESET}" >&2
            return 1
        fi
        echo "${ANSI_GREEN}Executing ${ANSI_CYAN}$EXECUTABLE${ANSI_RESET}"
        "$EXECUTABLE"
    fi
}

function test() {
    echo
    echo "${ANSI_MAGENTA}test${ANSI_RESET}"

    mkdir -p "$BASE_DIRECTORY/build/test/"
    find "$BASE_DIRECTORY/build/test/" -mindepth 1 -delete

    FOUND_TEST_PROJECTS=0
    for TEST_PROJECT_FILE in $(find $BASE_DIRECTORY/test/ -name "*.csproj" | sort); do
        FOUND_TEST_PROJECTS=1
        echo ; echo "${ANSI_MAGENTA}$TEST_PROJECT_FILE${ANSI_RESET}"

        BASE_NAME=$(basename "$TEST_PROJECT_FILE" | sed 's/.csproj$//')
        mkdir -p "$BASE_DIRECTORY/build/test/$BASE_NAME/"

        rm -r $BASE_DIRECTORY/src/bin 2>/dev/null
        dotnet test "$TEST_PROJECT_FILE" \
                    --configuration "Debug" \
                    --verbosity "minimal" \
                    || return 1
    done

    if [[ $FOUND_TEST_PROJECTS -eq 0 ]]; then
        echo "${ANSI_RED}No test project file found!${ANSI_RESET}" >&2
        return 1
    fi

    echo
    echo "${ANSI_GREEN}Testing completed${ANSI_RESET}"
}

function run() {
    echo
    echo "${ANSI_MAGENTA}run${ANSI_RESET}"
    PROJECT_DIR=$(dirname "$PROJECT_FILE")

    pushd "$PROJECT_DIR" >/dev/null
    dotnet restore && dotnet run --no-restore --verbosity minimal
    popd >/dev/null
}


while [ $# -gt 0 ]; do
    OPERATION="$1"
    case "$OPERATION" in
        clean)      clean               || break ;;
        distclean)  clean && distclean  || break ;;
        dist)       dist                || break ;;
        debug)      build "debug"       || break ;;
        release)    build "release"     || break ;;
        run)        run                 || break ;;
        test)       test                || break ;;

        *)  echo "${ANSI_RED}Unknown operation '$OPERATION'!${ANSI_RESET}" >&2 ; exit 1 ;;
    esac

    shift
done

if [[ "$1" != "" ]]; then
    echo "${ANSI_RED}Error performing '$OPERATION' operation!${ANSI_RESET}" >&2
    exit 1
fi
