using System;
using System.Linq;
using BattleTech;
using CustomComponents;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(ToHit), "GetSelfArmMountedModifier")]
    public static class ToHit_GetSelfArmMountedModifier_Patch
    {
        public static bool Prefix(Weapon weapon, ref float __result)
        {
            try
            {
                if (weapon.parent is Mech mech)
                {
                    __result += AccuracyEffects.AccuracyForLocation(
                        mech.StatCollection,
                        weapon.mechComponentRef.MountedLocation
                    );
                    
                    return false;
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }

            return true;
        }
    }
}
