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
using UnityEngine;

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

            // if un-blacklisted, we assume it can be autofixed or it is already fixed
            if (mechDef.MechTags.Contains("BLACKLISTED"))
            {
                return;
            }

            Control.mod.Logger.Log($"Auto fixing mechDef={mechDef.Description.Id} chassisDef={mechDef.Chassis.Description.Id}");

            var builder = new MechDefBuilder(mechDef.Chassis, mechDef.Inventory.ToList());

            ArmorStructureRatioFeature.Shared.AutoFixMechDef(mechDef);

            var res = EngineSearcher.SearchInventory(builder.Inventory);

            res.HeatBlockDef ??= mechDef.DataManager.HeatSinkDefs.Get(AutoFixerFeature.settings.MechDefHeatBlockDef).GetComponent<EngineHeatBlockDef>();
            res.CoolingDef ??= mechDef.DataManager.HeatSinkDefs.Get(AutoFixerFeature.settings.MechDefCoolingDef).GetComponent<CoolingDef>();
            var engineHeatSinkDef = mechDef.DataManager.HeatSinkDefs.Get(res.CoolingDef.HeatSinkDefId).GetComponent<EngineHeatSinkDef>();

            Engine maxEngine = null;
            if (res.CoreDef != null)
            {
                maxEngine = new Engine(res.CoolingDef, res.HeatBlockDef, res.CoreDef, res.Weights, new List<MechComponentRef>());
            }

            float CalcFreeTonnage()
            {
                float currentTotalTonnage = 0, maxValue = 0;
                MechStatisticsRules.CalculateTonnage(mechDef, ref currentTotalTonnage, ref maxValue);
                var freeTonnage = mechDef.Chassis.Tonnage - currentTotalTonnage;
                return freeTonnage;
            }

            if (maxEngine == null)
            {
                var freeTonnage = CalcFreeTonnage();
                var jumpJetList = builder.Inventory.Where(x => x.ComponentDefType == ComponentType.JumpJet).ToList();
                var engines = new LinkedList<Engine>();

                var engineCoreDefs = mechDef.DataManager.HeatSinkDefs
                    .Select(hs => hs.Value)
                    .Select(hs => hs.GetComponent<EngineCoreDef>())
                    .Where(c => c != null)
                    .OrderByDescending(x => x.Rating);

                foreach (var coreDef in engineCoreDefs)
                {
                    {
                        var engine = new Engine(res.CoolingDef, res.HeatBlockDef, coreDef, res.Weights, new List<MechComponentRef>());
                        engines.AddFirst(engine);
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

                    foreach (var engine in engines)
                    {
                        if (engine.TotalTonnage <= freeTonnage)
                        {
                            maxEngine = engine;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (maxEngine != null)
                    {
                        break;
                    }
                }

                if (maxEngine != null)
                {
                    Control.mod.Logger.LogDebug($" maxEngine={maxEngine.CoreDef} freeTonnage={freeTonnage}");
                    {
                        var dummyCore = builder.Inventory.FirstOrDefault(r => r.ComponentDefID == AutoFixerFeature.settings.MechDefCoreDummy);
                        if (dummyCore != null)
                        {
                            builder.Remove(dummyCore);
                        }
                    }

                    // add engine
                    builder.Add(maxEngine.CoreDef.Def, ChassisLocations.CenterTorso, true);
                }
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

            // add free heatsinks
            if (maxEngine != null) {
                var maxFree = maxEngine.HeatSinkExternalFreeMaxCount;
                var current = 0; //we assume exiting heatsinks on the mech are additional and not free
                for (var i = current; i < maxFree; i++)
                {
                    if (!builder.Add(engineHeatSinkDef.Def))
                    {
                        break;
                    }
                }
            }
            
            // find any overused location
            // TODO find out why locational dynamic slots are ignored
            if (builder.HasOveruseAtAnyLocation())
            {
                // heatsinks, upgrades
                var itemsToBeReordered = builder.Inventory
                    .Where(IsReorderable)
                    .OrderBy(c => MechDefBuilder.LocationCount(c.Def.AllowedLocations))
                    .ThenByDescending(c => c.Def.InventorySize)
                    .ThenByDescending(c =>
                    {
                        switch (c.ComponentDefType)
                        {
                            case ComponentType.Upgrade:
                                return 2;
                            case ComponentType.AmmunitionBox:
                                return 1;
                            default:
                                return 0;
                        }
                    })
                    .ToList();

                // remove all items that can be reordered: heatsinks, upgrades
                foreach (var item in itemsToBeReordered)
                {
                    builder.Remove(item);
                }

                // then add most restricting, and then largest items first (probably double head sinks)
                foreach (var item in itemsToBeReordered)
                {
                    // couldn't add everything
                    if (!builder.Add(item.Def))
                    {
                        Control.mod.Logger.LogError($"Couldn't re-add items through reordering for mechDef={mechDef.Description.Id} chassisDef={mechDef.Chassis.Description.Id}");
                        return;
                    }
                }
            }

            mechDef.SetInventory(builder.Inventory.OrderBy(element => element, new OrderComparer()).ToArray());

            //{
            //    var freeTonnage = CalcFreeTonnage();
            //    if (freeTonnage > 0)
            //    {
            //        // TODO add armor for each location with free tonnage left
            //    }
            //}
        }

        private class OrderComparer : IComparer<MechComponentRef>
        {
            private readonly SorterComparer comparer = new SorterComparer();
            public int Compare(MechComponentRef x, MechComponentRef y)
            {
                return comparer.Compare(x?.Def, y?.Def);
            }
        }

        private static bool IsReorderable(MechComponentRef c)
        {
            var def = c?.Def;
            
            if (def == null)
            {
                return false;
            }

            if (c.IsFixed)
            {
                return false;
            }
            
            if (!(def.ComponentType >= ComponentType.AmmunitionBox && def.ComponentType <= ComponentType.Upgrade))
            {
                return false;
            }

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
    }
}