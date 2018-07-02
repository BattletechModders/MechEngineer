using System.Collections.Generic;
using System.Linq;
using BattleTech;

namespace MechEngineer
{
    internal class EngineCoreRefHandler : IAutoFixMechDef, IValidationRulesCheck
    {
        internal static EngineCoreRefHandler Shared = new EngineCoreRefHandler();

        public void AutoFixMechDef(MechDef mechDef, float originalTotalTonnage)
        {
            if (!Control.settings.AutoFixMechDefEngine)
            {
                return;
            }

            if (mechDef.Inventory.Any(c => c.Def.IsEngineCore()))
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

                var engineDef = heatSinkDef.GetEngineCoreDef();
                if (engineDef == null)
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
                var isDHS = componentRefs.Select(c => c.Def as HeatSinkDef).Where(c => c != null).Any(c => c.IsDouble());

                var simGameUID = isDHS ? "/ihstype=dhs" : null;

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
            if (Control.settings.AllowMixingDoubleAndSingleHeatSinks)
            {
                return;
            }

            bool hasSingle = false, hasDouble = false;
            foreach (var componentRef in mechDef.Inventory)
            {
                if (componentRef == null)
                {
                    continue;
                }

                var componentDef = componentRef.Def as HeatSinkDef;
                if (componentDef == null)
                {
                    continue;
                }

                if (componentDef.IsDouble())
                {
                    hasDouble = true;
                }
                else if (componentDef.IsSingle())
                {
                    hasSingle = true;
                }
                else if (componentDef.IsEngineCore())
                {
                    var engineRef = componentRef.GetEngineCoreRef();
                    if (engineRef == null)
                    {
                        continue;
                    }

                    if (engineRef.IsDHS)
                    {
                        hasDouble = true;
                    }
                    else
                    {
                        hasSingle = true;
                    }
                }

                if (hasSingle && hasDouble)
                {
                    errorMessages[MechValidationType.InvalidInventorySlots].Add("MIXED HEATSINKS: Standard and Double Heat Sinks cannot be mixed");
                    return;
                }
            }
        }
    }
}