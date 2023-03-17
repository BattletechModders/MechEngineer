using BattleTech;

namespace MechEngineer.Features.AccuracyEffects.Patches;

[HarmonyPatch(typeof(ToHit), nameof(ToHit.GetSelfArmMountedModifier))]
public static class ToHit_GetSelfArmMountedModifier_Patch
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, Weapon weapon, ref float __result)
    {
        if (!__runOriginal)
        {
            return;
        }

        if (weapon.parent is Mech mech)
        {
            __result += AccuracyEffectsFeature.AccuracyForLocation(
                mech.StatCollection,
                weapon.mechComponentRef.MountedLocation
            );

            __runOriginal = false;
        }
    }
}
