using System;
using BattleTech;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(MechComponent))]
    [HarmonyPatch(nameof(MechComponent.inventorySize), MethodType.Getter)]
    internal static class MechComponent_inventorySize_Patch
    {
        public static int Prefix(MechComponent __instance)
        {
            try
            {
                return __instance.CriticalSlots();
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }

            return __instance.inventorySize;
        }
    }
}