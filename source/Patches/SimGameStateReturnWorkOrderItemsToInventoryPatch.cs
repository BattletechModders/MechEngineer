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
            return instructions
                .MethodReplacer(
                    AccessTools.Method(typeof(List<>), "Remove"),
                    AccessTools.Method(typeof(SimGameStateReturnWorkOrderItemsToInventoryPatch), "Remove")
                )
                .MethodReplacer(
                    AccessTools.Method(typeof(SimGameState), "AddItemStat", new[] { typeof(string), typeof(Type), typeof(bool) }),
                    AccessTools.Method(typeof(SimGameStateReturnWorkOrderItemsToInventoryPatch), "AddItemStat")
                );
        }

        internal static void Remove(this List<MechComponentRef> list, MechComponentRef item)
        {
            mechComponentRef = item;
            list.Remove(item);
        }

        internal static void AddItemStat(this SimGameState sim, string id, Type type, bool damaged)
        {
            if (mechComponentRef != null && !EnginePersistence.AddItemStat(sim, mechComponentRef, id, type, damaged))
            {
                return;
            }

            sim.AddItemStat(id, type, damaged);
        }

        internal static MechComponentRef mechComponentRef;

        internal static void Postfix()
        {
            mechComponentRef = null;
        }
    }
}