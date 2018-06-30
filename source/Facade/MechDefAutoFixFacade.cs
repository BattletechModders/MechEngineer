using System.Linq;
using BattleTech;
using BattleTech.Data;

namespace MechEngineer
{
    internal static class MechDefAutoFixFacade
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

                CockpitHandler.Shared.AutoFixMechDef(mechDef, originalTotalTonnage);
                GyroHandler.Shared.AutoFixMechDef(mechDef, originalTotalTonnage);
                EngineCoreRefHandler.Shared.AutoFixMechDef(mechDef, originalTotalTonnage);
            }
        }
    }
}