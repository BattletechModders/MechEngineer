# MechEngineer
BattleTech mod that adds engines and other components to mechs based on TT rules.

## Download

Downloads of the latest release can be found on [github](https://github.com/BattletechModders/MechEngineer/releases).

## Requirements and Installation

install [RogueTech](https://www.nexusmods.com/battletech/mods/79) for an in-depth MechEngineer experience

or

install [BattleTech Advanced](https://www.nexusmods.com/battletech/mods/452) for another in-depth MechEngineer experience

or

* HBS BattleTech ModLoader is supported
  * (optional) install [ModTek](https://github.com/BattletechModders/ModTek/releases) using [instructions here](https://github.com/BattletechModders/ModTek) if you don't like the ModLoader from BattleTech.
* install [CustomComponents](https://github.com/BattletechModders/CustomComponents/releases) using [instructions here](https://github.com/BattletechModders/CustomComponents)
* install MechEngineer by copying the MechEngineer folder to the mods/ directory of ModTek or the ModLoader.

## Suggested Mods

* [Pansar](https://github.com/hokvel/pansar) - applies armor ratio enforcement according to CBT rules

## TODOs and Bug Reporting

* see [issues list](https://github.com/BattletechModders/MechEngineer/issues)

## Contributors

Maintainer: CptMoore

* adammartinez271828 - rounding logic
* Aliencreature - ideas, lore and rules, item variants, testing
* Colobos - ideas, lore and rules, item and mech balancing, testing
* CptMoore - ideas, rules, coding, testing, core items
* CrusherBob - ideas, lore and rules, engine rating to walk/sprint distance conversions
* Denadan - ideas, custom components lib, coding
* Gentleman Reaper - ideas, lore and rules, testing
* LadyAlekto - ideas, lore and rules, testing, items
* TotalMeltdown - ideas, lore and rules

## Features

The current features are always described right within the default settings in `Settings.defaults.json`.

### Engines

A major feature of MechEngineer is the introduction of engine items. An engine consists of several parts, each representing a CBT variable in regards to engines.

- **Fusion Core** (e.g. Core 100):
The fusion core rating defines the movement speed, number of jump jets and internal engine heat sinks of a mech.
By default the weight of these items assume standard engine shielding.

- **Engine (Shielding)** (e.g. Std. Engine):
Just called "Engine", it actually represents the shielding of the fusion core.
An XL engine is extra-light shielding for the engine.
In combination with CC and the Weights feature, shielding differs in size and influences the fusion core weight.

- **Mech Cooling System** (e.g. Cooling DHS):
When choosing a heat sink system for the mech, in CBT one has to decide between standard and double heat sinks.
Since the engine heat sink type is derived from the mechs heat sink system, this is still part of the Engine feature.

- **Engine Cooling System** (e.g. E-Cooling + 2):
In CBT additional heat sinks can be added to engines for core sizes 275 and higher,
this item represents the additional heat sinks added.

#### Components

Some of the components introduced are only aesthetic or placeholders for the converted upgrades.

RogueTech introduces more components.

Cockpit | test only
--- | ---
standard | -
small | yes
cockpit upgrades* | -

*\*vanilla upgrades are auto-converted to be the items themselves*

Gyro | test only
--- | ---
standard | -
gryo upgrades* | -

*\*vanilla upgrades are auto-converted to be the items themselves*

Engine Core | test only
--- | ---
rating 005-400 | -

engine type | test only
--- | ---
std | -
xl | -
compact | yes
light | yes
cxl | yes
xxl | yes
cxxl | yes

E-Cooling | test only
--- | ---
DHS | -
CDHS | yes

Heat Sink | test only
--- | ---
Clan Double Heat Sink | yes

Armor | test only
--- | ---
standard | -
ferros-fibrous | -
clan ferros-fibrous | yes
light ferros-fibrous | yes
heavy ferros-fibrous | yes
stealth | yes

Structure | test only
--- | ---
standard | -
endo-steel | -
clan endo-steel | yes
endo-composite | yes
