using System;
using System.Collections.Generic;
using BattleTech;
using CustomComponents;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(MechStatisticsRules), "CalculateHeatEfficiencyStat")]
    public static class MechStatisticsRules_CalculateHeatEfficiencyStat_Patch
    {
        private static MechDef def;

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return instructions.MethodReplacer(
                AccessTools.Method(typeof(HeatSinkDef), "get_DissipationCapacity"),
                AccessTools.Method(typeof(MechStatisticsRules_CalculateHeatEfficiencyStat_Patch), "OverrideDissipationCapacity")
            );
        }

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
                var engineCoreDef = @this.GetComponent<EngineCoreDef>();
                if (engineCoreDef != null)
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