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

        public void AutoFixMechDef(MechDef mechDef, float originalTotalTonnage)
        {
            //DumpAllAsTable();

            if (mechDef.Inventory.Any(c => c.Def.GetComponent<EngineCoreDef>() != null))
            {
                return;
            }

            var inventory = new List<MechComponentRef>(mechDef.Inventory);
            var standardHeatSinkDef = mechDef.DataManager.GetDefaultEngineHeatSinkDef();
            var engineHeatSinkDef = inventory
                                        .Select(r => r.Def.GetComponent<EngineHeatSinkDef>())
                                        .FirstOrDefault(d => d != null && d != standardHeatSinkDef) ?? standardHeatSinkDef;

            if (!Control.settings.AllowMixingHeatSinkTypes)
            {
                // remove incompatible heat sinks
                inventory.RemoveAll(r => r.Def.Is<EngineHeatSinkDef>(out var engineHeatSink) && engineHeatSink.HSCategory != engineHeatSinkDef.HSCategory);
            }

            float freeTonnage;
            {
                float currentTotalTonnage = 0, maxValue = 0;
                MechStatisticsRules.CalculateTonnage(mechDef, ref currentTotalTonnage, ref maxValue);

                var originalInitialTonnage = ChassisHandler.GetOriginalTonnage(mechDef.Chassis);
                if (originalInitialTonnage.HasValue) // either use the freed up tonnage from the initial tonnage fix
                {
                    freeTonnage = originalInitialTonnage.Value - mechDef.Chassis.InitialTonnage;
                    freeTonnage -= currentTotalTonnage - originalTotalTonnage;
                }

                else // or use up available total tonnage
                {
                    freeTonnage = mechDef.Chassis.Tonnage - currentTotalTonnage;
                }
            }

            //Control.mod.Logger.LogDebug("C maxEngineTonnage=" + maxEngineTonnage);
            var standardWeights = new Weights(); // use default gyro and weights
            var standardEngineType = mechDef.DataManager.HeatSinkDefs.Get(Control.settings.AutoFixMechDefEngineTypeDef);
            var standardHeatBlock = mechDef.DataManager.HeatSinkDefs.Get(Control.settings.AutoFixMechDefHeatBlockDef).GetComponent<EngineHeatBlockDef>();
            // TODO autoselect correct type
            var standardCooling = mechDef.DataManager.HeatSinkDefs.Get(Control.settings.AutoFixMechDefCoolingDef).GetComponent<CoolingDef>();

            var engineCoreDefs = mechDef.DataManager.HeatSinkDefs
                .Select(hs => hs.Value)
                .Select(hs => hs.GetComponent<EngineCoreDef>())
                .Where(c => c != null)
                .OrderByDescending(x => x.Rating);

            var maxEngine = engineCoreDefs
                .Select(coreDef => new Engine(standardCooling, standardHeatBlock, coreDef, standardWeights, new List<MechComponentRef>()))
                .FirstOrDefault(engine => !(engine.TotalTonnage > freeTonnage));

            if (maxEngine == null)
            {
                return;
            }

            // Control.mod.Logger.LogDebug("D maxEngine=" + maxEngine.CoreDef);

            {
                // remove superfluous jump jets
                var maxJetCount = maxEngine.CoreDef.GetMovement(mechDef.Chassis.Tonnage).JumpJetCount;
                var jumpJetList = inventory.Where(x => x.ComponentDefType == ComponentType.JumpJet).ToList();
                for (var i = 0; i < jumpJetList.Count - maxJetCount; i++)
                {
                    inventory.Remove(jumpJetList[i]);
                }
            }

            var builder = new MechDefBuilder(mechDef.Chassis, inventory);

            // add engine
            builder.Add(
                maxEngine.CoreDef.Def,
                ChassisLocations.CenterTorso
            );

            // add standard shielding
            builder.Add(standardEngineType, ChassisLocations.CenterTorso);

            // add standard cooling
            builder.Add(standardCooling.Def, ChassisLocations.CenterTorso);

            // add standard heat block
            builder.Add(standardHeatBlock.Def, ChassisLocations.CenterTorso);

            // add free heatsinks
            {
                var count = 0;
                while (count < maxEngine.CoreDef.ExternalHeatSinksFreeMaxCount)
                {
                    if (builder.Add(engineHeatSinkDef.Def))
                    {
                        count++;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            mechDef.SetInventory(inventory.ToArray());
        }
    }
}