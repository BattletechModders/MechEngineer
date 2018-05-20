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
                    currentValue -= mechDef.Chassis.InitialTonnage / 2;
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}