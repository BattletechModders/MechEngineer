using System;
using BattleTech;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(MechComponent))]
    [HarmonyPatch(nameof(MechComponent.inventorySize), MethodType.Getter)]
    internal static class MechComponent_inventorySize_Patch
    {
        public static void Postfix(MechComponent __instance, ref int __result)
        {
            try
            {
                if (__instance.mechComponentRef?.Def?.IsIgnoreDamage() ?? false)
                {
                    __result = 0;
                    return;
                }
                __result = __instance.CriticalSlots();
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}