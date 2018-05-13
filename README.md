# MechEngineMod
BattleTech mod (using ModTek and DynModLib) that allows to add random or ronin mech warriors.

## Requirements
** Warning: Uses the experimental BTML mod loader and ModTek **

either
* install BattleTechModTools using [BattleTechModInstaller](https://github.com/CptMoore/BattleTechModTools/releases)

or
* install [BattleTechModLoader](https://github.com/Mpstark/BattleTechModLoader/releases) using [instructions here](https://github.com/Mpstark/BattleTechModLoader)
* install [ModTek](https://github.com/Mpstark/ModTek/releases) using [instructions here](https://github.com/Mpstark/ModTek)
* install [DynModLib](https://github.com/CptMoore/DynModLib/releases) using [instructions here](https://github.com/CptMoore/DynModLib)

## Features

- add random mercs to starting roster.
- add ronin mercs to starting roster.

Setting | Type | Default | Description
--- | --- | --- | ---
addRandomMercsCount | int | 3 | amount of random mercs to add to roster
randomMercQuality | int | 1 | merc quality is based on difficulty, choose a value between 1 and 5.
roninChance | float | 0.08f | chance that a random ronin is part of the starting roster
addRoninMercs | string[] | [] | a list of ronin pilot to always add, all backers and ronins under StreamingAssets\data\pilot can be chosen. e.g. `["pilot_ronin_Kraken", "pilot_backer_Chang"]`

Note:

The lists `StartingMechWarriors` and `StartingMechWarriorPortraits` in `SimGameConstants.json` need to be empty in order for this to work. ModTek in-memory patching should do that for you automatically. The game uses the first mercs in the roster to add as pilots for the starting mission. In any case mercs added through this mod will appear in the roster after the mission.

## Download

Downloads can be found on [github](https://github.com/CptMoore/StartingMercs/releases).

## Install

After installing BTML, ModTek and DynModLib, put the mod into the \BATTLETECH\Mods\ folder and launch the game.

## Development

* Use git
* Use Visual Studio or DynModLib to compile the project
