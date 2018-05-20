# MechEngineMod
BattleTech mod (using ModTek and DynModLib) that adds engines as necessary components for mechs.

## Requirements
** Warning: Uses the experimental BTML mod loader and ModTek **

either
* install BattleTechModTools using [BattleTechModInstaller](https://github.com/CptMoore/BattleTechModTools/releases)

or
* install [BattleTechModLoader](https://github.com/Mpstark/BattleTechModLoader/releases) using [instructions here](https://github.com/Mpstark/BattleTechModLoader)
* install [ModTek](https://github.com/Mpstark/ModTek/releases) using [instructions here](https://github.com/Mpstark/ModTek)
* install [DynModLib](https://github.com/CptMoore/DynModLib/releases) using [instructions here](https://github.com/CptMoore/DynModLib)

## Features

* added std engine type
* added xl engine type and side torso xl engine parts
* calculate walk and run speeds based on engine rating and mech tonnage
* all mechs have 10% their max tonnage as initial tonnage
* mechlab enforces to use an engine on a mech, xl engine requires the side torso parts
* mechlab enforces only one engine part per chassis location
* mechlab enforces only one gyro for center torso, also gryo to be 1 crit high
* adjust movement speed summary stat based on engine rating
* hide incompatible engines for mech
* engine tonnage determins install tech cost
* crit engine parts reduce speed, destroyed engine parts destroy mech
* jump jets should now be limited to the integer distance of the mech's TT walk speed


## Download

Downloads can be found on [github](https://github.com/CptMoore/StartingMercs/releases).

## Install

After installing BTML, ModTek and DynModLib, put the mod into the \BATTLETECH\Mods\ folder and launch the game.

## Development

* Use git
* Use Visual Studio or DynModLib to compile the project
