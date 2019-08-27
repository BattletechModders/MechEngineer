using System;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace MechEngineer.Features.AccuracyEffects.Patches
{
    [HarmonyPatch(typeof(CombatHUDWeaponSlot), "AddToolTipDetail")]
    public static class CombatHUDWeaponSlot_AddToolTipDetail_Patch
    {
        public static bool Prefix(Weapon ___displayedWeapon, ref string description)
        {
            try
            {
                if (description == "ARM MOUNTED")
                {
                    var location = Mech.GetAbbreviatedChassisLocation((ChassisLocations) ___displayedWeapon.Location);
                    description = $"{location} MOUNTED";

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
