using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using Harmony;

namespace MechEngineMod
{
    [HarmonyPatch(typeof(SimGameState), "MoveWorkOrderItemsToQueue")]
    public static class SimGameStateMoveWorkOrderItemsToQueuePatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return EnginePersistanceItemStat.Transpiler(instructions);
        }
    }
}