using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using Harmony;

namespace MechEngineMod
{
    [HarmonyPatch(typeof(SimGameState), "ReturnWorkOrderItemsToInventory")]
    public static class SimGameStateReturnWorkOrderItemsToInventoryPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return EnginePersistanceItemStat.Transpiler(instructions);
        }
    }
}