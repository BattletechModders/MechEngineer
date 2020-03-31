using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BattleTech;
using MechEngineer.Features.CriticalEffects.Patches;
using MechEngineer.Features.DynamicSlots;

namespace MechEngineer.Features.HardpointFix.utils
{
    internal class WeaponComponentPrefabCalculator
    {
        private readonly ChassisDef chassisDef;
        private readonly IDictionary<MechComponentRef, string> weaponMappings = new Dictionary<MechComponentRef, string>();
        private readonly IDictionary<ChassisLocations, PrefabSets> fallbackPrefabs = new Dictionary<ChassisLocations, PrefabSets>();
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
            var usedSlots = weaponMappings.Where(x => x.Key.MountedLocation == location).Select(x => x.Value).Select(GroupNumber).Distinct().ToList();
            var requiredBlanks = availableBlanks.Where(x => !usedSlots.Contains(GroupNumber(x))).ToList();
            Control.mod.Logger.LogDebug($"Blank mappings for chassis {chassisDef.Description.Id} at {location} [{requiredBlanks.JoinAsString()}]");
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
                    Control.mod.Logger.LogError($"no prefabName mapped for weapon ComponentDefID={mechComponentRef.ComponentDefID} PrefabIdentifier={mechComponentRef.Def.PrefabIdentifier}");
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
            var bestSelection = new PrefabSelectionCandidate(availablePrefabSets, new List<PrefabMapping>());
            var currentCandidates = new List<PrefabSelectionCandidate> { bestSelection };
            
            //Control.mod.Logger.LogDebug($"CalculateMappingForLocation chassisDef={chassisDef.Description.Id} location={location} availablePrefabSets={availablePrefabSets} sortedComponentRefs=[{sortedComponentRefs.Select(x => x.ComponentDefID).JoinAsString()}]");

            foreach (var componentRef in sortedComponentRefs)
            {
               //Control.mod.Logger.LogDebug($" componentRef={componentRef.ComponentDefID}");

                var newCandidates = new List<PrefabSelectionCandidate>();
                foreach (var candidate in currentCandidates)
                {
                    //Control.mod.Logger.LogDebug($"  candidate={candidate}");

                    var hasNew = false;
                    foreach (var set in candidate.FreeSets)
                    {
                        //Control.mod.Logger.LogDebug($"   set={set}");

                        var prefab = set.GetCompatiblePrefab(componentRef.Def.PrefabIdentifier);
                        if (prefab == null)
                        {
                            continue;
                        }

                        var newMapping = new PrefabMapping(prefab, componentRef);
                        var newCandidate = candidate.CreateWithoutSet(set, newMapping);
                        newCandidates.Add(newCandidate);
                        hasNew = true;
                    }

                    if (!hasNew) // we didn't find anything better, so re-add the old one
                    {
                        newCandidates.Add(candidate);
                    }
                }

                currentCandidates = newCandidates;
            }

            foreach (var candidate in currentCandidates)
            {
                if (candidate.CompareTo(bestSelection) > 0)
                {
                    bestSelection = candidate;
                }
            }

            fallbackPrefabs[location] = bestSelection.FreeSets;
            if (bestSelection.Mappings.Count > 0)
            {
                Control.mod.Logger.LogDebug($"Mappings for chassis {chassisDef.Description.Id} at {location} [{bestSelection.Mappings.JoinAsString()}]");
                foreach (var mapping in bestSelection.Mappings)
                {
                    weaponMappings[mapping.MechComponentRef] = mapping.Prefab.Name;
                }
            }
        }

        private class PrefabSelectionCandidate : IComparable<PrefabSelectionCandidate>
        {
            internal PrefabSets FreeSets { get; }
            internal List<PrefabMapping> Mappings { get; }

            private int MajorScore { get; }
            private int MinorScore { get; }
            private int ThirdScore { get; }

            internal PrefabSelectionCandidate(PrefabSets freeSets, List<PrefabMapping> prefabsMappings)
            {
                FreeSets = freeSets;
                Mappings = prefabsMappings ?? new List<PrefabMapping>();

                MajorScore = Mappings.Count; // more mappings
                MinorScore = Mappings.Select(x => x.MechComponentRef.Def.InventorySize).Sum(); // bigger items
                ThirdScore = -Mappings.Select(x => x.Prefab.Index).Sum(); // lower indexed
            }

            public int CompareTo(PrefabSelectionCandidate other)
            {
                var major = MajorScore - other.MajorScore;
                if (major != 0)
                {
                    return major;
                }

                var minor = MinorScore - other.MinorScore;
                if (minor != 0)
                {
                    return major;
                }

                return ThirdScore - other.ThirdScore;
            }

            internal PrefabSelectionCandidate CreateWithoutSet(PrefabSet exclude, PrefabMapping newMapping)
            {
                var sets = FreeSets.Except(exclude);
                var mappings = new List<PrefabMapping>(Mappings) { newMapping };
                return new PrefabSelectionCandidate(sets, mappings);
            }

            public override string ToString()
            {
                return $"[freeSets={FreeSets},mappings=[{Mappings.JoinAsString()}]]";
            }
        }

        private class PrefabSets : IEnumerable<PrefabSet>
        {
            private readonly HashSet<PrefabSet> hashSet;

            internal PrefabSets()
            {
                hashSet = new HashSet<PrefabSet>(new IndexEqualityComparer());
            }

            internal void Add(PrefabSet set)
            {
                hashSet.Add(set);
            }

            internal int Count => hashSet.Count;

