using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using BattleTech.UI.Tooltips;

namespace MechEngineMod
{
    internal static class Structure
    {
        internal static void ValidationRulesCheck(MechDef mechDef, ref Dictionary<MechValidationType, List<string>> errorMessages)
        {
            var calculator = new StructureWeightSavingCalculator(mechDef);
            if (calculator.ErrorMessage != null)
            {
                errorMessages[MechValidationType.InvalidInventorySlots].Add(calculator.ErrorMessage);
            }
        }

        internal static bool ProcessWeaponHit(MechComponent mechComponent, WeaponHitInfo hitInfo, ComponentDamageLevel damageLevel, bool applyEffects)
        {
            if (mechComponent.componentDef.IsStructure())
            {
                return false;
            }

            return true;
        }

        internal static float WeightSavings(MechDef mechDef)
        {
            var calculator = new StructureWeightSavingCalculator(mechDef);
            return calculator.WeightSavings;
        }
        
        internal static void AdjustTooltip(TooltipPrefab_Equipment tooltip, MechLabPanel panel, MechComponentDef mechComponentDef)
        {
            if (!mechComponentDef.IsStructure())
            {
                return;
            }

            var calculator = new StructureWeightSavingCalculator(panel.activeMechDef);
            var tonnage = calculator.WeightSavings;

            tooltip.bonusesText.text = string.Format("- {0} ton,  {1} / {2}", tonnage, calculator.Count, calculator.RequiredCount);
        }

        internal class StructureWeightSavingCalculator : WeightSavingSlotCalculator
        {
            internal float WeightSavings { get; private set; }

            internal StructureWeightSavingCalculator(MechDef mechDef) : base(mechDef.Inventory.Where(c => c.Def.IsStructure()).ToList(), Control.settings.StuctureTypes)
            {
                var tonnage = mechDef.Chassis.Tonnage / 10f;
                WeightSavings = Count == 0 ? 0 : WeightSavingForTonnage(tonnage);
            }
        }
    }
}