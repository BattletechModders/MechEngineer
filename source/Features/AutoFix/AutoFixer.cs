using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using CustomComponents;
using MechEngineer.Features.ArmorStructureRatio;
using MechEngineer.Features.DynamicSlots;
using MechEngineer.Features.Engines;
using MechEngineer.Features.Engines.Helper;
using MechEngineer.Features.OverrideTonnage;

namespace MechEngineer.Features.AutoFix
{
    internal class AutoFixer : IAutoFixMechDef
    {
        internal static AutoFixer Shared = new AutoFixer();

        public void AutoFix(List<MechDef> mechDefs, SimGameState simgame)
        {
            // we dont fix save games anymore, have to have money and time to fix an ongoing campaign
            if (simgame != null)
            {
                return;
            }
            
            foreach (var mechDef in mechDefs)
            {
                try
                {
                    AutoFixMechDef(mechDef);
                }
                catch (Exception e)
                {
                    Control.Logger.Error.Log(e);
                }
            }
        }

        public void AutoFixMechDef(MechDef mechDef)
        {
            if (!AutoFixerFeature.settings.MechDefEngine)
            {
                return;
            }

            if (!AutoFixerFeature.settings.MechTagsAutoFixEnabled.Any(mechDef.MechTags.Contains))
            {
                return;
            }

            Control.Logger.Info.Log($"Auto fixing mechDef={mechDef.Description.Id} chassisDef={mechDef.Chassis.Description.Id}");

            MechDefBuilder builder;
            {
                var inventory = mechDef.Inventory.ToList();
                if (Control.Logger.Debug != null)
                {
                    foreach (var componentRef in inventory)
                    {
                        Control.Logger.Debug.Log($" {componentRef.ComponentDefID}{(componentRef.IsFixed ? " (fixed)" : "")} at {componentRef.MountedLocation}");
                    }
                }

                builder = new MechDefBuilder(mechDef.Chassis, inventory);
            }

            ArmorStructureRatioFeature.Shared.AutoFixMechDef(mechDef);

            var res = EngineSearcher.SearchInventory(builder.Inventory);

            var engineHeatSinkDef = mechDef.DataManager.HeatSinkDefs.Get(res.CoolingDef.HeatSinkDefId).GetComponent<EngineHeatSinkDef>();

            float CalcFreeTonnage()
            {
                float currentTotalTonnage = 0, maxValue = 0;
                MechStatisticsRules.CalculateTonnage(mechDef, ref currentTotalTonnage, ref maxValue);
                var freeTonnage = mechDef.Chassis.Tonnage - currentTotalTonnage;
                return freeTonnage;
            }

            if (!EngineFeature.settings.AllowMixingHeatSinkTypes)
            {
                // remove incompatible heat sinks
                var incompatibleHeatSinks = builder.Inventory
                    .Where(r => r.Def.Is<EngineHeatSinkDef>(out var hs) && hs.HSCategory != engineHeatSinkDef.HSCategory)
                    .ToList();

                foreach (var incompatibleHeatSink in incompatibleHeatSinks)
                {
                    builder.Remove(incompatibleHeatSink);
                    builder.Add(engineHeatSinkDef.Def, ChassisLocations.Head, true);
                    Control.Logger.Debug?.Log($" Converted incompatible heat sinks to compatible ones");
                }
            }

            Engine engine = null;
            if (res.CoreDef != null)
            {
                Control.Logger.Debug?.Log($" Found an existing engine");
                engine = new Engine(res.CoolingDef, res.HeatBlockDef, res.CoreDef, res.Weights, new List<MechComponentRef>());

                // convert external heat sinks into internal ones
                // TODO only to make space if needed, drop the rest of the heat sinks

                {
                    var max = engine.HeatSinkInternalAdditionalMaxCount;
                    var oldCurrent = engine.EngineHeatBlockDef.HeatSinkCount;
                    var current = oldCurrent;

                    var heatSinks = builder.Inventory
                        .Where(r => r.Def.Is<EngineHeatSinkDef>(out var hs) && hs.HSCategory == engineHeatSinkDef.HSCategory)
                        .ToList();

                    while (current < max && heatSinks.Count > 0)
                    {
                        var component = heatSinks[0];
                        heatSinks.RemoveAt(0);
                        builder.Remove(component);
                        current++;
                    }
                    
                    if (current > 0)
                    {
                        var heatBlock = builder.Inventory.FirstOrDefault(r => r.Def.Is<EngineHeatBlockDef>());
                        if (heatBlock != null)
                        {
                            builder.Remove(heatBlock);
                        }

                        var heatBlockDefId = $"{AutoFixerFeature.settings.MechDefHeatBlockDef}_{current}";
                        var def = mechDef.DataManager.HeatSinkDefs.Get(heatBlockDefId);
                        builder.Add(def, ChassisLocations.CenterTorso, true);

                        Control.Logger.Debug?.Log($" Converted external heat sinks ({current - oldCurrent}) to internal ones (to make space)");
                    }
                }
            }
            else
            {
                var freeTonnage = CalcFreeTonnage();

                Control.Logger.Debug?.Log($" Finding engine for freeTonnage={freeTonnage}");

                var jumpJets = builder.Inventory.Where(x => x.ComponentDefType == ComponentType.JumpJet).ToList();
                var jumpJetTonnage = jumpJets.Select(x => x.Def.Tonnage).FirstOrDefault(); //0 if no jjs

                var externalHeatSinks = builder.Inventory
                    .Where(r => r.Def.Is<EngineHeatSinkDef>(out var hs) && hs.HSCategory == engineHeatSinkDef.HSCategory)
                    .ToList();
                var internalHeatSinksCount = res.HeatBlockDef.HeatSinkCount;

                var engineCandidates = new List<Engine>();

                var engineCoreDefs = mechDef.DataManager.HeatSinkDefs
                    .Select(hs => hs.Value)
                    .Select(hs => hs.GetComponent<EngineCoreDef>())
                    .Where(c => c != null)
                    .OrderByDescending(x => x.Rating);

                var removedExternalHeatSinksOverUse = false;

                foreach (var coreDef in engineCoreDefs)
                {
                    {
                        // remove superfluous jump jets
                        var maxJetCount = coreDef.GetMovement(mechDef.Chassis.Tonnage).JumpJetCount;
                        while (jumpJets.Count > maxJetCount)
                        {
                            var lastIndex = jumpJets.Count - 1;
                            var jumpJet = jumpJets[lastIndex];
                            freeTonnage += jumpJet.Def.Tonnage;
                            builder.Remove(jumpJet);
                            jumpJets.Remove(jumpJet);

                            Control.Logger.Debug?.Log("  Removed JumpJet");
                        }
                    }

                    {
                        var candidate = new Engine(res.CoolingDef, res.HeatBlockDef, coreDef, res.Weights, new List<MechComponentRef>());

                        Control.Logger.Debug?.Log($"  candidate id={coreDef.Def.Description.Id} TotalTonnage={candidate.TotalTonnage}");

                        engineCandidates.Add(candidate);

                        var internalHeatSinksMax = candidate.HeatSinkInternalAdditionalMaxCount;

                        // convert external ones to internal ones
                        while (internalHeatSinksCount < internalHeatSinksMax && externalHeatSinks.Count > 0)
                        {
                            var component = externalHeatSinks[0];
                            externalHeatSinks.RemoveAt(0);
                            builder.Remove(component);
                            internalHeatSinksCount++;

                            Control.Logger.Debug?.Log("  ~Converted external heat sinks to internal ones");
                        }

                        // this only runs on the engine that takes the most heat sinks (since this is in a for loop with rating descending order)
                        // that way we only remove external heat sinks that couldn't be moved internally
                        while (!removedExternalHeatSinksOverUse && externalHeatSinks.Count > 0)
                        {
                            var component = externalHeatSinks[0];
                            externalHeatSinks.RemoveAt(0);
                            builder.Remove(component);
                            var newComponent = builder.Add(component.Def);
                            if (newComponent == null)
                            {
                                Control.Logger.Debug?.Log("  Removed external heat sink that doesn't fit");
                                // might still need to remove some
                                continue;
                            }
                            // addition worked
                            externalHeatSinks.Add(newComponent);
                            break;
                        }
                        removedExternalHeatSinksOverUse = true;

                        // convert internal ones to external ones
                        while (internalHeatSinksCount > internalHeatSinksMax)
                        {
                            if (builder.Add(engineHeatSinkDef.Def) == null)
                            {
                                Control.Logger.Debug?.Log("  ~Dropped external when converting from internal");
                                freeTonnage++;
                            }
                            else
                            {
                                
                                Control.Logger.Debug?.Log("  ~Converted internal heat sink to external one");
                            }
                            internalHeatSinksCount--;
                        }

                        // remove candidates that make no sense anymore
                        // TODO not perfect and maybe too large for small mechs
                        engineCandidates = engineCandidates.Where(x => PrecisionUtils.SmallerOrEqualsTo(x.TotalTonnage, freeTonnage + 6*engineHeatSinkDef.Def.Tonnage + jumpJetTonnage)).ToList();
                    }

                    // go through all candidates, larger first
                    engine = engineCandidates.FirstOrDefault(candidate => PrecisionUtils.SmallerOrEqualsTo(candidate.TotalTonnage, freeTonnage));

                    if (engine != null)
                    {
                        break;
                    }
                }

                if (engine != null)
                {
                    Control.Logger.Debug?.Log($" engine={engine.CoreDef} freeTonnage={freeTonnage}");
                    var dummyCore = builder.Inventory.FirstOrDefault(r => r.ComponentDefID == AutoFixerFeature.settings.MechDefCoreDummy);
                    builder.Remove(dummyCore);
                    builder.Add(engine.CoreDef.Def, ChassisLocations.CenterTorso, true);

                    // convert internal heat sinks back as external ones if the mech can fit it
                    while (internalHeatSinksCount > 0 && builder.Add(engineHeatSinkDef.Def) != null)
                    {
                        internalHeatSinksCount--;
                    }

                    if (internalHeatSinksCount > 0)
                    {
                        var heatBlock = builder.Inventory.FirstOrDefault(r => r.Def.Is<EngineHeatBlockDef>());
                        if (heatBlock != null)
                        {
                            builder.Remove(heatBlock);
                        }

                        var heatBlockDefId = $"{AutoFixerFeature.settings.MechDefHeatBlockDef}_{internalHeatSinksCount}";
                        var def = mechDef.DataManager.HeatSinkDefs.Get(heatBlockDefId);
                        builder.Add(def, ChassisLocations.CenterTorso, true);
                    }
                }
            }

            if (engine == null)
            {
                return;
            }

            // add free heat sinks
            {
                var max = engine.HeatSinkExternalFreeMaxCount;
                var current = builder.Inventory
                    .Count(r => r.Def.Is<EngineHeatSinkDef>(out var hs)
                                && hs.HSCategory == engineHeatSinkDef.HSCategory);
                for (var i = current; i < max; i++)
                {
                    builder.Add(engineHeatSinkDef.Def, ChassisLocations.Head, true);
                }
            }
            
            // find any overused location
            if (builder.HasOveruseAtAnyLocation())
            {
                Control.Logger.Info.Log($" Overuse found");
                // heatsinks, upgrades
                var itemsToBeReordered = builder.Inventory
                    .Where(IsMovable)
                    .OrderBy(c => MechDefBuilder.LocationCount(c.Def.AllowedLocations))
                    .ThenByDescending(c => c.Def.InventorySize)
                    .ToList();

                // remove all items that can be reordered: heatsinks, upgrades
                foreach (var item in itemsToBeReordered)
                {
                    builder.Remove(item);
                }

                // then add most restricting, and then largest items first (probably double head sinks)
                foreach (var item in itemsToBeReordered)
                {
                    if (builder.Add(item.Def) == null)
                    {
                        Control.Logger.Warning.Log($" Component {item.ComponentDefID} from {item.MountedLocation} can't be re-added");
                    }
                    else
                    {
                        Control.Logger.Debug?.Log($"  Component {item.ComponentDefID} re-added");
                    }
                }
            }
            
            mechDef.SetInventory(builder.Inventory.OrderBy(element => element, new OrderComparer()).ToArray());

            {
                var freeTonnage = CalcFreeTonnage();
                if (PrecisionUtils.Equals(freeTonnage, 0))
                {
                    // do nothing
                }
                else if (freeTonnage > 0)
                {
                    // TODO add armor for each location with free tonnage left
                }
                else if (freeTonnage < 0)
                {
                    Control.Logger.Debug?.Log($" Found over tonnage {-freeTonnage}");
                    var removableItems = builder.Inventory
                        .Where(IsRemovable)
                        .OrderBy(c => c.Def.Tonnage)
                        .ThenByDescending(c => c.Def.InventorySize)
                        .ThenByDescending(c =>
                        {
                            switch (c.ComponentDefType)
                            {
                                case ComponentType.HeatSink:
                                    return 2;
                                case ComponentType.JumpJet:
                                    return 1;
                                default:
                                    return 0;
                            }
                        })
                        .ToList();

                    while (removableItems.Count > 0 && PrecisionUtils.SmallerThan(freeTonnage, 0))
                    {
                        var item = removableItems[0];
                        removableItems.RemoveAt(0);
                        freeTonnage += item.Def.Tonnage;
                        builder.Remove(item);
                        Control.Logger.Debug?.Log($"  Removed item, freeTonnage={freeTonnage}");
                    }
                }
            }

            mechDef.SetInventory(builder.Inventory.OrderBy(element => element, new OrderComparer()).ToArray());
        }

        private class OrderComparer : IComparer<MechComponentRef>
        {
            private readonly SorterComparer comparer = new SorterComparer();
            public int Compare(MechComponentRef x, MechComponentRef y)
            {
                return comparer.Compare(x?.Def, y?.Def);
            }
        }

        private static bool IsMovable(MechComponentRef c)
        {
            if (!IsRemovable(c))
            {
                return false;
            }

            var def = c.Def;

            // items in arms and legs are usually bound to a certain side, so lets ignore them from relocation
            if (MechDefBuilder.LocationCount(def.AllowedLocations) <= 2)
            {
                return false;
            }

            if (def.Is<Category>(out var category) && category.CategoryDescriptor.UniqueForLocation)
            {
                return false;
            }

            return true;
        }

        private static bool IsRemovable(MechComponentRef c)
        {
            var def = c.Def;
            
            if (def == null)
            {
                return false;
            }

            if (c.IsFixed || c.Def.Is<Category>(out var category) && category.CategoryDescriptor.Required)
            {
                return false;
            }

            return def.ComponentType == ComponentType.HeatSink || def.ComponentType == ComponentType.JumpJet;
        }
    }
}