using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using CustomComponents;

namespace MechEngineer
{
    internal class EngineCoreRefHandler : IAutoFixMechDef, IValidateMech, IModifySlotElement
    {
        internal static EngineCoreRefHandler Shared = new EngineCoreRefHandler();

        public void AutoFixMechDef(MechDef mechDef, float originalTotalTonnage)
        {
            if (mechDef.Inventory.Any(c => c.Def.GetComponent<EngineCoreDef>() != null))
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

            var externalHeatSinkTonnage = mechDef.Inventory
                .Select(r => r.Def)
                .Where(d => d.GetComponent<EngineHeatSink>() != null)
                .Sum(d => d.Tonnage);

            var type = mechDef.DataManager.HeatSinkDefs.Get(Control.settings.AutoFixMechDefEngineTypeDef).GetComponent<EngineType>();

            foreach (var keyvalue in mechDef.DataManager.HeatSinkDefs)
            {
                var heatSinkDef = keyvalue.Value;

                var coreDef = heatSinkDef.GetComponent<EngineCoreDef>();
                if (coreDef == null)
                {
                    continue;
                }

                var coreRef = new EngineCoreRef(coreDef);
                var engine = new Engine(coreRef, type, externalHeatSinkTonnage);
                if (engine.TotalTonnage > freeTonnage)
                {
                    continue;
                }

                if (maxEngine != null && maxEngine.Rating >= coreDef.Rating)
                {
                    continue;
                }

                maxEngine = coreDef;
            }

            if (maxEngine == null)
            {
                return;
            }

            // Control.mod.Logger.LogDebug("D maxEngine=" + maxEngine);

            var componentRefs = new List<MechComponentRef>(mechDef.Inventory);

            {
                // remove superfluous jump jets
                var maxJetCount = maxEngine.GetMovement(mechDef.Chassis.Tonnage).JumpJetCount;
                var jumpJetList = componentRefs.Where(x => x.ComponentDefType == ComponentType.JumpJet).ToList();
                for (var i = 0; i < jumpJetList.Count - maxJetCount; i++)
                {
                    componentRefs.Remove(jumpJetList[i]);
                }
            }

            {
                var standardHeatSinkDef = mechDef.DataManager.GetDefaultEngineHeatSinkDef();
                // add engine core
                var nonStandardHeatSinkDef = componentRefs
                    .Select(r => r.Def.GetComponent<EngineHeatSink>())
                    .FirstOrDefault(d => d != null && d != standardHeatSinkDef);

                var simGameUID = nonStandardHeatSinkDef != null ? "/ihstype=" + nonStandardHeatSinkDef.Def.Description.Id : null;

                var componentRef = new MechComponentRef(maxEngine.Def.Description.Id, simGameUID, maxEngine.Def.ComponentType, ChassisLocations.CenterTorso);
                componentRefs.Add(componentRef);
            }

            {
                // add standard shielding
                var componentRef = new MechComponentRef(type.Def.Description.Id, null, type.Def.ComponentType, ChassisLocations.CenterTorso);
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
                var def = componentRef?.Def;
                if (def == null)
                {
                    continue;
                }

                var componentDef = def.GetComponent<EngineHeatSink>();
                if (componentDef != null)
                {
                    set.Add(componentDef.HSCategory);
                }
                else if (def.GetComponent<EngineCoreDef>() != null)
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

        public void ModifySlotElement(MechLabItemSlotElement instance, MechLabPanel panel = null)
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