using System;
using BattleTech;

namespace MechEngineer.Features.AccuracyEffects.Patches;

[HarmonyPatch(typeof(ToHit), nameof(ToHit.GetSelfArmMountedModifier))]
public static class ToHit_GetSelfArmMountedModifier_Patch
{
    [HarmonyPrefix]
    public static void Prefix(ref bool __runOriginal, Weapon weapon, ref float __result)
    {
        if (!__runOriginal)
        {
            return;
        }

        try
        {
            if (weapon.parent is Mech mech)
            {
                __result += AccuracyEffectsFeature.AccuracyForLocation(
                    mech.StatCollection,
                    weapon.mechComponentRef.MountedLocation
                );

                __runOriginal = false;
            }
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
    }
}
