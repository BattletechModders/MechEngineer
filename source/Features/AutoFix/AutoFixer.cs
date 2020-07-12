using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using CustomComponents;
using MechEngineer.Features.ArmorStructureRatio;
using MechEngineer.Features.DynamicSlots;
using MechEngineer.Features.Engines;
using MechEngineer.Features.Engines.Helper;

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
                    Control.mod.Logger.LogError(e);
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

            Control.mod.Logger.Log($"Auto fixing mechDef={mechDef.Description.Id} chassisDef={mechDef.Chassis.Description.Id}");

            MechDefBuilder builder;
            {
                var inventory = mechDef.Inventory.ToList();
                foreach (var componentRef in inventory)
                {
                    Control.mod.Logger.LogDebug($" {componentRef.ComponentDefID}{(componentRef.IsFixed?" (fixed)":"")} at {componentRef.MountedLocation}");
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

            Engine engine = null;
            if (res.CoreDef != null)
            {
                engine = new Engine(res.CoolingDef, res.HeatBlockDef, res.CoreDef, res.Weights, new List<MechComponentRef>());
            }
            else
            {
                var freeTonnage = CalcFreeTonnage();
                var jumpJetList = builder.Inventory.Where(x => x.ComponentDefType == ComponentType.JumpJet).ToList();
                var engineCandidates = new LinkedList<Engine>();

                var engineCoreDefs = mechDef.DataManager.HeatSinkDefs
                    .Select(hs => hs.Value)
                    .Select(hs => hs.GetComponent<EngineCoreDef>())
                    .Where(c => c != null)
                    .OrderByDescending(x => x.Rating);

                foreach (var coreDef in engineCoreDefs)
                {
                    {
                        var candidate = new Engine(res.CoolingDef, res.HeatBlockDef, coreDef, res.Weights, new List<MechComponentRef>());
                        engineCandidates.AddFirst(candidate);
                    }
                    
                    {
                        // remove superfluous jump jets
                        var maxJetCount = coreDef.GetMovement(mechDef.Chassis.Tonnage).JumpJetCount;
                        while (jumpJetList.Count > maxJetCount)
                        {
                            var lastIndex = jumpJetList.Count - 1;
                            var jumpJet = jumpJetList[lastIndex];
                            freeTonnage += jumpJet.Def.Tonnage;
                            builder.Remove(jumpJet);
                            jumpJetList.Remove(jumpJet);
                        }
                    }

                    // go through all candidates, use the larger count anyway
                    foreach (var candidate in engineCandidates)
                    {
                        if (candidate.TotalTonnage <= freeTonnage)
                        {
                            engine = candidate;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (engine != null)
                    {
                        break;
                    }
                }

                if (engine != null)
                {
                    Control.mod.Logger.LogDebug($" maxEngine={engine.CoreDef} freeTonnage={freeTonnage}");
                    {
                        var dummyCore = builder.Inventory.FirstOrDefault(r => r.ComponentDefID == AutoFixerFeature.settings.MechDefCoreDummy);
                        if (dummyCore != null)
                        {
                            builder.Remove(dummyCore);
                        }
                    }

                    // add engine
                    builder.Add(engine.CoreDef.Def, ChassisLocations.CenterTorso, true);
                }
            }

            if (engine == null)
            {
                return;
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
                }

                // add same amount of compatible heat sinks
                foreach (var unused in incompatibleHeatSinks)
                {
                    builder.Add(engineHeatSinkDef.Def);
                }
            }

            // add free heat sinks
            {
                var max = engine.HeatSinkExternalFreeMaxCount;
                for (var i = 0; i < max; i++)
                {
                    if (!builder.Add(engineHeatSinkDef.Def))
                    {
                        break;
                    }
                }
            }

            // convert external heat sinks into internal ones
            {
                var max = engine.HeatSinkInternalAdditionalMaxCount;
                var current = engine.EngineHeatBlockDef.HeatSinkCount;

                var heatSinks = builder.Inventory
                    .Where(r => r.Def.Is<EngineHeatSinkDef>(out var hs) && hs.HSCategory == engineHeatSinkDef.HSCategory)
                    .ToList();

                for (; current < max && heatSinks.Count > 0; current++)
                {
                    var component = heatSinks[0];
                    heatSinks.RemoveAt(0);
                    builder.Remove(component);
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
                }
            }
            
            // find any overused location
            // TODO find out why locational dynamic slots are ignored
            if (builder.HasOveruseAtAnyLocation())
            {
                Control.mod.Logger.LogError($" Overuse found");
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
                    if (!builder.Add(item.Def))
                    {
                        Control.mod.Logger.LogError($" Component {item.ComponentDefID} from {item.MountedLocation} can't be re-added");
                    }
                }
            }
            
            mechDef.SetInventory(builder.Inventory.OrderBy(element => element, new OrderComparer()).ToArray());

            {
                var freeTonnage = CalcFreeTonnage();
                if (freeTonnage > 0)
                {
                    // TODO add armor for each location with free tonnage left
                }
                else if (freeTonnage < 0)
                {
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

                    while (removableItems.Count > 0 && freeTonnage < 0)
                    {
                        var item = removableItems[0];
                        removableItems.RemoveAt(0);
                        freeTonnage += item.Def.Tonnage;
                        builder.Remove(item);
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
            if (c.IsFixed)
            {
                return false;
            }

            var def = c.Def;
            
            if (def == null)
            {
                return false;
            }

            return def.ComponentType == ComponentType.HeatSink || def.ComponentType == ComponentType.JumpJet;
        }
    }
}