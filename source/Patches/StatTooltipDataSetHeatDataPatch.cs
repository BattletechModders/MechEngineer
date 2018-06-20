using System.Collections.Generic;
using BattleTech;
using Harmony;

namespace MechEngineMod
{
    [HarmonyPatch(typeof(StatTooltipData), "SetHeatData")]
    public static class StatTooltipDataSetHeatDataPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return instructions.MethodReplacer(
                AccessTools.Method(typeof(HeatSinkDef), "get_DissipationCapacity"),
                AccessTools.Method(typeof(StatTooltipDataSetHeatDataPatch), "DissipationCapacity")
            );
        }

        private static MechDef mechDef;

        public static void Prefix(MechDef def)
        {
            mechDef = def;
        }

        public static void Postfix()
        {
            mechDef = null;
        }

        public static float DissipationCapacity(this HeatSinkDef @this)
        {
            if (@this.IsMainEngine())
            {
                return EngineHeat.GetEngineHeatDissipation(mechDef.Inventory);
            }
            return @this.DissipationCapacity;
        }
    }
}