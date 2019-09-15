#!/bin/bash

set -ex

cd ..

SEVENZIP="/c/Program Files/7-Zip/7z"

INCLUDES="-ir!MechEngineer -ir!CustomComponents"
INCLUDES_ALL="$INCLUDES -ir!MechEngineerCheats -ir!BTDebug -ir!DebugToolsEnabler -ir!BattletechPerformanceFix -ir!SkipTravelCutscenes -ir!SkipTutorialCleaner -ir!SkipIntro -ir!CBTHeat -ir!DynModLib -ir!pansar -ir!SpeedMod -ir!ModTek -ir!CommanderPortraitLoader"

EXCLUDES="-xr!log.txt -xr!*.log -xr!*.suo -xr!*.pdb -xr!*.user -xr!bin -xr!obj -xr!packages -xr!.vs -xr!.git* -xr!.modtek -xr!_ignored -xr!*.zip -xr!*.sh"
EXCLUDES_ALL="$EXCLUDES -xr!CommanderPortraitLoader/source -xr!CustomComponents/source -xr!MechEngineer/source -xr!MechEngineer/engine_generator"

"$SEVENZIP" a -tzip -mx9 MechEngineer/MechEngineer.zip $EXCLUDES_ALL $INCLUDES
"$SEVENZIP" a -tzip -mx9 MechEngineer/MechEngineerWorkspace.zip $EXCLUDES $INCLUDES_ALL
