using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;

namespace MechEngineMod
{
    internal static class Armor
    {
        internal static void ValidationRulesCheck(MechDef mechDef, ref Dictionary<MechValidationType, List<string>> errorMessages)
        {
            var calculator = new ArmorWeightSavingCalculator(mechDef);
            if (calculator.ErrorMessage != null)
            {
                errorMessages[MechValidationType.InvalidInventorySlots].Add(calculator.ErrorMessage);
            }
        }

        internal static bool ProcessWeaponHit(MechComponent mechComponent, WeaponHitInfo hitInfo, ComponentDamageLevel damageLevel, bool applyEffects)
        {
            if (mechComponent.componentDef.IsArmor())
            {
                return false;
            }

            return true;
        }

        internal static float WeightSavings(MechDef mechDef)
        {
            var calculator = new ArmorWeightSavingCalculator(mechDef);
            return calculator.WeightSavings;
        }
        
        internal static void AdjustTooltip(TooltipPrefab_EquipmentAdapter tooltip, MechLabPanel panel, MechComponentDef mechComponentDef)
        {
            if (!mechComponentDef.IsArmor())
            {
                return;
            }

            var calculator = new ArmorWeightSavingCalculator(panel.activeMechDef);
            var tonnage = calculator.WeightSavings;

            tooltip.bonusesText.text = string.Format("- {0} ton,  {1} / {2}", tonnage, calculator.Count, calculator.RequiredCount);
            tooltip.bonusesText.SetAllDirty();
        }

        internal class ArmorWeightSavingCalculator : WeightSavingSlotCalculator
        {
            internal float WeightSavings { get; private set; }

            internal ArmorWeightSavingCalculator(MechDef mechDef) : base(mechDef.Inventory.Where(c => c.Def.IsArmor()).ToList(), Control.settings.ArmorTypes)
            {
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
                var tonnage = num / (UnityGameInstance.BattleTechGame.MechStatisticsConstants.ARMOR_PER_TENTH_TON * 10f);

                WeightSavings = Count == 0 ? 0 : WeightSavingForTonnage(tonnage);
            }
        }
    }
}