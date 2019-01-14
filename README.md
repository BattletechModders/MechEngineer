# MechEngineer
BattleTech mod that adds engines and other components to mechs based on TT rules.

## Download

Downloads of the latest release can be found on [github](https://github.com/BattletechModders/MechEngineer/releases).

## Requirements and Installation

either install [RogueTech](https://www.nexusmods.com/battletech/mods/79) for the full MechEngineer experience

or

* install [ModTek](https://github.com/BattletechModders/ModTek/releases) using [instructions here](https://github.com/BattletechModders/ModTek)
* install [CustomComponents](https://github.com/BattletechModders/CustomComponents/releases) using [instructions here](https://github.com/BattletechModders/CustomComponents)
* install MechEngineer by putting it into the \BATTLETECH\Mods\ folder

## Suggested Mods

Use these mods to maximize enjoyment
* [CBT Heat](https://github.com/McFistyBuns/CBTHeat) - replaces overheat damage to be crit rolls
* [CBT Movement](https://github.com/McFistyBuns/CBTMovement) - movement reduces accuracy
* [CBT Piloting](https://github.com/McFistyBuns/CBTPiloting) - mech can stumble by chance
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

* Engine ratings
  * defines the walk and sprint speeds of a mech
  * determines the amount of jump jets a mech can mount
* Engine types
  * defines the weight and space use of an engine
  * Standard, XL, Light, Compact, XXL, Clan XL and XXL variants
* Engine crits
  * each crit reduces heat sink dissipation
  * on third crit, destroy engine
  * each destroyed slot of an engine counts as a critical hit
* Engine heat sinks
  * global heat dissipation removed
  * engines come with internal heat sinks already installed
  * can add additional heat sinks to an engine through drag & drop
  * can convert an engine to use DHS heat sinks instead of SHS through drag & drop of an DHS conversion kit
* Armor and Structure components
  * these provide weight savings and in turn require critical slots
  * Endo Steel, Endo Composite, Clan Endo Steel
  * Ferros Fibrous, Heavy FF, Light FF, Clan FF
* MechLab enhancements
  * fixes to have a better approximation of slot count in a mech (12 torso slots, 2 leg slots, 2 head due to cockpit etc..)
  * enforces that gyro, cockpit and engine parts are mounted
  * enforce engine side torso requirements
  * does not allow to mix heat sink types (can be disabled)
  * updated summary and enhanced tooltip info for movement and heat management 
  * hide engine ratings that would make the mech slower or faster than allowed
* Auto-fixes existing mechs and weapons on load
  * reduces initial tonnage to 10% structure
  * auto adds cockpit and gyro
  * auto adds engine components
  * auto fix chassis to have inventory sizes that match the CBT standard implemented in this mod (* actuators are still missing)
  * fun fact, the atlas is perfectly auto-fixed
  * also auto-fixes existing save games
  * weapons resized to CBT spec
  * AC20 is not full size until crit splitting is implemented
* Prepared item packs
  * enabled by modifying the mod.json and removing the "disabled_" prefixes
  * standard package, that provides lore* and time appropiate items to the game (*lore as the game sees fit, so LosTech is OK)
  * exotic package, adds in stuff like clan tech
  * test package, to play around in skirmish mechlab with everythig
* Settings and Modding
  * players can disable some of the restritions
  * modders can add more components using CustomComponents
  * add engines using the generate.pl script in engine_generator
  * there are additional settings like factional accounting or partical weight savings for structure and armor components
  * see [Settings Source Code](https://github.com/CptMoore/MechEngineer/blob/master/source/MechEngineerSettings.cs) for all available settings

### Components

cockpit | exotic
--- | ---
standard | -
small | yes
cockpit upgrades* | -

*\*upgrades are auto-converted to act as the actual item*

gyro | exotic
--- | ---
standard | -
gryo upgrades* | -

*\*upgrades are auto-converted to act as the actual item*

engine core | exotic
--- | ---
rating 60 | -
rating 100-400 | -

engine type | exotic
--- | ---
std | -
xl | -
compact | yes
light | yes
cxl | yes
xxl | yes
cxxl | yes

engine kits | exotic
--- | ---
DHS conversion | -
CDHS conversion | yes

heat sinks | exotic
--- | ---
Clan Double Heat Sink | yes

armor | exotic
--- | ---
ferros-fibrous | -
clan ferros-fibrous | yes
light ferros-fibrous | yes
heavy ferros-fibrous | yes
stealth | yes

structure | exotic
--- | ---
endo-steel | -
clan endo-steel | yes
endo-composite | yes

### HardpointFixMod

Originally a standalone mod, it is now incorporated into MechEngineer.

- Fix bad visual loadout issues, sometimes the wrong or ugly looking loadout was shown, a new algorithm should improve upon this.
  Sometimes attaching a PPC after having already attached many small lasers would hide the PPC, this should be fixed now.
- Restrict weapon loadouts to what is supported by the mech model.
  BattleTech has some of the best looking models due to MWO, however we never know what mechs support which model hardpoints and therefore we might mount weapons that can't be shown.

An example of how the weapon loadout restrictions feature work for the Highlander assault mech:
The left torso has 2 missle hardpoint slots, however only one can mount an LRM20, the other is limited to LRM10. Without this mod you can mount an LRM20 also for the second slot, but it visually would only be showing up as LRM10. With this mod you can't mount the second LRM20 anymore, you have to take either an LRM10 or LRM5. Of course SRMs are still an option.
The left arm is also limited to an LRM15 and you can't mount an LRM20 at all.

There are currently the following configuration settings available:

Setting | Type | Default | Description
--- | --- | --- | ---
enforceHardpointLimits | bool | true | set this to false to deactivate the hardpoint limits in mechlab
allowDefaultLoadoutWeapons | bool | true | always allow to reattach weapons the mech comes with by default
allowLRMsInSmallerSlotsForAll | bool | true | set this to false so only mechs with a proper sized hardpoint can field an LRM20.
allowLRMsInSmallerSlotsForMechs | string[] | default ["atlas"] | a list of mechs that can field larger LRM sizes even in smaller slots, even if allowLRMsInSmallerSlotsForAll is false.
allowLRMsInLargerSlotsForAll | bool | true | allow smaller sized LRMs to be used in larger sized hardpoint slots. E.g. an LRM10 should fit into an LRM20 slot.

#### Limitations

- can't replace weapons by dragging another weapon ontop of it, you have to remove the weapon first and then add another one (you dont have to leave the mechlab for this to work)
