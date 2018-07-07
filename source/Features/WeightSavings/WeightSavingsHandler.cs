using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using CustomComponents;

namespace MechEngineer
{
    internal class WeightSavingsHandler : ITonnageChanges, IAdjustTooltip
    {
        public static readonly WeightSavingsHandler Shared = new WeightSavingsHandler();

        public void AdjustTooltip(TooltipPrefab_EquipmentAdapter tooltip, MechLabPanel panel, MechComponentDef mechComponentDef)
        {
            var weightSavings = mechComponentDef.GetComponent<WeightSavings>();
            if (weightSavings == null)
            {
                return;
            }
            
            var mechDef = panel.activeMechDef;
            var tonnageSaved = CalculateWeightSavings(weightSavings, mechDef);

            tooltip.bonusesText.text = $"- {tonnageSaved} ton, - {weightSavings.RequiredSlots} slots";
            tooltip.bonusesText.SetAllDirty();
        }

        public float TonnageChanges(MechDef mechDef)
        {
            var tonnageSaved = 0f;
            foreach (var savings in mechDef.Inventory.Select(r => r.Def.GetComponent<WeightSavings>()).Where(w => w != null))
            {
                tonnageSaved += CalculateWeightSavings(savings, mechDef);
            }

            return -tonnageSaved;
        }

        private static float CalculateWeightSavings(WeightSavings savings, MechDef mechDef)
        {
            return CalculateArmorWeightSavings(savings, mechDef) + CalculateStructureWeightSavings(savings, mechDef);
        }

        private static float CalculateArmorWeightSavings(WeightSavings savings, MechDef mechDef)
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

            return tonnage * savings.ArmorWeightSavingsFactor;
        }

        private static float CalculateStructureWeightSavings(WeightSavings savings, MechDef mechDef)
        {
            var tonnage = mechDef.Chassis.Tonnage / 10f;

            return tonnage * savings.StructureWeightSavingsFactor;
        }
    }
}