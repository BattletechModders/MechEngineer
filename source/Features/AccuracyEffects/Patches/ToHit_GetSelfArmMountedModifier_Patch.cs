using System;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.AccuracyEffects.Patches;

[HarmonyPatch(typeof(ToHit), nameof(ToHit.GetSelfArmMountedModifier))]
public static class ToHit_GetSelfArmMountedModifier_Patch
{
    [HarmonyPrefix]
    public static bool Prefix(Weapon weapon, ref float __result)
    {
        try
        {
            if (weapon.parent is Mech mech)
            {
                __result += AccuracyEffectsFeature.AccuracyForLocation(
                    mech.StatCollection,
                    weapon.mechComponentRef.MountedLocation
                );

                return false;
            }
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }

        return true;
    }
}