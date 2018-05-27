using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using Harmony;

namespace MechEngineMod
{
    // makes sure to properly handle mech inventory -> sim game state inventory for engine internal components
    // work orders is the mechanism to transfer material in between
    public static class SimGameStateAddItemStatPatch
    {
        internal static void OnAddItemStat(SimGameState sim, MechComponentRef componentRef)
        {
            EnginePersistence.OnAddItemStat(sim, componentRef);
        }

        internal static MechComponentRef lastMechComponentRef;

        internal static void OnAddItemStat(SimGameState sim)
        {
            if (lastMechComponentRef == null)
            {
                return;
            }
            OnAddItemStat(sim, lastMechComponentRef);
            lastMechComponentRef = null;
        }

        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return instructions.MethodReplacer(
                AccessTools.Method(typeof(SimGameState), "AddItemStat", new []{typeof(string), typeof(Type), typeof(bool)}),
                AccessTools.Method(typeof(SimGameStateAddItemStatPatch), "AddItemStat")
            );
        }

        public static void AddItemStat(this SimGameState instance, string id, Type type, bool damaged)
        {
            try
            {
                // Control.mod.Logger.LogDebug("AddItemStat id=" + id);

                instance.AddItemStat(id, type, damaged);
                OnAddItemStat(instance);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }

    [HarmonyPatch(typeof(SimGameState), "GetMechComponentRefForUID")]
    public static class SimGameStateGetMechComponentRefForUIDPatch
    {
        public static void Postfix(MechComponentRef __result)
        {
            SimGameStateAddItemStatPatch.lastMechComponentRef = __result;
        }
    }

    [HarmonyPatch(typeof(SimGameState), "ML_InstallComponent")]
    public static class SimGameStateML_InstallComponentPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return SimGameStateAddItemStatPatch.Transpiler(instructions);
        }
    }

    [HarmonyPatch(typeof(SimGameState), "ML_RepairComponent")]
    public static class SimGameStateML_RepairComponentPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return SimGameStateAddItemStatPatch.Transpiler(instructions);
        }
    }

    [HarmonyPatch(typeof(SimGameState), "ReturnWorkOrderItemsToInventory")]
    public static class SimGameStateReturnWorkOrderItemsToInventoryPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return SimGameStateAddItemStatPatch.Transpiler(instructions);
        }

        public static void Prefix(SimGameState __instance, WorkOrderEntry entry)
        {
            try
            {
                var install = entry as WorkOrderEntry_InstallComponent;
                var repair = entry as WorkOrderEntry_RepairComponent;

                string SimGameUID;
                if (install != null)
                {
                    SimGameUID = install.ComponentSimGameUID;
                }
                else if (repair != null)
                {
                    SimGameUID = repair.ComponentSimGameUID;
                }
                else
                {
                    return;
                }

                SimGameStateAddItemStatPatch.lastMechComponentRef = __instance.WorkOrderComponents.FirstOrDefault(c => c.SimGameUID == SimGameUID);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }

    [HarmonyPatch(typeof(SimGameState), "StripMech")]
    public static class SimGameStateStripMechPatch
    {
        public static void Prefix(SimGameState __instance, MechDef def)
        {
            try
            {
                foreach (var mechComponentRef in def.Inventory)
                {
                    if (mechComponentRef.DamageLevel == ComponentDamageLevel.Destroyed)
                    {
                        continue;
                    }

                    SimGameStateAddItemStatPatch.OnAddItemStat(__instance, mechComponentRef);
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}