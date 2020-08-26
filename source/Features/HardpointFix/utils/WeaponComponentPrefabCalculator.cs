﻿using BattleTech;
using MechEngineer.Features.CriticalEffects.Patches;
using MechEngineer.Features.DynamicSlots;
using MechEngineer.Features.HardpointFix.prefab;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MechEngineer.Features.HardpointFix.utils
{
    internal class WeaponComponentPrefabCalculator
    {
        private readonly ChassisDef chassisDef;
        private readonly IDictionary<MechComponentRef, string> weaponMappings = new Dictionary<MechComponentRef, string>();
        private readonly IDictionary<ChassisLocations, List<PrefabSet>> fallbackPrefabs = new Dictionary<ChassisLocations, List<PrefabSet>>();
        private readonly HashSet<string> preMappedPrefabNames;

        internal WeaponComponentPrefabCalculator(ChassisDef chassisDef, List<MechComponentRef> allComponentRefs, ChassisLocations location = ChassisLocations.All)
        {
            this.chassisDef = chassisDef;

            preMappedPrefabNames = allComponentRefs.Select(c => GetPredefinedPrefabName(c)).Where(x => x != null).ToHashSet();

            var componentRefs = allComponentRefs
                .Where(c => c.ComponentDefType == ComponentType.Weapon)
                .Where(c => GetPredefinedPrefabName(c) == null)
                .OrderByDescending(c => c.Def.InventorySize)
                .ThenByDescending(c => c.Def.Tonnage)
                .ThenByDescending(c => ((WeaponDef)c.Def).Damage)
                .ThenByDescending(c => c.ComponentDefID)
                .ToList();

            if (!MechDefBuilder.Locations.Contains(location))
            {
                location = ChassisLocations.All;
            }

            if (location == ChassisLocations.All)
            {
                foreach (var tlocation in MechDefBuilder.Locations)
                {
                    var localRefs = componentRefs.Where(c => c.MountedLocation == tlocation).ToList();
                    CalculateMappingForLocation(tlocation, localRefs);
                }
            }
            else
            {
                CalculateMappingForLocation(location, componentRefs);
            }
        }

        // chrPrfWeap_battlemaster_leftarm_ac20_bh1 -> 1
        internal static string GroupNumber(string prefab)
        {
            return prefab.Substring(prefab.Length - 1, 1);
        }

        internal static string PrefabHardpoint(string prefab)
        {
            var lastIndex = prefab.LastIndexOf("_");
            return prefab.Substring(lastIndex + 1);
        }

        internal static int GroupNumberAsInt(string prefab)
        {
            var gn = GroupNumber(prefab);
            if (int.TryParse(gn, out var n))
            {
                return n;
            }
            return 0;
        }

        // TODO cache?
        internal List<string> GetRequiredBlankPrefabNamesInLocation(ChassisLocations location)
        {
            var availableBlanks = GetAvailableBlankPrefabsForLocation(location);
            var usedSlots = weaponMappings.Where(x => x.Key.MountedLocation == location).Select(x => x.Value).Select(PrefabHardpoint).Distinct().ToList();
            var requiredBlanks = availableBlanks.Where(x => !usedSlots.Contains(PrefabHardpoint(x))).ToList();
            Control.Logger.Debug?.Log($"Blank mappings for chassis {chassisDef.Description.Id} at {location} [{requiredBlanks.JoinAsString()}]");
            return requiredBlanks;
        }

        internal int MappedComponentRefCount => weaponMappings.Count;

        internal string GetPrefabName(BaseComponentRef componentRef)
        {
            var pre = GetPredefinedPrefabName(componentRef);
            if (pre != null)
            {
                return pre;
            }
            if (componentRef is MechComponentRef mechComponentRef)
            {
                if (weaponMappings.TryGetValue(mechComponentRef, out var prefabName))
                {
                    return prefabName;
                }
                if (HardpointFixFeature.Shared.Settings.FallbackPrefabsForComponentDefIds.Contains(componentRef.ComponentDefID))
                {
                    return GetFallbackPrefabIdentifier(mechComponentRef);
                }
            }
            return null;
        }

        private string GetFallbackPrefabIdentifier(MechComponentRef mechComponentRef)
        {
            var location = mechComponentRef.MountedLocation;
            if (fallbackPrefabs.TryGetValue(location, out var sets))
            {
                // we assume not more than one fallback per location is required, meaning we don't have to remove fallback candidates
                var name = sets.Where(x => x.Any()).Select(x => x.First().Name).FirstOrDefault();
                if (name == null)
                {
                    name = chassisDef.HardpointDataDef.HardpointData?[0].weapons?[0]?[0];
                }
                if (name == null)
                {
                    Control.Logger.Error.Log($"no prefabName mapped for weapon ComponentDefID={mechComponentRef.ComponentDefID} PrefabIdentifier={mechComponentRef.Def.PrefabIdentifier}");
                }
                return name;
            }
            return null;
        }

        private string GetPredefinedPrefabName(BaseComponentRef componentRef)
        {
            if (componentRef.Def.PrefabIdentifier.StartsWith("chrPrfWeap", true, CultureInfo.InvariantCulture)
                || componentRef.Def.PrefabIdentifier.StartsWith("chrPrfComp", true, CultureInfo.InvariantCulture))
            {
                return componentRef.Def.PrefabIdentifier;
            }
            return null;
        }

        private void CalculateMappingForLocation(ChassisLocations location, List<MechComponentRef> sortedComponentRefs)
        {
            var availablePrefabSets = GetAvailablePrefabSetsForLocation(location);
            var bestSelection = new PrefabSelectionCandidate(availablePrefabSets);

            Control.Logger.Debug?.Log($"CalculateMappingForLocation chassisDef={chassisDef.Description.Id} location={location} availablePrefabSets={availablePrefabSets.JoinAsString()} sortedComponentRefs=[{sortedComponentRefs.Select(x => x.ComponentDefID).JoinAsString()}]");

            foreach (var componentRef in sortedComponentRefs)
            {
                var prefabIdentifier = Prefab.NormIdentifier(componentRef.Def.PrefabIdentifier);
                var compatibleTerms = CompatibleUtils.GetCompatiblePrefabTerms(prefabIdentifier);

                Control.Logger.Debug?.Log($" componentRef={componentRef.ComponentDefID} prefabIdentifier={prefabIdentifier} compatibleTerms={compatibleTerms.JoinAsString()}");

                foreach (var compatibleTerm in compatibleTerms)
                {
                    var prefab = bestSelection.FreeSets
                        .Select(x => x.GetPrefabByIdentifier(compatibleTerm))
                        .FirstOrDefault(x => x != null);

                    if (prefab == null)
                    {
                        continue;
                    }

                    Control.Logger.Debug?.Log($"  prefab={prefab}");

                    var newMapping = new PrefabMapping(prefab, componentRef);
                    bestSelection = bestSelection.CreateWithoutPrefab(prefab, newMapping);

                    break;
                }

            }
            fallbackPrefabs[location] = bestSelection.FreeSets;
            Control.Logger.Debug?.Log($"Mappings for chassis {chassisDef.Description.Id} at {location} [{bestSelection.Mappings.JoinAsString()}]");
            foreach (var mapping in bestSelection.Mappings)
            {
                weaponMappings[mapping.MechComponentRef] = mapping.Prefab.Name;
            }
        }

        internal List<PrefabSet> GetAvailablePrefabSetsForLocation(ChassisLocations location)
        {
            var weaponsData = GetWeaponData(location);
            var sets = new List<PrefabSet>();
            if (weaponsData.weapons == null)
            {
                Control.Logger.Debug?.Log($"no hardpoint data found for {chassisDef.Description.Id} at {location}");
            }
            else
            {
                foreach (var weapons in weaponsData.weapons)
                {
                    var index = sets.Count;
                    try
                    {
                        var set = new PrefabSet(index, weapons.Where(x => !preMappedPrefabNames.Contains(x)));
                        sets.Add(set);
                    }
                    catch (Exception e)
                    {
                        Control.Logger.Warning.Log($"error processing hardpoint data for {chassisDef.Description.Id} at {location}: index={index} weapons=[{weapons?.JoinAsString()}]", e);
                        //throw;
                    }
                }
            }
            return sets;
        }

        // TODO merge with other GetAvailable
        internal string[] GetAvailableBlankPrefabsForLocation(ChassisLocations location)
        {
            var weaponsData = GetWeaponData(location);
            return weaponsData.blanks ?? new string[0];
        }

        internal HardpointDataDef._WeaponHardpointData GetWeaponData(ChassisLocations location)
        {
            var locationString = VHLUtils.GetStringFromLocation(location);
            var weaponsData = chassisDef.HardpointDataDef.HardpointData.FirstOrDefault(x => x.location == locationString);
            return weaponsData;
        }
    }
}