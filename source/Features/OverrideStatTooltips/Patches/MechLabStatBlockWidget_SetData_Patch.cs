using BattleTech.UI;

namespace MechEngineer.Features.OverrideStatTooltips.Patches;

[HarmonyPatch(typeof(MechLabStatBlockWidget), nameof(MechLabStatBlockWidget.Awake))]
public static class MechLabStatBlockWidget_Awake_Patch
{
    [HarmonyPostfix]
    [HarmonyWrapSafe]
    public static void Postfix(LanceStat[] ___mechStats)
    {
        MechBayMechInfoWidget_Awake_Patch.SetMechStats(___mechStats);
    }
}
