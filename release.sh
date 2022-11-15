#!/bin/bash

# 7z is supported on Windows and Linux within GitHub
export PATH="/c/Program Files/7-Zip/:$PATH"
SEVENZIP="7z"

set -uex

( # Deps
cd "$MODS_DIR"
curl -L -o CustomComponents.zip https://github.com/BattletechModders/CustomComponents/releases/download/latest/CustomComponents.zip
unzip CustomComponents.zip
)

( # Build
dotnet build -c Release "-p:BattleTechGameDir=$BATTLETECH_DIR"
)

( # Dist
cd "$MODS_DIR"

MEZIP="$DIST_DIR/MechEngineer.zip"
INCLUDES="-ir!MechEngineer -ir!CustomComponents"
EXCLUDES="-xr!log.txt -xr!*.log -xr!*.DotSettings -xr!*.sln -xr!*.suo -xr!*.pdb -xr!*.user -xr!bin -xr!obj -xr!packages -xr!.vs -xr!.git* -xr!_ignored -xr!*.zip -xr!*.sh -xr!.idea -xr!MechEngineer/source -xr!MechEngineer/engine_generator"

rm -f "$MEZIP"
"$SEVENZIP" a -tzip -mx9 "$MEZIP" $EXCLUDES $INCLUDES
)
