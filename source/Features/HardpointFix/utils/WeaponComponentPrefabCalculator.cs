using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using MechEngineer.Features.CriticalEffects.Patches;
using MechEngineer.Features.DynamicSlots;

namespace MechEngineer.Features.HardpointFix.utils
{
    internal class WeaponComponentPrefabCalculator
    {
        private readonly ChassisDef chassisDef;
        private readonly IDictionary<MechComponentRef, string> cacheMappings = new Dictionary<MechComponentRef, string>();

        internal WeaponComponentPrefabCalculator(ChassisDef chassisDef, List<MechComponentRef> componentRefs, ChassisLocations location = ChassisLocations.All)
        {
            this.chassisDef = chassisDef;
            componentRefs = componentRefs
                .OrderByDescending(c => ((WeaponDef)c.Def).InventorySize)
                .ThenByDescending(c => ((WeaponDef)c.Def).Tonnage)
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
                    var locationComponentRefs = componentRefs.Where(c => c.MountedLocation == tlocation).ToList();
                    CalculateMappingForLocation(tlocation, locationComponentRefs);
                }
            }
            else
            {
                CalculateMappingForLocation(location, componentRefs);
            }
        }

        internal int MappedComponentRefCount => cacheMappings.Count;

        internal string GetPrefabName(MechComponentRef componentRef)
        {
            return cacheMappings.TryGetValue(componentRef, out var value) ? value : null;
        }

        private void CalculateMappingForLocation(ChassisLocations location, List<MechComponentRef> sortedComponentRefs)
        {
            //Control.mod.Logger.LogDebug($"CalculateMappingForLocation chassisDef={chassisDef.Description.Id} location={location} sortedComponentRefs=[{sortedComponentRefs.Select(x => x.ComponentDefID).JoinAsString()}]");

            var bestSelection = new PrefabSelectionCandidate(GetAvailablePrefabSetsForLocation(location), new List<PrefabMapping>());
            var currentCandidates = new List<PrefabSelectionCandidate> { bestSelection };

            foreach (var componentRef in sortedComponentRefs)
            {
               // Control.mod.Logger.LogDebug($" componentRef={componentRef.ComponentDefID}");

                var newCandidates = new List<PrefabSelectionCandidate>();
                foreach (var candidate in currentCandidates)
                {
                    //Control.mod.Logger.LogDebug($"  candidate={candidate}");

                    var hasNew = false;
                    foreach (var set in candidate.FreeSets)
                    {
                        //Control.mod.Logger.LogDebug($"   set={set}");

                        var prefabName = set.GetCompatiblePrefab(componentRef.Def.PrefabIdentifier);
                        if (prefabName == null)
                        {
                            continue;
                        }

                        var newMapping = new PrefabMapping(prefabName, componentRef);
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

            if (bestSelection.Mappings.Count < 1)
            {
                return;
            }

            Control.mod.Logger.LogDebug($"Mappings for chassis {chassisDef.Description.Id} at {location} [{bestSelection.Mappings.JoinAsString()}]");
            foreach (var mapping in bestSelection.Mappings)
            {
                cacheMappings[mapping.MechComponentRef] = mapping.PrefabName;
            }
        }

        private class PrefabSelectionCandidate : IComparable<PrefabSelectionCandidate>
        {
            internal PrefabSets FreeSets { get; }
            internal List<PrefabMapping> Mappings { get; }

            private int MajorScore { get; }
            private int MinorScore { get; }

            internal PrefabSelectionCandidate(PrefabSets freeSets, List<PrefabMapping> prefabsMappings)
            {
                FreeSets = freeSets;
                Mappings = prefabsMappings ?? new List<PrefabMapping>();

                MajorScore = Mappings.Count;
                MinorScore = Mappings.Select(x => x.MechComponentRef.Def.InventorySize).Sum();
            }

            public int CompareTo(PrefabSelectionCandidate other)
            {
                var major = MajorScore - other.MajorScore;
                if (major != 0)
                {
                    return major;
                }

                return MinorScore - other.MinorScore;
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

        private class PrefabSet
        {
            internal int Index { get; }
            private Dictionary<string, Prefab> Prefabs { get; }

            internal PrefabSet(int index, string[] prefabs)
            {
                Index = index;
                Prefabs = prefabs.Select(x => new Prefab(x)).ToDictionary(x => NormIdentifier(x.Identifier));
            }

            internal string GetCompatiblePrefab(string prefabIdentifierNotNormalized)
            {
                var prefabIdentifier = NormIdentifier(prefabIdentifierNotNormalized);
                var compatibleTerms = GetCompatiblePrefabTerms(prefabIdentifier);

                var prefabName = compatibleTerms
                    .Select(x => Prefabs.TryGetValue(x, out var prefab) ? prefab.Name : null)
                    .FirstOrDefault(x => x != null);
                
                //Control.mod.Logger.LogDebug($"    prefabIdentifier={prefabIdentifier} compatibleTerms={compatibleTerms.JoinAsString()} prefabName={prefabName}");
                return prefabName;
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

            internal Prefab(string name)
            {
                Name = name;
                var parts = Name.Split('_');
                
                // chrPrfWeap_thunderbolt_righttorso_hardpoint_eh1
                Identifier = parts[3];
            }
            
            // not needed
            //internal int Index;
            //private PrefabType Type;
            //private enum PrefabType
            //{
            //    AH, BH, EH, MH
            //}

            public override string ToString()
            {
                return Identifier;
            }
        }

        private class PrefabMapping
        {
            internal string PrefabName { get; }
            internal MechComponentRef MechComponentRef { get; }

            public PrefabMapping(string prefabName, MechComponentRef mechComponentRef)
            {
                PrefabName = prefabName;
                MechComponentRef = mechComponentRef;
            }

            public override string ToString()
            {
                return $"[PrefabName={PrefabName}, MechComponentRef={MechComponentRef.ComponentDefID}]";
            }
        }

        private PrefabSets GetAvailablePrefabSetsForLocation(ChassisLocations location)
        {
            var locationString = VHLUtils.GetStringFromLocation(location);
            var weaponsData = chassisDef.HardpointDataDef.HardpointData.FirstOrDefault(x => x.location == locationString);
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
                        var set = new PrefabSet(index, weapons);
                        sets.Add(set);
                    }
                    catch (Exception e)
                    {
                        Control.mod.Logger.LogDebug($"error processing hardpoint data for {chassisDef.Description.Id} at {location}: index={index} weapons=[{weapons?.JoinAsString()}]", e);
                        throw;
                    }
                }
            }
            return sets;
        }
    }
}