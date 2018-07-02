using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;

namespace MechEngineer
{
    internal class EngineCoreRefHandler : IAutoFixMechDef, IValidateMech
    {
        internal static EngineCoreRefHandler Shared = new EngineCoreRefHandler();

        public void AutoFixMechDef(MechDef mechDef, float originalTotalTonnage)
        {
            if (!Control.settings.AutoFixMechDefEngine)
            {
                return;
            }

            if (mechDef.Inventory.Any(c => c.Def is EngineCoreDef))
            {
                return;
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

            var maxEngine = (EngineCoreDef) null;

            //Control.mod.Logger.LogDebug("C maxEngineTonnage=" + maxEngineTonnage);

            foreach (var keyvalue in mechDef.DataManager.HeatSinkDefs)
            {
                var heatSinkDef = keyvalue.Value;

                if (heatSinkDef.Tonnage > freeTonnage)
                {
                    continue;
                }

                if (!(heatSinkDef is EngineCoreDef engineDef))
                {
                    continue;
                }

                if (maxEngine != null && maxEngine.Rating >= engineDef.Rating)
                {
                    continue;
                }

                maxEngine = engineDef;
            }

            //Control.mod.Logger.LogDebug("D maxEngine=" + maxEngine);

            if (maxEngine == null)
            {
                return;
            }

            var componentRefs = new List<MechComponentRef>(mechDef.Inventory);

            {
                // remove superfluous jump jets
                var maxJetCount = Control.calc.CalcJumpJetCount(maxEngine, mechDef.Chassis.Tonnage);
                var jumpJetList = componentRefs.Where(x => x.ComponentDefType == ComponentType.JumpJet).ToList();
                for (var i = 0; i < jumpJetList.Count - maxJetCount; i++)
                {
                    componentRefs.Remove(jumpJetList[i]);
                }
            }

            {
                // add engine core
                var nonStandardHeatSinkDef = componentRefs.Select(c => c.Def as EngineHeatSinkDef).FirstOrDefault(c => c != null && !c.IsSingle());

                var simGameUID = nonStandardHeatSinkDef != null ? "/ihstype=" + nonStandardHeatSinkDef.Description.Id : null;

                var componentRef = new MechComponentRef(maxEngine.Description.Id, simGameUID, maxEngine.ComponentType, ChassisLocations.CenterTorso);
                componentRefs.Add(componentRef);
            }

            {
                // add standard shielding
                var componentRef = new MechComponentRef(Control.settings.AutoFixMechDefEngineTypeDef, null, ComponentType.HeatSink, ChassisLocations.CenterTorso);
                componentRefs.Add(componentRef);
            }

            mechDef.SetInventory(componentRefs.ToArray());
        }

        public void ValidateMech(MechDef mechDef, Dictionary<MechValidationType, List<string>> errorMessages)
        {
            if (Control.settings.AllowMixingHeatSinkTypes)
            {
                return;
            }

            var set = new HashSet<string>();
            foreach (var componentRef in mechDef.Inventory)
            {
                if (componentRef?.Def is EngineHeatSinkDef componentDef)
                {
                    set.Add(componentDef.HSCategory);
                }
                else if (componentRef?.Def is EngineCoreDef)
                {
                    var engineRef = componentRef.GetEngineCoreRef();
                    set.Add(engineRef.HeatSinkDef.HSCategory);
                }
                else
                {
                    continue;
                }

                if (set.Count > 1)
                {
                    errorMessages[MechValidationType.InvalidInventorySlots].Add("MIXED HEATSINKS: Heat Sink types cannot be mixed");
                    return;
                }
            }
        }
    }
}