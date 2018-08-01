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
        private static MechDef mechDef;

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return instructions.MethodReplacer(
                AccessTools.Method(typeof(HeatSinkDef), "get_DissipationCapacity"),
                AccessTools.Method(typeof(MechStatisticsRules_CalculateHeatEfficiencyStat_Patch), "OverrideDissipationCapacity")
            );
        }

        public static void Prefix(MechDef def)
        {
            try
            {
                mechDef = def;
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }

        public static void Postfix()
        {
            try
            {
                mechDef = null;
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }

        public static float OverrideDissipationCapacity(this HeatSinkDef @this)
        {
            try
            {
                if (mechDef != null && @this.Is<EngineCoreDef>())
                {
                    return EngineHeat.GetEngineHeatDissipation(mechDef.Inventory);
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