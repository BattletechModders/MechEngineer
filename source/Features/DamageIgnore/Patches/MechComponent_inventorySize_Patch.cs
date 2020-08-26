using System;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.DamageIgnore.Patches
{
    [HarmonyPatch(typeof(MechComponent))]
    [HarmonyPatch(nameof(MechComponent.inventorySize), MethodType.Getter)]
    public static class MechComponent_inventorySize_Patch
    {
        public static void Postfix(MechComponent __instance, ref int __result)
        {
            try
            {
                if (__result != 0 && (__instance.mechComponentRef?.Def?.IsIgnoreDamage() ?? false))
                {
                    __result = 0;
                }
            }
            catch (Exception e)
            {
                Control.Logger.Error.Log(e);
            }
        }
    }
}