            internal PrefabSets Except(PrefabSet set)
            {
                var newHashSet = new HashSet<PrefabSet>(hashSet, new IndexEqualityComparer());
                newHashSet.Remove(set);
                return new PrefabSets(newHashSet);
            }

            private PrefabSets(HashSet<PrefabSet> hashSet)
            {
                this.hashSet = hashSet;
            }

            private sealed class IndexEqualityComparer : IEqualityComparer<PrefabSet>
            {
                public bool Equals(PrefabSet x, PrefabSet y)
                {
                    if (ReferenceEquals(x, y))
                    {
                        return true;
                    }

                    if (ReferenceEquals(x, null))
                    {
                        return false;
                    }

                    if (ReferenceEquals(y, null))
                    {
                        return false;
                    }

                    if (x.GetType() != y.GetType())
                    {
                        return false;
                    }

                    return x.Index == y.Index;
                }

                public int GetHashCode(PrefabSet obj)
                {
                    return obj.Index;
                }
            }

            public IEnumerator<PrefabSet> GetEnumerator()
            {
                return hashSet.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public override string ToString()
            {
                return $"[{hashSet.JoinAsString()}]";
            }
        }

        private class PrefabSet : IEnumerable<Prefab>
        {
            internal int Index { get; }
            private Dictionary<string, Prefab> Prefabs { get; }

            public IEnumerator<Prefab> GetEnumerator()
            {
                return Prefabs.Values.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            internal PrefabSet(int index, IEnumerable<string> prefabs)
            {
                Index = index;
                Prefabs = prefabs.Select(x => new Prefab(x)).ToDictionary(x => NormIdentifier(x.Identifier));
            }

            internal Prefab GetCompatiblePrefab(string prefabIdentifierNotNormalized)
            {
                var prefabIdentifier = NormIdentifier(prefabIdentifierNotNormalized);
                var compatibleTerms = GetCompatiblePrefabTerms(prefabIdentifier);

                var prefab = compatibleTerms
                    .Select(x => Prefabs.TryGetValue(x, out var prefab) ? prefab : null)
                    .FirstOrDefault(x => x != null);
                
                //Control.mod.Logger.LogDebug($"    prefabIdentifier={prefabIdentifier} compatibleTerms={compatibleTerms.JoinAsString()} prefabName={prefabName}");
                return prefab;
            }

            private static string NormIdentifier(string identifier)
            {
                return identifier.ToLowerInvariant();
            }

            private static readonly Dictionary<string, string[]> cachedCompatibleTerms = new Dictionary<string, string[]>();

            private static string[] GetCompatiblePrefabTerms(string prefabIdentifier)
            {
                if (!cachedCompatibleTerms.TryGetValue(prefabIdentifier, out var compatibleTerms))
                {
                    compatibleTerms = Control.settings.HardpointFix.WeaponPrefabMappings
                        .Where(x => string.Equals(x.PrefabIdentifier, prefabIdentifier, StringComparison.CurrentCultureIgnoreCase))
                        .Select(x => x.HardpointCandidates)
                        .SingleOrDefault();

                    if (compatibleTerms == null)
                    {
                        compatibleTerms = new[] { prefabIdentifier };
                    }

                    cachedCompatibleTerms[prefabIdentifier] = compatibleTerms;
                }

                return compatibleTerms;
            }

            public override string ToString()
            {
                return $"[index={Index}, Prefabs=[{Prefabs.Values.JoinAsString()}]]";
            }
        }

        private class Prefab
        {
            internal string Identifier { get; }
            internal string Name { get; }
            internal int Index { get; }

            internal Prefab(string name)
            {
                Name = name;
                var parts = Name.Split('_');
                
                // chrPrfWeap_thunderbolt_righttorso_(ppc|gauss|..)_eh1
                Identifier = parts[3];
                Index = int.Parse(name.Substring(name.Length - 1));
            }

            public override string ToString()
            {
                return Identifier;
            }
        }

        private class PrefabMapping
        {
            internal Prefab Prefab { get; }
            internal MechComponentRef MechComponentRef { get; }

            public PrefabMapping(Prefab prefab, MechComponentRef mechComponentRef)
            {
                Prefab = prefab;
                MechComponentRef = mechComponentRef;
            }

            public override string ToString()
            {
                return $"[Prefab={Prefab}, MechComponentRef={MechComponentRef.ComponentDefID}]";
            }
        }

        private PrefabSets GetAvailablePrefabSetsForLocation(ChassisLocations location)
        {
            var weaponsData = GetWeaponData(location);
            var sets = new PrefabSets();
            if (weaponsData.weapons == null)
            {
                //Control.mod.Logger.LogDebug($"no hardpoint data found for {chassisDef.Description.Id} at {location}");
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
                        Control.mod.Logger.LogWarning($"error processing hardpoint data for {chassisDef.Description.Id} at {location}: index={index} weapons=[{weapons?.JoinAsString()}]", e);
                        //throw;
                    }
                }
            }
            return sets;
        }

        // TODO merge with other GetAvailable
        private string[] GetAvailableBlankPrefabsForLocation(ChassisLocations location)
        {
            var weaponsData = GetWeaponData(location);
            return weaponsData.blanks ?? new string[0];
        }

        private HardpointDataDef._WeaponHardpointData GetWeaponData(ChassisLocations location)
        {
            var locationString = VHLUtils.GetStringFromLocation(location);
            var weaponsData = chassisDef.HardpointDataDef.HardpointData.FirstOrDefault(x => x.location == locationString);
            return weaponsData;
        }
    }
}