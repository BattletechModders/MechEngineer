using System;
using System.Linq;
using BattleTech;
using Harmony;

namespace MechEngineMod
{
    [HarmonyPatch(typeof(MechStatisticsRules), "CalculateTonnage")]
    public static class StructArmorMechStatisticsRulesPatch
    {
        // endo-steel and ferros-fibrous calculations for validation
        public static void Postfix(MechDef mechDef, ref float currentValue, ref float maxValue)
        {
            try
            {
                currentValue -= WeightSavingsIfEndoSteel(mechDef);
                currentValue -= WeightSavingsIfFerrosFibrous(mechDef);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }

        internal static float WeightSavingsIfEndoSteel(MechDef mechDef)
        {
            if (!mechDef.Inventory.Any(x => x.Def.IsEndoSteel()))
            {
                return 0;
            }

            return mechDef.Chassis.Tonnage / 10f * Control.settings.EndoSteelStructureWeightSavingsFactor;
        }

        internal static float WeightSavingsIfFerrosFibrous(MechDef mechDef)
        {
            if (!mechDef.Inventory.Any(x => x.Def.IsFerrosFibrous()))
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
            return armorWeight * Control.settings.FerrosFibrousArmorWeightSavingsFactor;
        }
    }
}