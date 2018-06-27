using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.Data;
using UnityEngine;

namespace MechEngineer
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

                mechDef.Refresh();

                float originalTotalTonnage = 0, maxValue = 0;
                MechStatisticsRules.CalculateTonnage(mechDef, ref originalTotalTonnage, ref maxValue);

                Cockpit.AddCockpitIfPossible(mechDef);
                Gyro.AddGyroIfPossible(mechDef);
                EngineMisc.AddEngineIfPossible(mechDef, originalTotalTonnage);
            }
        }
    }
}