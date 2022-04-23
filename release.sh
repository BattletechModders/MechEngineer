#!/bin/bash

export PATH="/c/Program Files/7-Zip/:$PATH"

SEVENZIP="7z"

set -ex

cd ..

MEZIP="MechEngineer/dist/MechEngineer.zip"
INCLUDES="-ir!MechEngineer -ir!CustomComponents"
EXCLUDES="-xr!log.txt -xr!*.log -xr!*.DotSettings -xr!*.sln -xr!*.suo -xr!*.pdb -xr!*.user -xr!bin -xr!obj -xr!packages -xr!.vs -xr!.git* -xr!_ignored -xr!*.zip -xr!*.sh -xr!.idea -xr!MechEngineer/source -xr!MechEngineer/engine_generator"

(
cd MechEngineer
dotnet build -c Release "$@"
)

rm -f "$MEZIP"
"$SEVENZIP" a -tzip -mx9 "$MEZIP" $EXCLUDES $INCLUDES
