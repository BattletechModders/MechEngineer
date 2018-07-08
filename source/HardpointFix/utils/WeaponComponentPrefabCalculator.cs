using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;

namespace MechEngineer
{
    internal class WeaponComponentPrefabCalculator
    {
        internal static readonly ChassisLocations[] SupportedLocations =
        {
            ChassisLocations.Head,
            ChassisLocations.MainBody,
            ChassisLocations.LeftArm,
            ChassisLocations.LeftTorso,
            ChassisLocations.RightArm,
            ChassisLocations.RightTorso
        };

        private readonly ChassisDef chassisDef;
        private readonly IDictionary<MechComponentRef, string> mapping = new Dictionary<MechComponentRef, string>();

        internal WeaponComponentPrefabCalculator(ChassisDef chassisDef, List<MechComponentRef> componentRefs, ChassisLocations location = ChassisLocations.All)
        {
            this.chassisDef = chassisDef;
            componentRefs = componentRefs
                .OrderByDescending(c => ((WeaponDef) c.Def).InventorySize)
                .ThenByDescending(c => ((WeaponDef) c.Def).Tonnage)
                .ToList();

            if (!SupportedLocations.Contains(location))
            {
                location = ChassisLocations.All;
            }

            if (location == ChassisLocations.All)
            {
                var locations = SupportedLocations;

                foreach (var tlocation in locations)
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

        internal int NotMappedPrefabNameCount { private set; get; }

        internal string GetPrefabName(MechComponentRef componentRef)
        {
            return mapping.TryGetValue(componentRef, out var value) ? value : null;
        }

        internal string GetNewPrefabName(MechComponentRef componentRef, ChassisLocations location)
        {
            var availablePrefabNames = GetAvailablePrefabNamesForLocation(location);
            return GetAvailableWeaponComponentPrefabName(componentRef.Def.PrefabIdentifier.ToLower(), chassisDef.PrefabBase, availablePrefabNames);
        }

        private void CalculateMappingForLocation(ChassisLocations location, List<MechComponentRef> locationComponentRefs)
        {
            foreach (var componentRef in locationComponentRefs)
            {
                var prefabName = GetNewPrefabName(componentRef, location);
                if (prefabName == null)
                {
                    //Control.mod.Logger.LogDebug("could not find prefabName for " + (componentRef != null ? (componentRef.Def != null ? componentRef.Def.PrefabIdentifier : null) : null));
                    NotMappedPrefabNameCount++;
                    continue;
                }

                mapping[componentRef] = prefabName;
            }
        }

        private static string GetAvailableWeaponComponentPrefabName(string prefabId, string prefabBase, List<string> availablePrefabNames)
        {
            List<string> compatibleTerms;
            if (prefabId.Contains("lrm"))
            {
                var prefabIdFix = prefabId == "lrm20" ? "srm20" : prefabId; // yea sure, srm20 ...
                var order = new[] {"lrm5", "lrm10", "lrm15", "srm20"};
                var index = Array.IndexOf(order, prefabIdFix);

                compatibleTerms = new List<string> {prefabIdFix};

                if (Control.settings.AllowLRMInLargerSlotsForAll)
                {
                    for (var i = index + 1; i < order.Length; i++)
                    {
                        compatibleTerms.Add(order[i]);
                    }
                }

                if (Control.settings.AllowLRMInSmallerSlotsForAll || Control.settings.AllowLRMInSmallerSlotsForMechs.Contains(prefabBase))
                {
                    for (var i = index - 1; i >= 0; i--)
                    {
                        compatibleTerms.Add(order[i]);
                    }
                }
            }
            else if (prefabId.Contains("ac"))
            {
                compatibleTerms = new List<string> {prefabId, "ac"}; // fix for hunchback ac hardpoint
            }
            else
            {
                compatibleTerms = new List<string> {prefabId};
            }

            // if orderBy of availablePreabNames is from least important to more important, least important slots are filled first -> doesn't look nice though
            //foreach (var term in compatibleTerms)
            //{
            //    var prefabName = availablePrefabNames.FirstOrDefault(x => x.Contains("_" + term + "_"));
            //    if (prefabName != null)
            //    {
            //        return prefabName;
            //    }
            //}

            // since prefabNames is sorted by index and get available weapon component is already sorted from largest to smallest, we should have a nice sort order

            //Control.mod.Logger.LogDebug("availablePrefabNames for " + prefabBase);
            //availablePrefabNames.ForEach(Control.mod.Logger.LogDebug);
            //Control.mod.Logger.LogDebug("compatibleTerms for " + prefabId);
            //compatibleTerms.ForEach(Control.mod.Logger.LogDebug);

            var prefabName = compatibleTerms.Select(t => availablePrefabNames.FirstOrDefault(n => n.Contains("_" + t + "_"))).FirstOrDefault(n => n != null);
            //Control.mod.Logger.LogDebug("found prefabName " + prefabName);
            return prefabName;
        }

        private List<string> GetAvailablePrefabNamesForLocation(ChassisLocations location)
        {
            var hardpointDatas = chassisDef.HardpointDataDef.HardpointData
                .Where(x => x.location == VHLUtils.GetStringFromLocation(location));

            // SelectMany with string[][] crashes if compiled with Mono.CSharp (2.1.0.0) for Unity, newer Mono and VS compilers are fine
            var hpsets = new List<string[]>();
            foreach (var hardpointData in hardpointDatas)
            {
                foreach (var hpset in hardpointData.weapons)
                {
                    hpsets.Add(hpset);
                }
            }

            return hpsets
                .Where(hpset => !hpset.Intersect(mapping.Values).Any()) // only include hardpoint sets not yet used
                // commented out code not required, instead of filling the least used, we get the complete weapons list and sort by weapon sizes and fill from nicest index to ugliest index
                //.Select(hpset => RemoveUnwantedHardpoints(location, hpset)) // that allows our length order to work better
                //.OrderBy(hpset => hpset.Length) // sort hardpoints by how many weapon types are supported (use up the ones with less options first) - no -> use index order, lower index = nicer looking
                .SelectMany(hpset => hpset) // we don't care about groups anymore, just flatten everything into one stream
                .ToList();
        }

        private string[] RemoveUnwantedHardpoints(ChassisLocations location, string[] hardpointSet)
        {
            var counter = new HardpointCounter(chassisDef, location);

            IEnumerable<string> hardpoints = hardpointSet;
            if (counter.numBallistic == 0)
            {
                hardpoints = hardpoints.Where(hp => !hp.Contains("_bh"));
            }

            if (counter.numEnergy == 0 && counter.numSmall == 0)
            {
                hardpoints = hardpoints.Where(hp => !hp.Contains("_eh"));
            }

            if (counter.numMissile == 0)
            {
                hardpoints = hardpoints.Where(hp => !hp.Contains("_mh"));
            }

            if (counter.numSmall == 0)
            {
                hardpoints = hardpoints.Where(hp => !hp.Contains("_ah"));
            }

            return hardpoints.ToArray();
        }
    }
}