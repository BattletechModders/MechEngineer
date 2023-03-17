using System;
using BattleTech;
using MechEngineer.Features.DamageIgnore;

namespace MechEngineer.Features.CriticalEffects.Patches;

[HarmonyPatch(typeof(MechComponent))]
[HarmonyPatch(nameof(MechComponent.inventorySize), MethodType.Getter)]
internal static class MechComponent_inventorySize_Patch
{
    [HarmonyAfter(DamageIgnoreFeature.Namespace)]
    [HarmonyPrefix]
    public static void Prefix(ref bool __runOriginal, MechComponent __instance, ref int __result)
    {
        if (!__runOriginal)
        {
            return;
        }

        try
        {
            __result = __instance.Criticals().ComponentHittableCount();
            __runOriginal = false;
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
    }
}
