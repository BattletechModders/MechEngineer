using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using CustomComponents;
using UnityEngine;

namespace MechEngineer
{
    internal class EngineHandler : IAutoFixMechDef
    {
        internal static EngineHandler Shared = new EngineHandler();

        public void AutoFixMechDef(MechDef mechDef)
        {
            //DumpAllAsTable();
            if (mechDef.Inventory.Any(c => c.Def.GetComponent<EngineCoreDef>() != null))
            {
                return;
            }

            Control.mod.Logger.LogDebug($"AutoFixing {mechDef.Chassis.Description.Id}");

            var builder = new MechDefBuilder(mechDef.Chassis, mechDef.Inventory.ToList());
            var standardHeatSinkDef = mechDef.DataManager.GetDefaultEngineHeatSinkDef();
            var engineHeatSinkDef = builder.Inventory
                                        .Select(r => r.Def.GetComponent<CoolingDef>())
                                        .Where(d => d != null)
                                        .Select(d => mechDef.DataManager.HeatSinkDefs.Get(d.HeatSinkDefId))
                                        .Where(d => d != null)
                                        .Select(d => d.GetComponent<EngineHeatSinkDef>())
                                        .FirstOrDefault() ?? standardHeatSinkDef;

            float freeTonnage;
            {
                float currentTotalTonnage = 0, maxValue = 0;
                MechStatisticsRules.CalculateTonnage(mechDef, ref currentTotalTonnage, ref maxValue);
                var maxFreeTonnage = mechDef.Chassis.Tonnage - currentTotalTonnage;

                var initialTonnage = mechDef.Chassis.InitialTonnage;
                var originalInitialTonnage = ChassisHandler.GetOriginalTonnage(mechDef.Chassis);
                if (originalInitialTonnage.HasValue) // either use the freed up tonnage from the initial tonnage fix
                {
                    freeTonnage = originalInitialTonnage.Value - initialTonnage;
                    freeTonnage = Mathf.Min(freeTonnage, maxFreeTonnage); // if the original was overweight, make sure not to stay overweight
                }
                else // or use up available total tonnage
                {
                    Control.mod.Logger.LogWarning("Couldn't find original initial tonnage");
                    return;
                }

                Control.mod.Logger.LogDebug($"AutoFix {mechDef.Chassis.Description.Id}" +
                                            $" InitialTonnage={initialTonnage}" +
                                            $" originalInitialTonnage={originalInitialTonnage}" +
                                            $" currentTotalTonnage={currentTotalTonnage}" +
                                            $" freeTonnage={freeTonnage}" +
                                            $" maxFreeTonnage={maxFreeTonnage}");
            }

            //Control.mod.Logger.LogDebug("C maxEngineTonnage=" + maxEngineTonnage);
            var standardWeights = new Weights(); // use default gyro and weights
            var standardHeatBlock = mechDef.DataManager.HeatSinkDefs.Get(Control.settings.AutoFixMechDefHeatBlockDef).GetComponent<EngineHeatBlockDef>();
            var standardCooling = mechDef.DataManager.HeatSinkDefs.Get(Control.settings.AutoFixMechDefCoolingDef).GetComponent<CoolingDef>();

            var engineCoreDefs = mechDef.DataManager.HeatSinkDefs
                .Select(hs => hs.Value)
                .Select(hs => hs.GetComponent<EngineCoreDef>())
                .Where(c => c != null)
                .OrderByDescending(x => x.Rating);

            Engine maxEngine = null;

            var heatSinks = builder.Inventory.Where(x => x.ComponentDefType == ComponentType.HeatSink && x.Def.Is<EngineHeatSinkDef>()).ToList();
            var jumpJetList = builder.Inventory.Where(x => x.ComponentDefType == ComponentType.JumpJet).ToList();

            foreach (var coreDef in engineCoreDefs)
            {
                var engine = new Engine(standardCooling, standardHeatBlock, coreDef, standardWeights, heatSinks);

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

                if (engine.TotalTonnage > freeTonnage)
                {
                    continue;
                }

                maxEngine = engine;
                break;
            }

            if (maxEngine == null)
            {
                return;
            }

            Control.mod.Logger.LogDebug($"D maxEngine={maxEngine.CoreDef} freeTonnage={freeTonnage}");
            {
                var dummyCore = builder.Inventory.FirstOrDefault(r => r.ComponentDefID == Control.settings.AutoFixMechDefCoreDummy);
                if (dummyCore != null)
                {
                    builder.Remove(dummyCore);
                }
            }

            // add engine
            builder.Add(
                maxEngine.CoreDef.Def,
                ChassisLocations.CenterTorso
            );

            if (!Control.settings.AllowMixingHeatSinkTypes)
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
            {
                //var maxFree = maxEngine.CoreDef.ExternalHeatSinksFreeMaxCount;
                //var current = maxEngine.ExternalHeatSinkCount;
                var maxFree = maxEngine.CoreDef.ExternalHeatSinksFreeMaxCount;
                var current = 0;
                for (var i = current; i < maxFree; i++)
                {
                    if (!builder.Add(engineHeatSinkDef.Def))
                    {
                        break;
                    }
                }
            }
            
            // find any overused location
            if (builder.HasOveruse())
            {
                // heatsinks, upgrades
                var itemsToBeReordered = mechDef.Inventory
                    .Where(c => IsReorderable(c.Def))
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
                        return;
                    }
                }
            }

            mechDef.SetInventory(builder.Inventory.ToArray());
        }

        private static bool IsReorderable(MechComponentDef def)
        {
            if (!(def.ComponentType >= ComponentType.AmmunitionBox && def.ComponentType <= ComponentType.Upgrade))
            {
                return false;
            }

            if (MechDefBuilder.LocationCount(def.AllowedLocations) == 1)
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