using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using BattleTech.UI.Tooltips;

namespace MechEngineMod
{
    internal static class FerrosFibrous
    {
        internal static void ValidationRulesCheck(MechDef mechDef, ref Dictionary<MechValidationType, List<string>> errorMessages)
        {
            var currentCount = mechDef.Inventory.Count(x => x.Def.IsFerrosFibrous());
            var exactCount = Control.settings.FerrosFibrousRequiredCriticals;
            if (currentCount > 0 && (Control.settings.FerroFibrousRequireAllSlots ? currentCount != exactCount : currentCount <= exactCount))
            {
                errorMessages[MechValidationType.InvalidInventorySlots].Add(String.Format("FERROS-FIBROUS: Critical slots count does not match ({0} instead of {1})", currentCount, exactCount));
            }
        }

        internal static float WeightSavings(MechDef mechDef)
        {
            int count;
            return WeightSavings(mechDef, out count);
        }

        internal static float WeightSavings(MechDef mechDef, out int count)
        {
            count = mechDef.Inventory.Count(x => x.Def.IsFerrosFibrous());
            if (count == 0)
            {
                return 0;
            }

            var num = 0f;
            num += mechDef.Head.AssignedArmor;
            num += mechDef.CenterTorso.AssignedArmor;
            num += mechDef.CenterTorso.AssignedRearArmor;
            num += mechDef.LeftTorso.AssignedArmor;
            num += mechDef.LeftTorso.AssignedRearArmor;
            num += mechDef.RightTorso.AssignedArmor;
            num += mechDef.RightTorso.AssignedRearArmor;
            num += mechDef.LeftArm.AssignedArmor;
            num += mechDef.RightArm.AssignedArmor;
            num += mechDef.LeftLeg.AssignedArmor;
            num += mechDef.RightLeg.AssignedArmor;
            var armorWeight = num / (UnityGameInstance.BattleTechGame.MechStatisticsConstants.ARMOR_PER_TENTH_TON * 10f);

            var partialFactor = Control.settings.FerroFibrousRequireAllSlots ? 1.0f : count / (float)Control.settings.FerrosFibrousRequiredCriticals;
            var savings = armorWeight * partialFactor * Control.settings.FerrosFibrousArmorWeightSavingsFactor;
            return savings.RoundToHalf();
        }

        internal static void AdjustTooltip(TooltipPrefab_Equipment tooltip, MechLabPanel panel, MechComponentDef mechComponentDef)
        {
            if (!mechComponentDef.IsFerrosFibrous())
            {
                return;
            }

            int count;
            var tonnage = WeightSavings(panel.activeMechDef, out count);

            tooltip.bonusesText.text = string.Format("- {0} ton,  {1} / {2}", tonnage, count, Control.settings.FerrosFibrousRequiredCriticals);
        }
    }
}