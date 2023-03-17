using BattleTech;

namespace MechEngineer.Features.DamageIgnore.Patches;

[HarmonyPatch(typeof(MechComponent), nameof(MechComponent.DamageComponent))]
public static class MechComponent_DamageComponent_Patch
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, MechComponent __instance, ref ComponentDamageLevel damageLevel)
    {
        if (!__runOriginal)
        {
            return;
        }

        if (__instance.componentDef.IsIgnoreDamage())
        {
            damageLevel = ComponentDamageLevel.Functional;
            __runOriginal = false;
        }
    }
}
