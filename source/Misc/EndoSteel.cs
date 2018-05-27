using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using BattleTech.UI.Tooltips;

namespace MechEngineMod
{
    internal static class EndoSteel
    {
        internal static void ValidationRulesCheck(MechDef mechDef, ref Dictionary<MechValidationType, List<string>> errorMessages)
        {
            var currentCount = mechDef.Inventory.Count(x => x.Def.IsEndoSteel());
            var exactCount = Control.settings.EndoSteelRequiredCriticals;
            if (currentCount > 0 && (Control.settings.EndoSteelRequireAllSlots ? currentCount != exactCount : currentCount <= exactCount))
            {
                errorMessages[MechValidationType.InvalidInventorySlots].Add(String.Format("ENDO-STEEL: Critical slots count does not match ({0} instead of {1})", currentCount, exactCount));
            }
        }

        internal static float WeightSavings(MechDef mechDef)
        {
            int count;
            return WeightSavings(mechDef, out count);
        }

        internal static float WeightSavings(MechDef mechDef, out int count)
        {
            count = mechDef.Inventory.Count(x => x.Def.IsEndoSteel());
            if (count == 0)
            {
                return 0;
            }

            var partialFactor = Control.settings.EndoSteelRequireAllSlots ? 1.0f : count / (float)Control.settings.EndoSteelRequiredCriticals;

            var savings = mechDef.Chassis.Tonnage / 10f * partialFactor * Control.settings.EndoSteelStructureWeightSavingsFactor;
            return savings.RoundToHalf();
        }
        
        internal static void AdjustTooltip(TooltipPrefab_Equipment tooltip, MechLabPanel panel, MechComponentDef mechComponentDef)
        {
            if (!mechComponentDef.IsEndoSteel())
            {
                return;
            }

            int count;
            var tonnage = WeightSavings(panel.activeMechDef, out count);

            tooltip.bonusesText.text = string.Format("- {0} ton,  {1} / {2}", tonnage, count, Control.settings.EndoSteelRequiredCriticals);
        }
    }
}