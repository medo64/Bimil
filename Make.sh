#!/bin/sh
#~ .NET Project
SCRIPT_DIR="$( cd -- "$(dirname "$0")" >/dev/null 2>&1 ; pwd -P )"
SCRIPT_NAME=`basename $0`

if [ -t 1 ]; then
    ANSI_RESET="$(tput sgr0)"
    ANSI_RED="`[ $(tput colors) -ge 16 ] && tput setaf 9 || tput setaf 1 bold`"
    ANSI_YELLOW="`[ $(tput colors) -ge 16 ] && tput setaf 11 || tput setaf 3 bold`"
    ANSI_MAGENTA="`[ $(tput colors) -ge 16 ] && tput setaf 13 || tput setaf 5 bold`"
    ANSI_PURPLE="$(tput setaf 5)"
    ANSI_CYAN="`[ $(tput colors) -ge 16 ] && tput setaf 14 || tput setaf 6 bold`"
fi

if [ "$1" = "--help" ] || [ "$1" = "-h" ]; then
    echo "Usage: $SCRIPT_NAME [target]..."
    echo
    echo "Targets:"
    echo "  clean      Clean all build artifacts"
    echo "  run        Run the project"
    echo "  test       Run tests"
    echo "  benchmark  Run benchmarks"
    echo "  examples   Compile examples"
    echo "  debug      Compile in debug mode"
    echo "  release    Compile in release mode"
    echo "  package    Package the project"
    echo "  publish    Publish the project"
    echo "  tools      Compile tools"
    echo
    echo "Actions with '~' prefix are negated"
    echo
    echo "Examples:"
    echo "  make release         - Compile in release mode"
    echo "  make ~clean release  - Compile in release mode without cleaning"
    echo
    exit 0
fi


if ! [ -e "$SCRIPT_DIR/.meta" ]; then
    echo "${ANSI_RED}Meta file not found${ANSI_RESET}" >&2
    exit 113
fi

if ! command -v git >/dev/null; then
    echo "${ANSI_YELLOW}Missing git command${ANSI_RESET}"
fi


HAS_CHANGES=$( git status -s 2>&1 | wc -l )
if [ "$HAS_CHANGES" -gt 0 ]; then
    echo "${ANSI_YELLOW}Uncommitted changes present${ANSI_RESET}"
fi


PROJECT_NAME=$( cat "$SCRIPT_DIR/.meta" | grep -E "^PROJECT_NAME:" | sed  -n 1p | cut -d: -sf2- | xargs )
if [ "$PROJECT_NAME" = "" ]; then
    echo "${ANSI_PURPLE}Project name ........: ${ANSI_RED}not found${ANSI_RESET}"
    exit 113
fi
PROJECT_NAME_LOWER=$( echo "$PROJECT_NAME" | tr '[:upper:]' '[:lower:]' )
echo "${ANSI_PURPLE}Project name ........: ${ANSI_MAGENTA}$PROJECT_NAME${ANSI_RESET}"

GIT_INDEX=$( git rev-list --count HEAD 2>/dev/null )
if [ "$GIT_INDEX" = "" ]; then GIT_INDEX=0; fi

GIT_HASH=$( git log -n 1 --format=%h 2>/dev/null )
if [ "$GIT_HASH" = "" ]; then GIT_HASH=alpha; fi

GIT_VERSION=$( git tag --points-at HEAD 2>/dev/null | grep -E '^v[0-9]+\.[0-9]+\.[0-9]+$' | sed -n 1p | sed 's/^v//g' | xargs )
if [ "$GIT_VERSION" != "" ]; then
    if [ "$HAS_CHANGES" -eq 0 ]; then
        ASSEMBLY_VERSION_TEXT="$GIT_VERSION"
    else
        ASSEMBLY_VERSION_TEXT="$GIT_VERSION+$GIT_HASH"
    fi
else
    ASSEMBLY_VERSION_TEXT="0.0.0+$GIT_HASH"
fi

if [ "$GIT_VERSION" != "" ]; then
    echo "${ANSI_PURPLE}Git tag version .....: ${ANSI_MAGENTA}$GIT_VERSION${ANSI_RESET}"
else
    echo "${ANSI_PURPLE}Git tag version .....: ${ANSI_MAGENTA}-${ANSI_RESET}"
fi

if [ "$GIT_VERSION" != "" ]; then
    ASSEMBLY_VERSION="$GIT_VERSION.$GIT_INDEX"
else
    ASSEMBLY_VERSION="0.0.0.$GIT_INDEX"
fi
echo "${ANSI_PURPLE}Assembly version ....: ${ANSI_MAGENTA}$ASSEMBLY_VERSION${ANSI_RESET}"
echo "${ANSI_PURPLE}Assembly version text: ${ANSI_MAGENTA}$ASSEMBLY_VERSION_TEXT${ANSI_RESET}"

PROJECT_ENTRYPOINT=$( cat "$SCRIPT_DIR/.meta" | grep -E "^PROJECT_ENTRYPOINT:" | sed  -n 1p | cut -d: -sf2- | xargs )
if [ "$PROJECT_ENTRYPOINT" = "" ]; then  # auto-detect
    PROJECT_ENTRYPOINT=$( find "$SCRIPT_DIR/src" -type f -name "*.csproj" -print | sed -n 1p )
    PROJECT_ENTRYPOINT=$( echo "$PROJECT_ENTRYPOINT" | sed "s|$SCRIPT_DIR/||g" )
fi
if [ "$PROJECT_ENTRYPOINT" != "" ] && [ -e "$SCRIPT_DIR/$PROJECT_ENTRYPOINT" ]; then
    echo "${ANSI_PURPLE}Project entry point .: ${ANSI_MAGENTA}$PROJECT_ENTRYPOINT${ANSI_RESET}"
else
    echo "${ANSI_PURPLE}Project entry point .: ${ANSI_RED}not found${ANSI_RESET}" >&2
    exit 113
fi

PROJECT_OUTPUTTYPE=$( cat "$SCRIPT_DIR/$PROJECT_ENTRYPOINT" | grep -E "<OutputType>" | sed -n 1p | sed -E "s|.*<OutputType>(.*)</OutputType>.*|\1|g" | xargs | tr '[:upper:]' '[:lower:]' )
if [ "$PROJECT_OUTPUTTYPE" != "" ]; then
    echo "${ANSI_PURPLE}Project output type .: ${ANSI_MAGENTA}$PROJECT_OUTPUTTYPE${ANSI_RESET}"
else
    echo "${ANSI_PURPLE}Project output type .: ${ANSI_RED}cannot determine${ANSI_RESET}" >&2
    exit 113
fi

PROJECT_SINGLEFILE=$( cat "$SCRIPT_DIR/.meta" | grep -E "^PROJECT_SINGLEFILE:" | sed  -n 1p | cut -d: -sf2- | xargs | tr '[:upper:]' '[:lower:]' )
if [ "$PROJECT_SINGLEFILE" = "true" ] || [ "$PROJECT_SINGLEFILE" = "false" ]; then
    echo "${ANSI_PURPLE}Project single-file .: ${ANSI_MAGENTA}$PROJECT_SINGLEFILE${ANSI_RESET}"
