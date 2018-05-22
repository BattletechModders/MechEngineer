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
            var count = mechDef.Inventory.Count(x => x.Def.IsEndoSteel());
            if (count == 0)
            {
                return 0;
            }

            var partialFactor = Control.settings.EndoSteelRequireAllSlots ? 1.0f : count / (float)Control.settings.EndoSteelRequiredCriticals;

            return mechDef.Chassis.Tonnage / 10f * partialFactor * Control.settings.EndoSteelStructureWeightSavingsFactor;
        }

        internal static float WeightSavingsIfFerrosFibrous(MechDef mechDef)
        {
            var count = mechDef.Inventory.Count(x => x.Def.IsFerrosFibrous());
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
            return armorWeight * partialFactor * Control.settings.FerrosFibrousArmorWeightSavingsFactor;
        }
    }
}