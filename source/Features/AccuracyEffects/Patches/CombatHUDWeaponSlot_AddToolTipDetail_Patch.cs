using System;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace MechEngineer.Features.AccuracyEffects.Patches
{
    [HarmonyPatch(typeof(CombatHUDWeaponSlot), "AddToolTipDetail")]
    public static class CombatHUDWeaponSlot_AddToolTipDetail_Patch
    {
        public static void Prefix(ref string description)
        {
            try
            {
                if (description == "ARM MOUNTED")
                {
                    description = AccuracyEffectsFeature.Shared.Settings.CombatHUDTooltipName;
                }
            }
            catch (Exception e)
            {
                Control.Logger.Error.Log(e);
            }
        }
    }
}