elif [ "$PROJECT_OUTPUTTYPE" = "exe" ] || [ "$PROJECT_OUTPUTTYPE" = "winexe" ]; then
    PROJECT_SINGLEFILE=true
    echo "${ANSI_PURPLE}Project single-file .: ${ANSI_MAGENTA}$PROJECT_SINGLEFILE${ANSI_RESET}"
elif [ "$PROJECT_OUTPUTTYPE" = "library" ]; then  # libraries cannot be published as a single file
    PROJECT_SINGLEFILE=false
    echo "${ANSI_PURPLE}Project single-file .: ${ANSI_MAGENTA}$PROJECT_SINGLEFILE${ANSI_RESET}"
else
    PROJECT_SINGLEFILE=
fi

PROJECT_RUNTIMES=$( cat "$SCRIPT_DIR/.meta" | grep -E "^PROJECT_RUNTIMES:" | sed  -n 1p | cut -d: -sf2- | xargs )
if [ "$PROJECT_RUNTIMES" = "" ]; then
    PROJECT_RUNTIMES=current
fi
echo "${ANSI_PURPLE}Project runtimes ....: ${ANSI_MAGENTA}$PROJECT_RUNTIMES${ANSI_RESET}"


DOCKER_FILE="$(find "$SCRIPT_DIR/src" -type f -name "Dockerfile" -print | sed -n 1p)"

PACKAGE_LINUX_DOCKER=$( cat "$SCRIPT_DIR/.meta" | grep -E "^PACKAGE_LINUX_DOCKER:" | sed  -n 1p | cut -d: -sf2- | xargs )
if [ "$PACKAGE_LINUX_DOCKER" = "" ] && [ "$DOCKER_FILE" != "" ]; then
    PACKAGE_LINUX_DOCKER=$( echo $PROJECT_NAME | tr [:upper:] [:lower:] )
fi
if [ "$PACKAGE_LINUX_DOCKER" != "" ]; then
    if [ "$DOCKER_FILE" != "" ]; then
        echo "${ANSI_PURPLE}Docker source .......: ${ANSI_MAGENTA}$DOCKER_FILE${ANSI_RESET}"
    else
        echo "${ANSI_PURPLE}Docker source .......: ${ANSI_RED}not found${ANSI_RESET}" >&2
        exit 113
    fi
    echo "${ANSI_PURPLE}Docker local image ..: ${ANSI_MAGENTA}$PACKAGE_LINUX_DOCKER${ANSI_RESET}"

    PUBLISH_LINUX_DOCKER=$( cat "$SCRIPT_DIR/.meta.private" 2>/dev/null | grep -E "^PUBLISH_LINUX_DOCKER:" | sed  -n 1p | cut -d: -sf2- | xargs )
    if [ "$PUBLISH_LINUX_DOCKER" != "" ]; then
        if [ "$PACKAGE_LINUX_DOCKER" = "" ]; then
            echo "${ANSI_PURPLE}Docker remote image .: ${ANSI_RED}not found${ANSI_RESET}" >&2
            exit 113
        fi

        DOCKER_IMAGE_ID=$( echo "$PUBLISH_LINUX_DOCKER" | cut -d/ -f1 )
        DOCKER_IMAGE_NAME=$( echo "$PUBLISH_LINUX_DOCKER" | cut -d/ -sf2 )
        if [ "$DOCKER_IMAGE_ID" != "" ] && [ "$DOCKER_IMAGE_NAME" = "" ]; then
            DOCKER_IMAGE_NAME="$PACKAGE_LINUX_DOCKER"
        fi
        if [ "$DOCKER_IMAGE_ID" != "" ] && [ "$DOCKER_IMAGE_NAME" != "" ]; then
            echo "${ANSI_PURPLE}Docker remote image .: ${ANSI_MAGENTA}$DOCKER_IMAGE_ID/$DOCKER_IMAGE_NAME${ANSI_RESET}"
        else
            echo "${ANSI_PURPLE}Docker remote image .: ${ANSI_RED}not found${ANSI_RESET}" >&2
            exit 113
        fi

        if [ -d ".meta.docker" ]; then
            echo "${ANSI_PURPLE}Docker config .......: ${ANSI_MAGENTA}.meta.docker${ANSI_RESET}"
        else
            echo "${ANSI_PURPLE}Docker config .......: ${ANSI_MAGENTA}default${ANSI_RESET}"
        fi
    fi
fi


PUBLISH_LINUX_ARCHIVE=$( cat "$SCRIPT_DIR/.meta.private" 2>/dev/null | grep -E "^PUBLISH_LINUX_ARCHIVE:" | sed  -n 1p | cut -d: -sf2- | xargs )
if [ "$PUBLISH_LINUX_ARCHIVE" = "" ]; then
    echo "${ANSI_PURPLE}Archive remote ......: ${ANSI_YELLOW}(not configured)${ANSI_RESET}" >&2
else
    echo "${ANSI_PURPLE}Archive remote ......: ${ANSI_MAGENTA}$PUBLISH_LINUX_ARCHIVE${ANSI_RESET}"
fi


