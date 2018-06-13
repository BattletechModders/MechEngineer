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
            return instructions
                .MethodReplacer(
                    AccessTools.Method(typeof(SimGameState), "GetMechComponentRefForUID"),
                    AccessTools.Method(typeof(SimGameStateMoveWorkOrderItemsToQueuePatch), "GetMechComponentRefForUID")
                )
                .MethodReplacer(
                    AccessTools.Method(typeof(SimGameState), "RemoveItemStat", new[] { typeof(string), typeof(Type), typeof(bool) }),
                    AccessTools.Method(typeof(SimGameStateMoveWorkOrderItemsToQueuePatch), "RemoveItemStat")
                );
        }

        internal static MechComponentRef GetMechComponentRefForUID(this SimGameState sim, MechDef mech, string simGameUID, string componentID, ComponentType componentType, ComponentDamageLevel damageLevel, ChassisLocations desiredLocation, int hardpointSlot, ref bool itemWasFromInventory)
        {
            mechComponentRef = sim.GetMechComponentRefForUID(mech, simGameUID, componentID, componentType, damageLevel, desiredLocation, hardpointSlot, ref itemWasFromInventory);
            return mechComponentRef;
        }

        internal static void RemoveItemStat(this SimGameState sim, string id, Type type, bool damaged)
        {
            if (mechComponentRef != null && !EnginePersistence.RemoveItemStat(sim, mechComponentRef, id, type, damaged))
            {
                return;
            }

            sim.RemoveItemStat(id, type, damaged);
        }

        internal static MechComponentRef mechComponentRef;

        internal static void Postfix()
        {
            mechComponentRef = null;
        }
    }
}