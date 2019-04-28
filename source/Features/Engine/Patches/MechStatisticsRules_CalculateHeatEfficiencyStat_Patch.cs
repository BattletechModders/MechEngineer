using System;
using System.Collections.Generic;
using BattleTech;
using CustomComponents;
using Harmony;
using MechEngineer.Features.Engine.StaticHandler;

namespace MechEngineer.Features.Engine.Patches
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
            try
            {
                def = mechDef;
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
                def = null;
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
                if (def != null && @this.Is<EngineCoreDef>())
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