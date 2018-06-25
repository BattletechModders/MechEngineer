using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;

namespace MechEngineer
{
    internal static class ArmorStructure
    {
        internal static void ValidationRulesCheck(MechDef mechDef, ref Dictionary<MechValidationType, List<string>> errorMessages)
        {
            var errors = new List<string>();

            {
                var weightSavings = CalculateArmorWeightSavings(mechDef);
                errors.AddRange(weightSavings.ErrorMessages);
            }
            
            {
                var weightSavings = CalculateStructureWeightSavings(mechDef);
                errors.AddRange(weightSavings.ErrorMessages);
            }

            errorMessages[MechValidationType.InvalidInventorySlots].AddRange(errors);
        }

        internal static bool ProcessWeaponHit(MechComponent mechComponent, WeaponHitInfo hitInfo, ComponentDamageLevel damageLevel, bool applyEffects)
        {
            if (mechComponent.componentDef.IsArmor() || mechComponent.componentDef.IsStructure())
            {
                return false;
            }

            return true;
        }

        internal static float TonnageSavings(MechDef mechDef)
        {
            float tonnageSaved = 0;

            {
                var weightSavings = CalculateArmorWeightSavings(mechDef);
                tonnageSaved += weightSavings.TonnageSaved;
            }
            
            {
                var weightSavings = CalculateStructureWeightSavings(mechDef);
                tonnageSaved += weightSavings.TonnageSaved;
            }

            return tonnageSaved;
        }
        
        internal static void AdjustTooltip(TooltipPrefab_EquipmentAdapter tooltip, MechLabPanel panel, MechComponentDef mechComponentDef)
        {
            var mechDef = panel.activeMechDef;
            WeightSavings savings;
            if (mechComponentDef.IsArmor())
            {
                savings = CalculateArmorWeightSavings(mechDef, mechComponentDef);
            }
            else if (mechComponentDef.IsStructure())
            {
                savings = CalculateStructureWeightSavings(mechDef, mechComponentDef);
            }
            else
            {
                return;
            }

            var tonnage = savings.TonnageSaved;

            tooltip.bonusesText.text = string.Format("- {0} ton,  {1} / {2}", tonnage, savings.Count, savings.RequiredCount);
            tooltip.bonusesText.SetAllDirty();
        }

        private static WeightSavings CalculateArmorWeightSavings(MechDef mechDef, MechComponentDef mechComponentDef = null)
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
            
            var slots = mechDef.Inventory.Select(c => c.Def).Where(c => c.IsArmor()).ToList();

            return WeightSavings.Create(tonnage, slots, Control.settings.ArmorTypes, mechComponentDef);
        }

        private static WeightSavings CalculateStructureWeightSavings(MechDef mechDef, MechComponentDef mechComponentDef = null)
        {
            var tonnage = mechDef.Chassis.Tonnage / 10f;
            
            var slots = mechDef.Inventory.Select(c => c.Def).Where(c => c.IsStructure()).ToList();

            return WeightSavings.Create(tonnage, slots, Control.settings.StructureTypes, mechComponentDef);
        }
    }
}