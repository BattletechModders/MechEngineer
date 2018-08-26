using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using CustomComponents;
using UnityEngine;

namespace MechEngineer
{
    internal class EngineCoreRefHandler : IAutoFixMechDef, IAdjustSlotElement
    {
        internal static EngineCoreRefHandler Shared = new EngineCoreRefHandler();

        private bool alreadyDumped = false;
        private void DumpAllAsTable()
        {
            if (alreadyDumped)
            {
                return;
            }

            alreadyDumped = true;
            var cores = UnityGameInstance.BattleTechGame.DataManager.HeatSinkDefs
                .Select(hs => hs.Value)
                .Select(hs => hs.GetComponent<EngineCoreDef>())
                .Where(c => c != null)
                .OrderBy(x => x.Rating);

            var weights = UnityGameInstance.BattleTechGame.DataManager.HeatSinkDefs
                .Select(hs => hs.Value)
                .Select(hs => hs.GetComponent<Weights>())
                .Where(c => c != null && !Mathf.Approximately(c.EngineFactor, 1))
                .ToList();

            var standardHeatSinkDef = UnityGameInstance.BattleTechGame.DataManager.GetDefaultEngineHeatSinkDef();

            foreach (var coreDef in cores)
            {
                foreach (var weight in weights)
                {
                    var coreRef = new EngineCoreRef(standardHeatSinkDef, coreDef);
                    var engine = new Engine(coreRef, weight, Enumerable.Empty<MechComponentRef>());

                    var coreName = coreDef.Def.Description.UIName;
                    var weightName = weight.Def.Description.UIName;
                    var engineTonnage = engine.EngineTonnage;
                    Control.mod.Logger.LogDebug($"{coreName} {weightName} {engineTonnage}");
                }
            }
        }

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
                                        .Select(r => r.Def.GetComponent<EngineHeatSink>())
                                        .FirstOrDefault(d => d != null && d != standardHeatSinkDef) ?? standardHeatSinkDef;

            if (!Control.settings.AllowMixingHeatSinkTypes)
            {
                // remove incompatible heat sinks
                inventory.RemoveAll(r => r.Def.Is<EngineHeatSink>(out var engineHeatSink) && engineHeatSink.HSCategory != engineHeatSinkDef.HSCategory);
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
            var stanardEngineType = mechDef.DataManager.HeatSinkDefs.Get(Control.settings.AutoFixMechDefEngineTypeDef);

            var engineCoreDefs = mechDef.DataManager.HeatSinkDefs
                .Select(hs => hs.Value)
                .Select(hs => hs.GetComponent<EngineCoreDef>())
                .Where(c => c != null)
                .OrderByDescending(x => x.Rating);

            var maxEngine = engineCoreDefs
                .Select(coreDef => new EngineCoreRef(engineHeatSinkDef, coreDef))
                .Select(coreRef => new Engine(coreRef, standardWeights, Enumerable.Empty<MechComponentRef>()))
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
                ChassisLocations.CenterTorso,
                engineHeatSinkDef != standardHeatSinkDef ? "/ihstype=" + engineHeatSinkDef.Def.Description.Id : null
            );

            // add standard shielding
            builder.Add(stanardEngineType, ChassisLocations.CenterTorso);

            // add free heatsinks
            {
                var count = 0;
                while (count < maxEngine.CoreDef.MaxFreeExternalHeatSinks)
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

        public void AdjustSlotElement(MechLabItemSlotElement instance, MechLabPanel panel = null)
        {
            var engineRef = instance.ComponentRef.GetEngineCoreRef();
            if (engineRef == null)
            {
                return;
            }

            var adapter = new MechLabItemSlotElementAdapter(instance);
            adapter.bonusTextA.text = engineRef.BonusValueA;
            adapter.bonusTextB.text = engineRef.BonusValueB;
        }
    }
}