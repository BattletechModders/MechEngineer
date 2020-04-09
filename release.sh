#!/bin/bash

PATH="/c/Program Files/7-Zip/:/c/Program Files (x86)/Microsoft Visual Studio/2019/Community/MSBuild/Current/Bin/:$PATH"

SEVENZIP="7z.exe"
MSBUILD="MSBuild.exe"

set -ex

MEZIP="MechEngineer/MechEngineer.zip"
MEALLZIP="MechEngineer/MechEngineerWorkspace.zip"
rm -f "$MEZIP" "$MEALLZIP"

cd ..

(
cd CustomComponents/source
#git describe --exact-match
"$MSBUILD" /property:Configuration=Release /target:rebuild
)

(
cd MechEngineer/source
git describe --exact-match
"$MSBUILD" /property:Configuration=Release /target:rebuild
)

(
INCLUDES="-ir!MechEngineer -ir!CustomComponents"
INCLUDES_ALL="$INCLUDES -ir!MechEngineerCheats -ir!BTDebug -ir!DebugToolsEnabler -ir!BattletechPerformanceFix -ir!SkipTravelCutscenes -ir!SkipTutorialCleaner -ir!SkipIntro -ir!CBTHeat -ir!DynModLib -ir!pansar -ir!SpeedMod -ir!ModTek"

EXCLUDES="-xr!log.txt -xr!*.log -xr!*.suo -xr!*.pdb -xr!*.user -xr!bin -xr!obj -xr!packages -xr!.vs -xr!.git* -xr!.modtek -xr!_ignored -xr!*.zip -xr!*.sh -xr!.idea"
EXCLUDES_ALL="$EXCLUDES -xr!HBS -xr!CommanderPortraitLoader/source -xr!CustomComponents/source -xr!MechEngineer/source -xr!MechEngineer/engine_generator"

"$SEVENZIP" a -tzip -mx9 "$MEZIP" $EXCLUDES_ALL $INCLUDES
"$SEVENZIP" a -tzip -mx9 "$MEALLZIP" $EXCLUDES_ALL $INCLUDES_ALL
)
