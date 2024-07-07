#!/bin/bash
SCRIPT_DIR="$( cd -- "$(dirname "$0")" >/dev/null 2>&1 ; pwd -P )"

pushd "$SCRIPT_DIR/.." > /dev/null

# PasswordSafe
#git subtree add --prefix lib/PasswordSafe https://github.com/medo64/Medo.PasswordSafe.git main --squash
git subtree pull --prefix lib/PasswordSafe https://github.com/medo64/Medo.PasswordSafe.git main --squash

popd > /dev/null