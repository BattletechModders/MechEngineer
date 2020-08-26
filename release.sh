#!/bin/bash

PATH="/c/Program Files/7-Zip/:$PATH"

SEVENZIP="7z"

set -ex

MEZIP="MechEngineer/dist/MechEngineer.zip"
MEALLZIP="MechEngineer/dist/MechEngineerWorkspace.zip"
rm -f "$MEZIP" "$MEALLZIP"

cd ..

(
cd CustomComponents/source
#git describe --exact-match
dotnet build --configuration Release --no-incremental -p:OutputPath=../ "$@"
)

(
cd MechEngineer/source
#git describe --exact-match
dotnet build --configuration Release --no-incremental -p:OutputPath=../ "$@"
)

(
INCLUDES="-ir!MechEngineer -ir!CustomComponents"
INCLUDES_ALL="$INCLUDES -ir!MechEngineerCheats -ir!BTDebug -ir!DebugToolsEnabler -ir!BattletechPerformanceFix -ir!SkipTravelCutscenes -ir!SkipTutorialCleaner -ir!SkipIntro -ir!CBTHeat -ir!DynModLib -ir!pansar -ir!SpeedMod -ir!ModTek"

EXCLUDES="-xr!log.txt -xr!*.log -xr!*.suo -xr!*.pdb -xr!*.user -xr!bin -xr!obj -xr!packages -xr!.vs -xr!.git* -xr!.modtek -xr!_ignored -xr!*.zip -xr!*.sh -xr!.idea -xr!publicized_assemblies"
EXCLUDES_ALL="$EXCLUDES -xr!HBS -xr!CCTestMod -xr!CommanderPortraitLoader/source -xr!CustomComponents/source -xr!MechEngineer/source -xr!MechEngineer/engine_generator"

"$SEVENZIP" a -tzip -mx9 "$MEZIP" $EXCLUDES_ALL $INCLUDES
"$SEVENZIP" a -tzip -mx9 "$MEALLZIP" $EXCLUDES_ALL $INCLUDES_ALL
)
