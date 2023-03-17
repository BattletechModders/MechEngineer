using BattleTech;

namespace MechEngineer.Features.PlaceholderEffects.Patches;

[HarmonyPatch(typeof(MechComponent), nameof(MechComponent.ApplyPassiveEffectToTarget))]
public static class MechComponent_ApplyPassiveEffectToTarget_Patch
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, MechComponent __instance, ref EffectData effect)
    {
        if (!__runOriginal)
        {
            return;
        }

        PlaceholderEffectsFeature.ProcessLocationalEffectData(ref effect, __instance);
    }
}
