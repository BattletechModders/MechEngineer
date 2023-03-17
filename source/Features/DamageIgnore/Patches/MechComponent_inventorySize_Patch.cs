using System;
using BattleTech;

namespace MechEngineer.Features.DamageIgnore.Patches;

[HarmonyPatch(typeof(MechComponent))]
[HarmonyPatch(nameof(MechComponent.inventorySize), MethodType.Getter)]
public static class MechComponent_inventorySize_Patch
{
    [HarmonyPrefix]
    public static void Prefix(ref bool __runOriginal, MechComponent __instance, ref int __result)
    {
        if (!__runOriginal)
        {
            return;
        }

        try
        {
            if (__instance.componentDef?.IsIgnoreDamage() ?? false)
            {
                __result = 0;
                __runOriginal = false;
            }
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
    }
}
