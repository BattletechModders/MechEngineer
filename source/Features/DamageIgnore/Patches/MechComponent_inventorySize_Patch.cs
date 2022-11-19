using System;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.DamageIgnore.Patches;

[HarmonyPatch(typeof(MechComponent))]
[HarmonyPatch(nameof(MechComponent.inventorySize), MethodType.Getter)]
public static class MechComponent_inventorySize_Patch
{
    [HarmonyPrefix]
    public static bool Prefix(MechComponent __instance, ref int __result)
    {
        Log.Main.Trace?.Log("MechComponent_inventorySize_Patch");
        try
        {
            if (__instance.componentDef?.IsIgnoreDamage() ?? false)
            {
                __result = 0;
                return false;
            }
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }

        return true;
    }
}
