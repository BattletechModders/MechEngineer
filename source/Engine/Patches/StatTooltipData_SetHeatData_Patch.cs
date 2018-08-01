using System;
using System.Collections.Generic;
using BattleTech;
using CustomComponents;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(StatTooltipData), "SetHeatData")]
    public static class StatTooltipData_SetHeatData_Patch
    {
        private static MechDef mechDef;

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return instructions.MethodReplacer(
                AccessTools.Method(typeof(HeatSinkDef), "get_DissipationCapacity"),
                AccessTools.Method(typeof(StatTooltipData_SetHeatData_Patch), "DissipationCapacity")
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

        public static float DissipationCapacity(this HeatSinkDef @this)
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