PACKAGE_LINUX_APPIMAGE=$( cat "$SCRIPT_DIR/.meta" | grep -E "^PACKAGE_LINUX_APPIMAGE:" | sed  -n 1p | cut -d: -sf2- | xargs )
if [ "$PACKAGE_LINUX_APPIMAGE" = "" ]; then  # auto-detect
    if [ -d "$SCRIPT_DIR/packaging/linux-appimage" ] && [ -d "$SCRIPT_DIR/packaging/linux-deb" ]; then
        PACKAGE_LINUX_APPIMAGE=$(basename "$SCRIPT_DIR/packaging/linux-deb/usr/share/applications"/*.desktop .desktop)
    fi
fi
if [ "$PACKAGE_LINUX_APPIMAGE" != "" ]; then
    echo "${ANSI_PURPLE}AppImage ............: ${ANSI_MAGENTA}$PACKAGE_LINUX_APPIMAGE${ANSI_RESET}"

    PUBLISH_LINUX_APPIMAGE=$( cat "$SCRIPT_DIR/.meta.private" 2>/dev/null | grep -E "^PUBLISH_LINUX_APPIMAGE:" | sed  -n 1p | cut -d: -sf2- | xargs )
    if [ "$PUBLISH_LINUX_APPIMAGE" = "" ]; then
        echo "${ANSI_PURPLE}AppImage remote .....: ${ANSI_YELLOW}(not configured)${ANSI_RESET}" >&2
    else
        echo "${ANSI_PURPLE}AppImage remote .....: ${ANSI_MAGENTA}$PUBLISH_LINUX_APPIMAGE${ANSI_RESET}"
    fi
fi


PACKAGE_LINUX_DEB=$( cat "$SCRIPT_DIR/.meta" | grep -E "^PACKAGE_LINUX_DEB:" | sed  -n 1p | cut -d: -sf2- | xargs )
if [ "$PACKAGE_LINUX_DEB" = "" ]; then  # auto-detect
    if [ -d "$SCRIPT_DIR/packaging/linux-deb" ]; then
        PACKAGE_LINUX_DEB=$PROJECT_NAME
    fi
fi
if [ "$PACKAGE_LINUX_DEB" != "" ]; then
    echo "${ANSI_PURPLE}Debian package ......: ${ANSI_MAGENTA}$PACKAGE_LINUX_DEB${ANSI_RESET}"

    PUBLISH_LINUX_DEB=$( cat "$SCRIPT_DIR/.meta.private" 2>/dev/null | grep -E "^PUBLISH_LINUX_DEB:" | sed  -n 1p | cut -d: -sf2- | xargs )
    if [ "$PUBLISH_LINUX_DEB" = "" ]; then
        echo "${ANSI_PURPLE}Debian package remote: ${ANSI_YELLOW}(not configured)${ANSI_RESET}" >&2
    else
        echo "${ANSI_PURPLE}Debian package remote: ${ANSI_MAGENTA}$PUBLISH_LINUX_APPIMAGE${ANSI_RESET}"
    fi
fi


PACKAGE_NUGET=$( cat "$SCRIPT_DIR/.meta" | grep -E "^PACKAGE_NUGET:" | sed  -n 1p | cut -d: -sf2- | xargs )
if [ "$PACKAGE_NUGET" = "" ]; then  # auto-detect
    if [ "$PROJECT_OUTPUTTYPE" = "library" ]; then
        PACKAGE_NUGET=$PROJECT_NAME
    fi
fi
if [ "$PACKAGE_NUGET" != "" ]; then
    echo "${ANSI_PURPLE}NuGET package .......: ${ANSI_MAGENTA}$PACKAGE_NUGET${ANSI_RESET}"

    PACKAGE_NUGET_ENTRYPOINT=$( cat "$SCRIPT_DIR/.meta" | grep -E "^PACKAGE_NUGET_ENTRYPOINT:" | sed  -n 1p | cut -d: -sf2- | xargs )
    if [ "$PACKAGE_NUGET_ENTRYPOINT" = "" ]; then
        PACKAGE_NUGET_ENTRYPOINT=$PROJECT_ENTRYPOINT
    fi
    echo "${ANSI_PURPLE}NuGET entry point ...: ${ANSI_MAGENTA}$PACKAGE_NUGET_ENTRYPOINT${ANSI_RESET}"

    PACKAGE_NUGET_ID=`cat "$PACKAGE_NUGET_ENTRYPOINT" | grep "<PackageId>" | sed 's^</\?PackageId>^^g' | xargs`
    if [ "$PACKAGE_NUGET_ID" = "" ]; then
        PACKAGE_NUGET_ID=$PROJECT_NAME
    fi
    echo "${ANSI_PURPLE}NuGET package ID ....: ${ANSI_MAGENTA}$PACKAGE_NUGET_ID${ANSI_RESET}"

    PACKAGE_NUGET_VERSION=`cat "$PACKAGE_NUGET_ENTRYPOINT" | grep "<Version>" | sed 's^</\?Version>^^g' | xargs`
    if [ "$PACKAGE_NUGET_VERSION" = "" ]; then
        PACKAGE_NUGET_VERSION=$ASSEMBLY_VERSION_TEXT
    fi
    echo "${ANSI_PURPLE}NuGET package version: ${ANSI_MAGENTA}$PACKAGE_NUGET_VERSION${ANSI_RESET}"

    PACKAGE_NUGET_FRAMEWORKS=`cat "$PACKAGE_NUGET_ENTRYPOINT" | grep "<TargetFramework" | sed 's^</\?TargetFrameworks\?>^^g' | tr ';' ' ' | xargs`
    if [ "$PACKAGE_NUGET_FRAMEWORKS" = "" ]; then
        echo "${ANSI_PURPLE}NuGET frameworks ....: ${ANSI_YELLOW}(not configured)${ANSI_RESET}" >&2
    else
        echo "${ANSI_PURPLE}NuGET frameworks ....: ${ANSI_MAGENTA}$PACKAGE_NUGET_FRAMEWORKS${ANSI_RESET}"
    fi

    PUBLISH_NUGET_KEY=$( cat "$SCRIPT_DIR/.meta.private" 2>/dev/null | grep -E "^PUBLISH_NUGET_KEY:" | sed  -n 1p | cut -d: -sf2- | xargs )
    if [ "$PUBLISH_NUGET_KEY" = "" ]; then
        echo "${ANSI_PURPLE}NuGET package key ...: ${ANSI_YELLOW}(not configured)${ANSI_RESET}" >&2
    else
        echo "${ANSI_PURPLE}NuGET package key ...: ${ANSI_MAGENTA}(configured)${ANSI_RESET}"
    fi
fi


prereq_compile() {
    if ! command -v dotnet >/dev/null; then
        echo "${ANSI_RED}Missing dotnet command${ANSI_RESET}" >&2
        exit 113
    fi
}

prereq_package() {
    if [ "$PACKAGE_LINUX_DOCKER" != "" ]; then
        if ! command -v docker >/dev/null; then
            echo "${ANSI_RED}Missing docker command${ANSI_RESET}" >&2
            exit 113
        fi
    fi

    if [ "$PACKAGE_LINUX_APPIMAGE" != "" ]; then
        if ! [ -d "$SCRIPT_DIR/packaging/linux-appimage" ]; then
            echo "${ANSI_RED}Missing linux-appimage directory${ANSI_RESET}" >&2
            exit 113
        fi
        if ! [ -d "$SCRIPT_DIR/packaging/linux-deb" ]; then
            echo "${ANSI_RED}Missing linux-deb directory${ANSI_RESET}" >&2
            exit 113
        fi
        if ! command -v appimagetool-x86_64.AppImage >/dev/null; then
            echo "${ANSI_RED}Missing appimagetool-x86_64.AppImage${ANSI_RESET}" >&2
            exit 113
        fi
    fi

    if [ "$PACKAGE_LINUX_DEB" != "" ]; then
        if ! [ -d "$SCRIPT_DIR/packaging/linux-deb" ]; then
            echo "${ANSI_RED}Missing linux-deb directory${ANSI_RESET}" >&2
            exit 113
        fi
        if ! [ -e "$SCRIPT_DIR/packaging/linux-deb/usr/share/applications"/*.desktop ]; then
            echo "${ANSI_RED}Missing desktip file${ANSI_RESET}" >&2
            exit 113
        fi
        if ! [ -e "$SCRIPT_DIR/packaging/linux-deb/usr/share/icons/hicolor/128x128/apps"/*.png ]; then
            echo "${ANSI_RED}Missing icon files${ANSI_RESET}" >&2
            exit 113
        fi
        if ! command -v dpkg-deb >/dev/null; then
            echo "${ANSI_RED}Missing dpkg-deb command (dpkg-deb package)${ANSI_RESET}" >&2
            exit 113
        fi
        if ! command -v fakeroot >/dev/null; then
            echo "${ANSI_RED}Missing fakeroot command${ANSI_RESET}" >&2
            exit 113
        fi
        if ! command -v gzip >/dev/null; then
            echo "${ANSI_RED}Missing gzip command${ANSI_RESET}" >&2
            exit 113
        fi
        if ! command -v lintian >/dev/null; then
            echo "${ANSI_RED}Missing lintian command (lintian package)${ANSI_RESET}" >&2
            exit 113
        fi
        if ! command -v strip >/dev/null; then
            echo "${ANSI_RED}Missing strip command${ANSI_RESET}" >&2
            exit 113
        fi
    fi
}

make_clean() {
    echo
    echo "${ANSI_MAGENTA}┏━━━━━━━┓${ANSI_RESET}"
    echo "${ANSI_MAGENTA}┃ CLEAN ┃${ANSI_RESET}"
    echo "${ANSI_MAGENTA}┗━━━━━━━┛${ANSI_RESET}"
    echo

    find "$SCRIPT_DIR/bin" -mindepth 1 -delete 2>/dev/null || true
    rmdir "$SCRIPT_DIR/bin" 2>/dev/null || true
    find "$SCRIPT_DIR/build" -mindepth 1 -delete 2>/dev/null || true
    rmdir "$SCRIPT_DIR/build" 2>/dev/null || true
    find "$SCRIPT_DIR/examples/bin" -mindepth 1 -delete 2>/dev/null || true
    rmdir "$SCRIPT_DIR/examples/bin" 2>/dev/null || true
    find "$SCRIPT_DIR/tools/bin" -mindepth 1 -delete 2>/dev/null || true
    rmdir "$SCRIPT_DIR/tools/bin" 2>/dev/null || true

    find "$SCRIPT_DIR/src" -type d \( -name "bin" -or -name "obj" \) -exec rm -rf "{}" + 2>/dev/null || true
    find "$SCRIPT_DIR/tests" -type d \( -name "bin" -or -name "obj" \) -exec rm -rf "{}" + 2>/dev/null || true
    find "$SCRIPT_DIR/tests" -type d -name "BenchmarkDotNet.Artifacts" -exec rm -rf "{}" + 2>/dev/null || true
    find "$SCRIPT_DIR/examples" -type d \( -name "bin" -or -name "obj" \) -exec rm -rf "{}" + 2>/dev/null || true
    find "$SCRIPT_DIR/tools" -type d \( -name "bin" -or -name "obj" \) -exec rm -rf "{}" + 2>/dev/null || true
}

make_run() {
    echo
    echo "${ANSI_MAGENTA}┏━━━━━┓${ANSI_RESET}"
    echo "${ANSI_MAGENTA}┃ RUN ┃${ANSI_RESET}"
    echo "${ANSI_MAGENTA}┗━━━━━┛${ANSI_RESET}"
    echo

    echo "${ANSI_MAGENTA}$(basename $PROJECT_ENTRYPOINT)${ANSI_RESET}"
    if [ "$PROJECT_OUTPUTTYPE" = "exe" ] || [ "$PROJECT_OUTPUTTYPE" = "winexe" ]; then
        cd $( dirname "$SCRIPT_DIR/$PROJECT_ENTRYPOINT" )
        dotnet run                                       \
            -p:EnableNETAnalyzers=false                  \
            --project "$SCRIPT_DIR/$PROJECT_ENTRYPOINT"
    else
        echo "${ANSI_RED}Nothing to run${ANSI_RESET}" >&2
        exit 113
    fi
}

make_test() {
    echo
    echo "${ANSI_MAGENTA}┏━━━━━━┓${ANSI_RESET}"
    echo "${ANSI_MAGENTA}┃ TEST ┃${ANSI_RESET}"
    echo "${ANSI_MAGENTA}┗━━━━━━┛${ANSI_RESET}"
    echo

    if [ ! -e "$SCRIPT_DIR/tests" ]; then
        echo "${ANSI_YELLOW}No tests found${ANSI_RESET}" >&2
        echo
        return 0
    fi

    ANYTHING_DONE=0

    for PROJECT_FILE in $(find "$SCRIPT_DIR/tests" -name "*.csproj"); do
        IS_TEST=$(cat "$PROJECT_FILE" | grep -E "MSTest.Sdk" | wc -l)
        if [ $IS_TEST -eq 0 ]; then continue; fi

        ANYTHING_DONE=1
        echo "${ANSI_MAGENTA}$(basename $PROJECT_FILE)${ANSI_RESET}"

        dotnet test                                \
            -p:TestingPlatformCaptureOutput=false  \
            -p:EnableNETAnalyzers=false            \
            -l "console;verbosity=detailed"        \
            --verbosity detailed                   \
            "$PROJECT_FILE"                       || exit 113
        echo
    done

    if [ "$ANYTHING_DONE" -eq 0 ]; then
        echo "${ANSI_RED}No test project found${ANSI_RESET}" >&2
        exit 113
    fi
}

make_benchmark() {
    echo
    echo "${ANSI_MAGENTA}┏━━━━━━━━━━━┓${ANSI_RESET}"
    echo "${ANSI_MAGENTA}┃ BENCHMARK ┃${ANSI_RESET}"
    echo "${ANSI_MAGENTA}┗━━━━━━━━━━━┛${ANSI_RESET}"
    echo

    ANYTHING_DONE=0

    for PROJECT_FILE in $(find "$SCRIPT_DIR/tests" -name "*.csproj"); do
        IS_BENCHMARK=$(cat "$PROJECT_FILE" | grep -E "BenchmarkDotNet" | wc -l)
        if [ $IS_BENCHMARK -eq 0 ]; then continue; fi

        ANYTHING_DONE=1
        echo "${ANSI_MAGENTA}$(basename $PROJECT_FILE)${ANSI_RESET}"

        cd "$( dirname "$PROJECT_FILE" )"
        dotnet run                       \
            --configuration "Release"    \
            -p:EnableNETAnalyzers=false  \
            --project "$PROJECT_FILE"   || exit 113
        cd "$SCRIPT_DIR"
        echo
    done

    if [ "$ANYTHING_DONE" -eq 0 ]; then
        echo "${ANSI_RED}No benchmark project found${ANSI_RESET}" >&2
        exit 113
    fi
}

make_examples() {
    echo
    echo "${ANSI_MAGENTA}┏━━━━━━━━━━┓${ANSI_RESET}"
    echo "${ANSI_MAGENTA}┃ EXAMPLES ┃${ANSI_RESET}"
    echo "${ANSI_MAGENTA}┗━━━━━━━━━━┛${ANSI_RESET}"
    echo

    ANYTHING_DONE=0

    for PROJECT_FILE in $(find "$SCRIPT_DIR/examples/src" -name "*.csproj"); do
        ANYTHING_DONE=1

        echo "${ANSI_MAGENTA}$(basename $PROJECT_FILE) ($(basename $(dirname $PROJECT_FILE)))${ANSI_RESET}"

        mkdir -p "$SCRIPT_DIR/examples/bin"
        dotnet build "$PROJECT_FILE" --configuration Release --output "$SCRIPT_DIR/examples/bin"
        echo
    done

    if [ "$ANYTHING_DONE" -eq 0 ]; then
        echo "${ANSI_RED}No example project found${ANSI_RESET}" >&2
        exit 113
    fi
}

make_tools() {
    echo
    echo "${ANSI_MAGENTA}┏━━━━━━━┓${ANSI_RESET}"
    echo "${ANSI_MAGENTA}┃ TOOLS ┃${ANSI_RESET}"
    echo "${ANSI_MAGENTA}┗━━━━━━━┛${ANSI_RESET}"
    echo

    ANYTHING_DONE=0

    for PROJECT_FILE in $(find "$SCRIPT_DIR/tools/src" -name "*.csproj"); do
        ANYTHING_DONE=1

        echo "${ANSI_MAGENTA}$(basename $PROJECT_FILE) ($(basename $(dirname $PROJECT_FILE)))${ANSI_RESET}"

        mkdir -p "$SCRIPT_DIR/tools/bin"
        dotnet build "$PROJECT_FILE" --configuration Release --output "$SCRIPT_DIR/tools/bin"
        echo
    done

    if [ "$ANYTHING_DONE" -eq 0 ]; then
        echo "${ANSI_RED}No example project found${ANSI_RESET}" >&2
        exit 113
    fi
}

make_debug() {
    echo
    echo "${ANSI_MAGENTA}┏━━━━━━━┓${ANSI_RESET}"
    echo "${ANSI_MAGENTA}┃ DEBUG ┃${ANSI_RESET}"
    echo "${ANSI_MAGENTA}┗━━━━━━━┛${ANSI_RESET}"
    echo

    echo "${ANSI_MAGENTA}$(basename $PROJECT_ENTRYPOINT)${ANSI_RESET}"

    mkdir -p "$SCRIPT_DIR/bin"
    dotnet build                           \
        --configuration Debug              \
        --output "$SCRIPT_DIR/bin"         \
        -p:EnableNETAnalyzers=false        \
        "$SCRIPT_DIR/$PROJECT_ENTRYPOINT" || exit 113
}

make_release() {
    echo
    echo "${ANSI_MAGENTA}┏━━━━━━━━━┓${ANSI_RESET}"
    echo "${ANSI_MAGENTA}┃ RELEASE ┃${ANSI_RESET}"
    echo "${ANSI_MAGENTA}┗━━━━━━━━━┛${ANSI_RESET}"
    echo

    mkdir -p "$SCRIPT_DIR/bin"
    PROJECT_RUNTIME_COUNT=$(echo $PROJECT_RUNTIMES | wc -w)
    for RUNTIME in $PROJECT_RUNTIMES; do
        echo "${ANSI_MAGENTA}$(basename $PROJECT_ENTRYPOINT) ($RUNTIME)${ANSI_RESET}"

        PUBLISH_EXTRA_ARGS=
        if [ "$PROJECT_SINGLEFILE" = "true" ]; then
            PUBLISH_EXTRA_ARGS="$PUBLISH_EXTRA_ARGS --self-contained true -p:PublishSingleFile=true"
        elif [ "$PROJECT_SINGLEFILE" = "false" ]; then
            PUBLISH_EXTRA_ARGS="$PUBLISH_EXTRA_ARGS --self-contained false -p:PublishSingleFile=false"
        fi

        if [ "$PROJECT_OUTPUTTYPE" = "exe" ] || [ "$PROJECT_OUTPUTTYPE" = "winexe" ]; then
            PUBLISH_EXTRA_ARGS="$PUBLISH_EXTRA_ARGS -p:PublishReadyToRun=true"
        elif [ "$PROJECT_OUTPUTTYPE" = "library" ]; then  # libraries cannot be published as a single file
            PUBLISH_EXTRA_ARGS="$PUBLISH_EXTRA_ARGS -p:GenerateDocumentationFile=true"
        else
            echo "${ANSI_RED}Cannot compile project type'$PROJECT_OUTPUTTYPE'${ANSI_RESET}" >&2
            exit 113
        fi

        if [ "$RUNTIME" = "current" ]; then
            PUBLISH_EXTRA_ARGS="$PUBLISH_EXTRA_ARGS --use-current-runtime"
            if [ "$PROJECT_RUNTIME_COUNT" -eq 1 ]; then
                PUBLISH_OUTPUT_DIR="$SCRIPT_DIR/bin"
            else
                PUBLISH_OUTPUT_DIR="$SCRIPT_DIR/bin/current"
            fi
        else
            PUBLISH_EXTRA_ARGS="$PUBLISH_EXTRA_ARGS --runtime $RUNTIME"
            PUBLISH_OUTPUT_DIR="$SCRIPT_DIR/bin/$RUNTIME"
        fi

        dotnet publish "$SCRIPT_DIR/$PROJECT_ENTRYPOINT"                           \
            --configuration Release                                                \
            -p:AssemblyVersion=$ASSEMBLY_VERSION -p:FileVersion=$ASSEMBLY_VERSION  \
            -p:Version=$ASSEMBLY_VERSION_TEXT                                      \
            -p:EnableNETAnalyzers=false                                            \
            $PUBLISH_EXTRA_ARGS --output "$PUBLISH_OUTPUT_DIR"                     \
        && echo "${ANSI_CYAN}$SCRIPT_DIR/bin${ANSI_RESET}"                        || exit 113

        if [ -e "$SCRIPT_DIR/examples/content" ]; then
            mkdir -p "$PUBLISH_OUTPUT_DIR/examples"
            (cd "$SCRIPT_DIR/examples/content" && find . -type d -exec mkdir -p "$PUBLISH_OUTPUT_DIR/examples/{}" \;)
            (cd "$SCRIPT_DIR/examples/content" && find . -type f -exec cp --parents {} "$PUBLISH_OUTPUT_DIR/examples/" \;)
        fi

        echo
    done
}

make_package() {
    echo
    echo "${ANSI_MAGENTA}┏━━━━━━━━━┓${ANSI_RESET}"
    echo "${ANSI_MAGENTA}┃ PACKAGE ┃${ANSI_RESET}"
    echo "${ANSI_MAGENTA}┗━━━━━━━━━┛${ANSI_RESET}"
    echo

    ANYTHING_DONE=0

    for RUNTIME in $PROJECT_RUNTIMES; do
        if [ "$RUNTIME" = "current" ]; then continue; fi

        ANYTHING_DONE=1
        echo "${ANSI_MAGENTA}archive ($RUNTIME)${ANSI_RESET}"

        mkdir -p "$SCRIPT_DIR/build/archive-$RUNTIME"
        find "$SCRIPT_DIR/build/archive-$RUNTIME" -mindepth 1 -delete

        rsync -a "$SCRIPT_DIR/bin/$RUNTIME/" "$SCRIPT_DIR/build/archive-$RUNTIME/" || exit 113

        mkdir -p "dist"

        case $RUNTIME in
            win-*)  ARCHIVE_NAME_CURR="$PROJECT_NAME_LOWER-$ASSEMBLY_VERSION_TEXT-$RUNTIME.zip" ;;
            *)      ARCHIVE_NAME_CURR="$PROJECT_NAME_LOWER-$ASSEMBLY_VERSION_TEXT-$RUNTIME.tgz" ;;
        esac
        rm "dist/$ARCHIVE_NAME_CURR" 2>/dev/null

        case $RUNTIME in
            win-*)  (cd "$SCRIPT_DIR/build/archive-$RUNTIME" && zip -r "$SCRIPT_DIR/dist/$ARCHIVE_NAME_CURR" .) || exit 113 ;;
            *)      tar czvf "dist/$ARCHIVE_NAME_CURR" -C "$SCRIPT_DIR/build/archive-$RUNTIME" . || exit 113 ;;
        esac

        echo "${ANSI_CYAN}dist/$APPIMAGE_NAME_CURR${ANSI_RESET}"
        echo
    done

    if [ "$PACKAGE_LINUX_APPIMAGE" != "" ]; then
        for RUNTIME in $PROJECT_RUNTIMES; do
            case $RUNTIME in
                linux-x64)   APPIMAGE_ARCHITECTURE=x86_64 ;;
                linux-arm64) APPIMAGE_ARCHITECTURE=aarch64 ;;
                *)           continue ;;
            esac

            ANYTHING_DONE=1
            echo "${ANSI_MAGENTA}appimage ($RUNTIME: $APPIMAGE_ARCHITECTURE)${ANSI_RESET}"

            APPIMAGE_NAME_CURR="$PROJECT_NAME_LOWER-$ASSEMBLY_VERSION_TEXT-$APPIMAGE_ARCHITECTURE.AppImage"

            mkdir -p "$SCRIPT_DIR/build/AppImage-$RUNTIME"
            find "$SCRIPT_DIR/build/AppImage-$RUNTIME" -mindepth 1 -delete

            cp "$SCRIPT_DIR/packaging/linux-appimage/AppRun" "$SCRIPT_DIR/build/AppImage-$RUNTIME/" || exit 113

            mkdir -p "$SCRIPT_DIR/build/AppImage-$RUNTIME/opt/$PROJECT_NAME"
            rsync -a "$SCRIPT_DIR/bin/linux-x64/" "$SCRIPT_DIR/build/AppImage-$RUNTIME/opt/$PROJECT_NAME/" || exit 113

            rsync -a "$SCRIPT_DIR/packaging/linux-deb/usr/" "$SCRIPT_DIR/build/AppImage-$RUNTIME/usr/" || exit 113

            cp "$SCRIPT_DIR/packaging/linux-deb/usr/share/applications"/*.desktop "$SCRIPT_DIR/build/AppImage-$RUNTIME/" || exit 113
            cp "$SCRIPT_DIR/packaging/linux-deb/usr/share/icons/hicolor/128x128/apps"/*.png "$SCRIPT_DIR/build/AppImage-$RUNTIME/" || exit 113
            cp "$SCRIPT_DIR/packaging/linux-deb/usr/share/icons/hicolor/128x128/apps"/*.png "$SCRIPT_DIR/build/AppImage-$RUNTIME/.DirIcon" || exit 113

            if [ -e "$SCRIPT_DIR/packaging/linux-deb/etc/" ]; then
                rsync -a "$SCRIPT_DIR/packaging/linux-deb/etc/" "$SCRIPT_DIR/build/AppImage-$RUNTIME/etc/" || exit 113
            fi

            mkdir -p "dist"
            rm "dist/$APPIMAGE_NAME_CURR" 2>/dev/null
            ARCH=$APPIMAGE_ARCHITECTURE appimagetool-x86_64.AppImage "$SCRIPT_DIR/build/AppImage-$RUNTIME/" "dist/$APPIMAGE_NAME_CURR" || exit 113

            case $RUNTIME in
                linux-x64)   APPIMAGE_NAME_AMD64=$APPIMAGE_NAME_CURR ;;
                linux-arm64) APPIMAGE_NAME_ARM64=$APPIMAGE_NAME_CURR ;;
                *)           continue ;;
            esac

            echo "${ANSI_CYAN}dist/$APPIMAGE_NAME_CURR${ANSI_RESET}"
            echo
        done
    fi

    if [ "$PACKAGE_LINUX_DEB" != "" ]; then
        for RUNTIME in $PROJECT_RUNTIMES; do
            case $RUNTIME in
                linux-x64)   DEB_ARCHITECTURE=amd64 ;;
                linux-arm64) DEB_ARCHITECTURE=arm64 ;;
                *)           continue ;;
            esac

            ANYTHING_DONE=1
            echo "${ANSI_MAGENTA}deb ($RUNTIME: $DEB_ARCHITECTURE)${ANSI_RESET}"

            if [ "$GIT_VERSION" != "" ]; then
                DEB_VERSION=$GIT_VERSION
                DEB_PACKAGE_NAME="${PROJECT_NAME_LOWER}_${ASSEMBLY_VERSION_TEXT}_${DEB_ARCHITECTURE}"
            else
                DEB_VERSION=0.0.0
                DEB_PACKAGE_NAME="${PROJECT_NAME_LOWER}_${ASSEMBLY_VERSION_TEXT}_${DEB_ARCHITECTURE}"
            fi

            mkdir -p "$SCRIPT_DIR/build/$DEB_PACKAGE_NAME"
            find "$SCRIPT_DIR/build/$DEB_PACKAGE_NAME/" -mindepth 1 -delete

            rsync -a "$SCRIPT_DIR/packaging/linux-deb/DEBIAN/" "$SCRIPT_DIR/build/$DEB_PACKAGE_NAME/DEBIAN/" || exit 113
            sed -i "s/<DEB_VERSION>/$DEB_VERSION/" "$SCRIPT_DIR/build/$DEB_PACKAGE_NAME/DEBIAN/control" || exit 113
            sed -i "s/<DEB_ARCHITECTURE>/amd64/" "$SCRIPT_DIR/build/$DEB_PACKAGE_NAME/DEBIAN/control" || exit 113

            rsync -a "$SCRIPT_DIR/packaging/linux-deb/usr/" "$SCRIPT_DIR/build/$DEB_PACKAGE_NAME/usr/" || exit 113

            mkdir -p  "$SCRIPT_DIR/build/$DEB_PACKAGE_NAME/opt/$PROJECT_NAME/"
            rsync -a "$SCRIPT_DIR/bin/linux-x64/" "$SCRIPT_DIR/build/$DEB_PACKAGE_NAME/opt/$PROJECT_NAME/" || exit 113

            if [ -e "$SCRIPT_DIR/packaging/linux-deb/copyright" ]; then
                mkdir -p "$SCRIPT_DIR/build/$DEB_PACKAGE_NAME/usr/share/doc/$PROJECT_NAME/"
                cp "$SCRIPT_DIR/packaging/linux-deb/copyright" "$SCRIPT_DIR/build/$DEB_PACKAGE_NAME/usr/share/doc/$PROJECT_NAME/copyright" || exit 113
            fi

            find "$SCRIPT_DIR/build/$DEB_PACKAGE_NAME/" -type d -exec chmod 755 {} + || exit 113
            find "$SCRIPT_DIR/build/$DEB_PACKAGE_NAME/" -type f -exec chmod 644 {} + || exit 113
            find "$SCRIPT_DIR/build/$DEB_PACKAGE_NAME/opt/" -type f -name "$PROJECT_NAME" -exec chmod 755 {} + || exit 113
            chmod 755 "$SCRIPT_DIR/build/$DEB_PACKAGE_NAME/DEBIAN"/config || exit 113
            chmod 755 "$SCRIPT_DIR/build/$DEB_PACKAGE_NAME/DEBIAN"/p*inst || exit 113
            chmod 755 "$SCRIPT_DIR/build/$DEB_PACKAGE_NAME/DEBIAN"/p*rm || exit 113

            fakeroot dpkg-deb -Z gzip --build "$SCRIPT_DIR/build/$DEB_PACKAGE_NAME/" > /dev/null || exit 113
            mv "$SCRIPT_DIR/build/$DEB_PACKAGE_NAME.deb" "dist/$DEB_PACKAGE_NAME.deb" || exit 113
            lintian --suppress-tags dir-or-file-in-opt,embedded-library "dist/$DEB_PACKAGE_NAME.deb"

            case $RUNTIME in
                linux-x64)   DEB_PACKAGE_AMD64=$DEB_PACKAGE_NAME.deb ;;
                linux-arm64) DEB_PACKAGE_ARM64=$DEB_PACKAGE_NAME.deb ;;
                *)           continue ;;
            esac

            echo "${ANSI_CYAN}dist/$DEB_PACKAGE_NAME.deb${ANSI_RESET}"
            echo
        done
    fi

    if [ "$PACKAGE_LINUX_DOCKER" != "" ]; then
        ANYTHING_DONE=1
        echo "${ANSI_MAGENTA}docker${ANSI_RESET}"

        if [ "$GIT_VERSION" != "" ]; then
            docker build                               \
                -t $PACKAGE_LINUX_DOCKER:$GIT_VERSION  \
                -t $PACKAGE_LINUX_DOCKER:latest        \
                -t $PACKAGE_LINUX_DOCKER:unstable      \
                -f "$DOCKER_FILE" .                   || exit 113
            echo "${ANSI_CYAN}$PACKAGE_LINUX_DOCKER:$GIT_VERSION $PACKAGE_LINUX_DOCKER:latest $PACKAGE_LINUX_DOCKER:unstable${ANSI_RESET}"

            mkdir -p "$SCRIPT_DIR/dist"
            docker save                                                 \
                $PACKAGE_LINUX_DOCKER:$GIT_VERSION                      \
                | gzip > ./dist/$PACKAGE_LINUX_DOCKER.$GIT_VERSION.tgz || exit 113
            echo "${ANSI_CYAN}dist/$PACKAGE_LINUX_DOCKER-$GIT_VERSION.tgz${ANSI_RESET}"
        else
            docker build                           \
                -t $PACKAGE_LINUX_DOCKER:unstable  \
                -f "$DOCKER_FILE" .               || exit 113
            echo "${ANSI_CYAN}$PACKAGE_LINUX_DOCKER:unstable${ANSI_RESET}"
        fi
        echo
    fi

    if [ "$PACKAGE_NUGET" != "" ]; then
        ANYTHING_DONE=1
        echo "${ANSI_MAGENTA}nuget${ANSI_RESET}"

        mkdir -p "$SCRIPT_DIR/build/nuget"
        dotnet pack                             \
            "$PACKAGE_NUGET_ENTRYPOINT"         \
            --configuration "Release"           \
            --force                             \
            --include-source                    \
            --output "./build/nuget/"           \
            --p:PackageId=$PACKAGE_NUGET_ID     \
            --p:Version=$PACKAGE_NUGET_VERSION  \
            --p:EnableNETAnalyzers=false        \
            --verbosity "minimal"              || return 1

        mkdir -p "$SCRIPT_DIR/dist/"

        echo
        cp "$SCRIPT_DIR/build/nuget/$PACKAGE_NUGET_ID.$PACKAGE_NUGET_VERSION.nupkg"  "$SCRIPT_DIR/dist/" || return 1
        echo "${ANSI_CYAN}dist/$PACKAGE_NUGET_ID.$PACKAGE_NUGET_VERSION.nupkg${ANSI_RESET}"
        cp "$SCRIPT_DIR/build/nuget/$PACKAGE_NUGET_ID.$PACKAGE_NUGET_VERSION.snupkg" "$SCRIPT_DIR/dist/" || return 1
        echo "${ANSI_CYAN}dist/$PACKAGE_NUGET_ID.$PACKAGE_NUGET_VERSION.snupkg${ANSI_RESET}"
        echo
    fi

    if [ "$ANYTHING_DONE" -eq 0 ]; then
        echo "${ANSI_RED}Nothing to package${ANSI_RESET}" >&2
        exit 113
    fi
}

make_publish() {
    echo
    echo "${ANSI_MAGENTA}┏━━━━━━━━━┓${ANSI_RESET}"
    echo "${ANSI_MAGENTA}┃ PUBLISH ┃${ANSI_RESET}"
    echo "${ANSI_MAGENTA}┗━━━━━━━━━┛${ANSI_RESET}"
    echo

    ANYTHING_DONE=0

    if [ "$PUBLISH_LINUX_ARCHIVE" != "" ]; then
        if [ "$GIT_VERSION" != "" ]; then
            for RUNTIME in $PROJECT_RUNTIMES; do
                case $RUNTIME in
                    current) continue ;;
                    win-*)   ARCHIVE_NAME_CURR="$PROJECT_NAME_LOWER-$ASSEMBLY_VERSION_TEXT-$RUNTIME.zip" ;;
                    *)       ARCHIVE_NAME_CURR="$PROJECT_NAME_LOWER-$ASSEMBLY_VERSION_TEXT-$RUNTIME.tgz" ;;
                esac

                ANYTHING_DONE=1
                echo "${ANSI_MAGENTA}archive $ARCHIVE_NAME_CURR ($RUNTIME)${ANSI_RESET}"

                rsync --no-g --no-o --progress --chmod=D755,F644  "dist/$ARCHIVE_NAME_CURR" $PUBLISH_LINUX_ARCHIVE || exit 113
                echo "${ANSI_CYAN}$PUBLISH_LINUX_ARCHIVE/$ARCHIVE_NAME_CURR${ANSI_RESET}"
                echo
            done
        else
            echo "${ANSI_YELLOW}Not publishing unversioned archive${ANSI_RESET}" >&2
        fi
    fi

    if [ "$PUBLISH_LINUX_APPIMAGE" != "" ]; then
        for RUNTIME in $PROJECT_RUNTIMES; do
            case $RUNTIME in
                linux-x64)   APPIMAGE_NAME_CURR=$APPIMAGE_NAME_AMD64 ;;
                linux-arm64) APPIMAGE_NAME_CURR=$APPIMAGE_NAME_ARM64 ;;
                *)           continue ;;
            esac

            ANYTHING_DONE=1
            echo "${ANSI_MAGENTA}appimage ($RUNTIME)${ANSI_RESET}"

            rsync --no-g --no-o --progress --chmod=D755,F644  "dist/$APPIMAGE_NAME_CURR" $PUBLISH_LINUX_APPIMAGE || exit 113
            echo "${ANSI_CYAN}$PUBLISH_LINUX_APPIMAGE${ANSI_RESET}"
            echo
        done
    fi

    if [ "$PUBLISH_LINUX_DEB" != "" ]; then
        for RUNTIME in $PROJECT_RUNTIMES; do
            case $RUNTIME in
                linux-x64)   DEB_ARCHITECTURE=amd64 ; DEB_PACKAGE_CURR=$DEB_PACKAGE_AMD64 ;;
                linux-arm64) DEB_ARCHITECTURE=arm64 ; DEB_PACKAGE_CURR=$DEB_PACKAGE_ARM64 ;;
                *)           continue ;;
            esac

            ANYTHING_DONE=1
            echo "${ANSI_MAGENTA}deb ($RUNTIME: $DEB_ARCHITECTURE)${ANSI_RESET}"

            PUBLISH_LINUX_DEB_CURR="$( echo "$PUBLISH_LINUX_DEB" | sed "s/<DEB_ARCHITECTURE>/$DEB_ARCHITECTURE/g" )"

            rsync --no-g --no-o --progress "dist/$DEB_PACKAGE_CURR" $PUBLISH_LINUX_DEB_CURR || exit 113
            echo "${ANSI_CYAN}$PUBLISH_LINUX_DEB_CURR${ANSI_RESET}"
            echo
        done
    fi

    if [ "$PUBLISH_LINUX_DOCKER" != "" ]; then
        ANYTHING_DONE=1
        echo "${ANSI_MAGENTA}docker${ANSI_RESET}"

        if [ -d "$SCRIPT_DIR/.meta.docker" ]; then
            DOCKER_CONFIG_ARGS="--config .meta.docker"
        fi

        if [ "$GIT_VERSION" != "" ]; then
            docker tag                                            \
                $PACKAGE_LINUX_DOCKER:$GIT_VERSION                \
                $DOCKER_IMAGE_ID/$DOCKER_IMAGE_NAME:$GIT_VERSION || exit 113
            docker $DOCKER_CONFIG_ARGS push                       \
                $DOCKER_IMAGE_ID/$DOCKER_IMAGE_NAME:$GIT_VERSION || exit 113
            echo "${ANSI_CYAN}$DOCKER_IMAGE_ID/$DOCKER_IMAGE_NAME:$GIT_VERSION${ANSI_RESET}"
            echo

            docker tag                                      \
                $PACKAGE_LINUX_DOCKER:latest                \
                $DOCKER_IMAGE_ID/$DOCKER_IMAGE_NAME:latest || exit 113
            docker $DOCKER_CONFIG_ARGS push                 \
                $DOCKER_IMAGE_ID/$DOCKER_IMAGE_NAME:latest || exit 113
            echo "${ANSI_CYAN}$DOCKER_IMAGE_ID/$DOCKER_IMAGE_NAME:latest${ANSI_RESET}"
            echo
        fi

        docker tag                                        \
            $PACKAGE_LINUX_DOCKER:unstable                \
            $DOCKER_IMAGE_ID/$DOCKER_IMAGE_NAME:unstable || exit 113
        docker $DOCKER_CONFIG_ARGS push                   \
            $DOCKER_IMAGE_ID/$DOCKER_IMAGE_NAME:unstable || exit 113
            echo "${ANSI_CYAN}$DOCKER_IMAGE_ID/$DOCKER_IMAGE_NAME:unstable${ANSI_RESET}"
        echo
    fi

    if [ "$PUBLISH_NUGET_KEY" != "" ]; then
        ANYTHING_DONE=1

        if [ "$PACKAGE_NUGET_VERSION" = "0.0.0" ]; then
            echo "${ANSI_RED}Not pushing version 0.0.0!${ANSI_RESET}" >&2
            return 1;
        fi
        dotnet nuget push   \
            --source "https://api.nuget.org/v3/index.json"           \
            --api-key "$PUBLISH_NUGET_KEY"                           \
            --symbol-api-key "$PUBLISH_NUGET_KEY"                    \
            "./dist/$PACKAGE_NUGET_ID.$PACKAGE_NUGET_VERSION.nupkg" || return 1
        echo "${ANSI_GREEN}Sent ${ANSI_CYAN}dist/$PACKAGE_NUGET_ID-$PACKAGE_NUGET_VERSION.nupkg${ANSI_RESET}"
        echo
    fi

    if [ "$ANYTHING_DONE" -eq 0 ]; then
        echo "${ANSI_RED}Nothing to publish${ANSI_RESET}" >&2
        exit 113
    fi
}


if [ "$1" = "" ]; then ACTIONS="all"; else ACTIONS="$@"; fi

TOKENS=" "
NEGTOKENS=
PREREQ_COMPILE=0
PREREQ_PACKAGE=0
for ACTION in $ACTIONS; do
    case $ACTION in
        all)        TOKENS="$TOKENS clean release"                      ; PREREQ_COMPILE=1                    ;;
        clean)      TOKENS="$TOKENS clean"                                                                    ;;
        run)        TOKENS="$TOKENS run"                                ; PREREQ_COMPILE=1                    ;;
        test)       TOKENS="$TOKENS clean test"                         ; PREREQ_COMPILE=1                    ;;
        benchmark)  TOKENS="$TOKENS clean benchmark"                    ; PREREQ_COMPILE=1                    ;;
        examples)   TOKENS="$TOKENS clean examples"                     ; PREREQ_COMPILE=1                    ;;
        tools)      TOKENS="$TOKENS clean tools"                        ; PREREQ_COMPILE=1                    ;;
        debug)      TOKENS="$TOKENS clean debug"                        ; PREREQ_COMPILE=1                    ;;
        release)    TOKENS="$TOKENS clean test release"                 ; PREREQ_COMPILE=1                    ;;
        package)    TOKENS="$TOKENS clean test release package"         ; PREREQ_COMPILE=1 ; PREREQ_PACKAGE=1 ;;
        publish)    TOKENS="$TOKENS clean test release package publish" ; PREREQ_COMPILE=1 ; PREREQ_PACKAGE=1 ;;
        ~clean)     NEGTOKENS="$NEGTOKENS clean"     ;;
        ~run)       NEGTOKENS="$NEGTOKENS run"       ;;
        ~test)      NEGTOKENS="$NEGTOKENS test"      ;;
        ~benchmark) NEGTOKENS="$NEGTOKENS benchmark" ;;
        ~examples)  NEGTOKENS="$NEGTOKENS examples"  ;;
        ~tools)     NEGTOKENS="$NEGTOKENS tools"  ;;
        ~debug)     NEGTOKENS="$NEGTOKENS debug"     ;;
        ~release)   NEGTOKENS="$NEGTOKENS release"   ;;
        ~package)   NEGTOKENS="$NEGTOKENS package"   ;;
        ~publish)   NEGTOKENS="$NEGTOKENS publish"   ;;
        *)         echo "Unknown action $ACTION" >&2 ; exit 113 ;;
    esac
done

if [ $PREREQ_COMPILE -ne 0 ]; then prereq_compile; fi
if [ $PREREQ_PACKAGE -ne 0 ]; then prereq_package; fi

NEGTOKENS=$( echo $NEGTOKENS | xargs | tr ' ' '\n' | awk '!seen[$0]++' | xargs )  # remove duplicates
TOKENS=$( echo $TOKENS | xargs | tr ' ' '\n' | awk '!seen[$0]++' | xargs )  # remove duplicates

for NEGTOKEN in $NEGTOKENS; do  # remove tokens we specifically asked not to have
    TOKENS=$( echo $TOKENS | tr ' ' '\n' | grep -v $NEGTOKEN | xargs )
done

if [ "$TOKENS" != "" ]; then
    echo "${ANSI_PURPLE}Make targets ........: ${ANSI_MAGENTA}$TOKENS${ANSI_RESET}"
else
    echo "${ANSI_PURPLE}Make targets ........: ${ANSI_RED}not found${ANSI_RESET}"
    exit 113
fi
echo

for TOKEN in $TOKENS; do
    case $TOKEN in
        clean)     make_clean     || exit 113 ;;
        run)       make_run       || exit 113 ;;
        test)      make_test      || exit 113 ;;
        benchmark) make_benchmark || exit 113 ;;
        examples)  make_examples  || exit 113 ;;
        tools)     make_tools     || exit 113 ;;
        debug)     make_debug     || exit 113 ;;
        release)   make_release   || exit 113 ;;
        package)   make_package   || exit 113 ;;
        publish)   make_publish   || exit 113 ;;
        *)         echo "Unknown token $TOKEN" >&2 ; exit 113 ;;
    esac
done

exit 0
