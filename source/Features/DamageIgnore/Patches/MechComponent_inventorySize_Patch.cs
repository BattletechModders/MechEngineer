using BattleTech;

namespace MechEngineer.Features.DamageIgnore.Patches;

[HarmonyPatch(typeof(MechComponent))]
[HarmonyPatch(nameof(MechComponent.inventorySize), MethodType.Getter)]
public static class MechComponent_inventorySize_Patch
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, MechComponent __instance, ref int __result)
    {
        if (!__runOriginal)
        {
            return;
        }

        if (__instance.componentDef?.IsIgnoreDamage() ?? false)
        {
            __result = 0;
            __runOriginal = false;
        }
    }
}
