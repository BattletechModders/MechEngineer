using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.Data;

namespace MechEngineMod
{
    internal static class MechDefMods
    {
        // call if mechdef is first time retrieved
        // prepare all engine defs beforehand - RequestDataManagerResources()
        internal static void PostProcessAfterLoading(DataManager dataManager)
        {
            foreach (var keyValuePair in dataManager.MechDefs)
            {
                var mechDef = keyValuePair.Value;

                if (Control.settings.AutoFixMechDefSkip.Contains(mechDef.Description.Id))
                {
                    continue;
                }

                AddGyroIfPossible(mechDef);
                AddEngineIfPossible(mechDef);
            }
        }

        internal static void AddGyroIfPossible(MechDef mechDef)
        {
            if (!Control.settings.AutoFixMechDefGyro)
            {
                return;
            }

            if (mechDef.Inventory.Any(x => x.Def != null && x.Def.IsCenterTorsoUpgrade()))
            {
                return;
            }

            var componentRefs = new List<MechComponentRef>(mechDef.Inventory);

            var componentRef = new MechComponentRef("Gear_Gyro_Generic_Standard", null, ComponentType.Upgrade, ChassisLocations.CenterTorso);
            componentRefs.Add(componentRef);

            mechDef.SetInventory(componentRefs.ToArray());
        }
        internal static void AddEngineIfPossible(MechDef mechDef)
        {
            if (!Control.settings.AutoFixMechDefEngine)
            {
                return;
            }

            //Control.mod.Logger.LogDebug("A Id=" + mechDef.Description.Id);

            mechDef.Refresh();

            //Control.mod.Logger.LogDebug("B DataManager=" + mechDef.DataManager);

            if (mechDef.Inventory.Any(x => x.GetEngineRef() != null))
            {
                return;
            }

            float currentValue = 0, maxValue = 0;
            MechStatisticsRules.CalculateTonnage(mechDef, ref currentValue, ref maxValue);

            var tonnage = mechDef.Chassis.Tonnage;
            var maxEngineTonnage = tonnage - currentValue;
            var maxEngine = (EngineDef) null;

            //Control.mod.Logger.LogDebug("C maxEngineTonnage=" + maxEngineTonnage);

            foreach (var keyvalue in mechDef.DataManager.HeatSinkDefs)
            {
                var heatSinkDef = keyvalue.Value;

                if (heatSinkDef.Tonnage > maxEngineTonnage)
                {
                    continue;
                }

                var engineDef = heatSinkDef.GetEngineDef();
                if (engineDef == null)
                {
                    continue;
                }

                if (engineDef.Type != EngineDef.EngineType.Std)
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

            { // remove superfluous jump jets
                var maxJetCount = Control.calc.CalcJumpJetCount(maxEngine, tonnage);
                var jumpJetList = componentRefs.Where(x => x.ComponentDefType == ComponentType.JumpJet).ToList();
                for (var i = 0; i < jumpJetList.Count - maxJetCount; i++)
                {
                    componentRefs.Remove(jumpJetList[i]);
                }
            }

            { // add engine
                var componentRef = new MechComponentRef(maxEngine.Def.Description.Id, null, maxEngine.Def.ComponentType, ChassisLocations.CenterTorso);
                componentRefs.Add(componentRef);
            }

            mechDef.SetInventory(componentRefs.ToArray());
        }
    }
}