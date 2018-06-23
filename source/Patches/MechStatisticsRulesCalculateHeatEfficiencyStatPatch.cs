using System;
using System.Collections.Generic;
using BattleTech;
using Harmony;

namespace MechEngineMod
{
    [HarmonyPatch(typeof(MechStatisticsRules), "CalculateHeatEfficiencyStat")]
    public static class MechStatisticsRulesCalculateHeatEfficiencyStatPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return instructions.MethodReplacer(
                AccessTools.Method(typeof(HeatSinkDef), "get_DissipationCapacity"),
                AccessTools.Method(typeof(MechStatisticsRulesCalculateHeatEfficiencyStatPatch), "OverrideDissipationCapacity")
            );
        }

        private static MechDef def;

        public static void Prefix(MechDef mechDef)
        {
            def = mechDef;
        }

        public static void Postfix()
        {
            def = null;
        }

        public static float OverrideDissipationCapacity(this HeatSinkDef @this)
        {
            try
            {
                if (@this.IsMainEngine())
                {
                    return EngineHeat.GetEngineHeatDissipation(def.Inventory);
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
            return @this.DissipationCapacity;
        }
    }
}