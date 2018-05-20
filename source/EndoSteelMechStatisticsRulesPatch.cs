using System;
using System.Linq;
using BattleTech;
using Harmony;

namespace MechEngineMod
{
    [HarmonyPatch(typeof(MechStatisticsRules), "CalculateTonnage")]
    public static class EndoSteelMechStatisticsRulesPatch
    {
        // endo-steel calculations for validation
        public static void Postfix(MechDef mechDef, ref float currentValue, ref float maxValue)
        {
            try
            {
                if (mechDef.Inventory.Any(x => x.Def.IsEndoSteel()))
                {
                    currentValue -= WeightSavingsIfEndoSteel(mechDef);
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }

        internal static float WeightSavingsIfEndoSteel(MechDef mechDef)
        {
            if (mechDef.Inventory.Any(x => x.Def.IsEndoSteel()))
            {
                return mechDef.Chassis.Tonnage / 10f / 2f;
            }

            return 0;
        }
    }
}