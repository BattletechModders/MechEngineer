using BattleTech;
using MechEngineer.Features.DamageIgnore;

namespace MechEngineer.Features.CriticalEffects.Patches;

[HarmonyPatch(typeof(MechComponent))]
[HarmonyPatch(nameof(MechComponent.inventorySize), MethodType.Getter)]
internal static class MechComponent_inventorySize_Patch
{
    [HarmonyAfter(DamageIgnoreFeature.Namespace)]
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, MechComponent __instance, ref int __result)
    {
        if (!__runOriginal)
        {
            return;
        }

        __result = __instance.Criticals().ComponentHittableCount();
        __runOriginal = false;
    }
}
