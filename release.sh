#!/bin/bash

set -ex

cd ..

SEVENZIP="/c/Program Files/7-Zip/7z"

INCLUDES="-ir!MechEngineer -ir!CustomComponents"
# -ir!HardpointFixMod
INCLUDES_ALL="$INCLUDES -ir!AdjustedMechSalvage -ir!BattletechPerformanceFix -ir!SkipIntro -ir!CBTHeat -ir!DynModLib -ir!pansar -ir!SpeedMod ModTek.dll"
EXCLUDES="-xr!log.txt -xr!*.log -xr!*.suo -xr!*.pdb -xr!*.user -xr!bin -xr!obj -xr!packages -xr!.vs -xr!.git -xr!_ignored -xr!*.zip"

"$SEVENZIP" a -tzip -mx9 MechEngineer/MechEngineer.zip $EXCLUDES $INCLUDES
"$SEVENZIP" a -tzip -mx9 MechEngineer/MechEngineerWorkspace.zip $EXCLUDES $INCLUDES_ALL
