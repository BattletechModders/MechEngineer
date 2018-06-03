# MechEngineMod
BattleTech mod that adds engines and other components to mechs based on TT rules.

## Requirements

either
* install BattleTechModTools using [BattleTechModInstaller](https://github.com/CptMoore/BattleTechModTools/releases)

or
* install [BattleTechModLoader](https://github.com/Mpstark/BattleTechModLoader/releases) using [instructions here](https://github.com/Mpstark/BattleTechModLoader)
* install [ModTek](https://github.com/Mpstark/ModTek/releases) using [instructions here](https://github.com/Mpstark/ModTek)
* install [DynModLib](https://github.com/CptMoore/DynModLib/releases) using [instructions here](https://github.com/CptMoore/DynModLib)

## Features

* added std engine type
* mechlab enforces to use an engine
* added xl engine type and side torso xl engine parts
* mechlab enforces side torso parts for xl engines
* crit engine parts reduce speed, destroyed engine parts destroy mech
* calculate walk and run speeds based on engine rating and mech tonnage
* adjust movement speed summary stat based on engine rating
* jump jets should now be limited to the integer distance of the mech's TT walk speed
* added endo-steel critical slots to reduce structure weight by half
* mechlab enforces the use of all 14 critical slots when choosing endo-steel
* added ferros-fibrous critical slots to reduce armor weight by 1/12
* mechlab enforces the use of all 14 critical slots when choosing ferros-fibrous
* add all free heat sinks an engine supports, select type of heat sinks for engine by installing either double or single heatsinks on mech
* mechlab enforces either single or double heat sink types but no mix
* adjust heat performance summary stat based on engine rating
* mechlab enforces only one gyro for center torso, also gryo to be 1 crit high
* hide incompatible engines for mech
* engine tooltip now gives info on speed and heatsink capability in mechlab
* auto-adds engines to all newly bought and salvaged mechs that are missing them otherwise
* internal structure of mechs weights 10% of maximum chassis weight
* initial tonnage is the weight of internal structure + the cockpit weight of 3 tons

TODO

* see [issues list](https://github.com/CptMoore/MechEngineMod/issues)

## Download

Downloads can be found on [github](https://github.com/CptMoore/MechEngineMod/releases).

## Install

After installing BTML, ModTek and DynModLib, put the mod into the \BATTLETECH\Mods\ folder and launch the game.

## Development

* Use git
* Use Visual Studio or DynModLib to compile the project
