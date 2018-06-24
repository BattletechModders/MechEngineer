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

                Cockpit.AddCockpitIfPossible(mechDef);
                Gyro.AddGyroIfPossible(mechDef);
                EngineMisc.AddEngineIfPossible(mechDef);
            }
        }
    }
}