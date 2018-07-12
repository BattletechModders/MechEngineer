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

            var maxEngine = (Engine) null;

            //Control.mod.Logger.LogDebug("C maxEngineTonnage=" + maxEngineTonnage);

            var heatSinks = mechDef.Inventory
                .Where(r => r.Def.GetComponent<EngineHeatSink>() != null)
                .ToList();

            var standardEngineType = mechDef.DataManager.HeatSinkDefs.Get(Control.settings.AutoFixMechDefEngineTypeDef).GetComponent<EngineType>();
            var standardHeatSinkDef = mechDef.DataManager.GetDefaultEngineHeatSinkDef();

            var engineHeatSinkdef = mechDef.Inventory
                .Select(r => r.Def.GetComponent<EngineHeatSink>())
                .FirstOrDefault(d => d != null && d != standardHeatSinkDef) ?? standardHeatSinkDef;

            foreach (var keyvalue in mechDef.DataManager.HeatSinkDefs)
            {
                var heatSinkDef = keyvalue.Value;

                var coreDef = heatSinkDef.GetComponent<EngineCoreDef>();
                if (coreDef == null)
                {
                    continue;
                }

                var coreRef = new EngineCoreRef(engineHeatSinkdef, coreDef);
                var engine = new Engine(coreRef, standardEngineType, Enumerable.Empty<MechComponentRef>());
                if (engine.TotalTonnage > freeTonnage)
                {
                    continue;
                }

                if (maxEngine != null && maxEngine.CoreDef.Rating >= coreDef.Rating)
                {
                    continue;
                }

                maxEngine = engine;
            }

            if (maxEngine == null)
            {
                return;
            }

            Control.mod.Logger.LogDebug("D maxEngine=" + maxEngine.CoreDef);

            var componentRefs = new List<MechComponentRef>(mechDef.Inventory);

            {
                // remove superfluous jump jets
                var maxJetCount = maxEngine.CoreDef.GetMovement(mechDef.Chassis.Tonnage).JumpJetCount;
                var jumpJetList = componentRefs.Where(x => x.ComponentDefType == ComponentType.JumpJet).ToList();
                for (var i = 0; i < jumpJetList.Count - maxJetCount; i++)
                {
                    componentRefs.Remove(jumpJetList[i]);
                }
            }

            var builder = new MechDefBuilder(mechDef.Chassis, componentRefs);

            // add engine
            builder.Add(
                maxEngine.CoreDef.Def,
                ChassisLocations.CenterTorso,
                engineHeatSinkdef != standardHeatSinkDef ? "/ihstype=" + engineHeatSinkdef.Def.Description.Id : null
            );

            // add standard shielding
            builder.Add(standardEngineType.Def, ChassisLocations.CenterTorso);

            // add free heatsinks
            {
                var count = 0;
                while (count < maxEngine.CoreDef.MaxFreeExternalHeatSinks)
                {
                    if (builder.Add(engineHeatSinkdef.Def))
                    {
                        count++;
                    }
                    else
                    {
                        break;
                    }
                }
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