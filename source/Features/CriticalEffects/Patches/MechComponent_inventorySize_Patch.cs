using System;
using BattleTech;
using Harmony;
using MechEngineer.Features.DamageIgnore;

namespace MechEngineer.Features.CriticalEffects.Patches;

[HarmonyPatch(typeof(MechComponent))]
[HarmonyPatch(nameof(MechComponent.inventorySize), MethodType.Getter)]
internal static class MechComponent_inventorySize_Patch
{
    [HarmonyAfter(DamageIgnoreFeature.Namespace)]
    [HarmonyPrefix]
    public static bool Prefix(MechComponent __instance, ref int __result)
    {
        try
        {
            __result = __instance.Criticals().ComponentHittableCount();
            return false;
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
        return true;
    }
}