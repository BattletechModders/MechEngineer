using BattleTech.UI;

namespace MechEngineer.Features.AccuracyEffects.Patches;

[HarmonyPatch(typeof(CombatHUDWeaponSlot), nameof(CombatHUDWeaponSlot.AddToolTipDetail))]
public static class CombatHUDWeaponSlot_AddToolTipDetail_Patch
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, ref string description)
    {
        if (!__runOriginal)
        {
            return;
        }

        if (description == "ARM MOUNTED")
        {
            description = AccuracyEffectsFeature.Shared.Settings.CombatHUDTooltipName;
        }
    }
}
