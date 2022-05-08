using System;
using BattleTech.UI;
using Harmony;

namespace MechEngineer.Features.AccuracyEffects.Patches;

[HarmonyPatch(typeof(CombatHUDWeaponSlot), nameof(CombatHUDWeaponSlot.AddToolTipDetail))]
public static class CombatHUDWeaponSlot_AddToolTipDetail_Patch
{
    [HarmonyPrefix]
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