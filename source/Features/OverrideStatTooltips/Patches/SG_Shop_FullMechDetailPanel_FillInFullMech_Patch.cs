using BattleTech.UI;

namespace MechEngineer.Features.OverrideStatTooltips.Patches;

[HarmonyPatch(typeof(SG_Shop_FullMechDetailPanel), nameof(SG_Shop_FullMechDetailPanel.FillInFullMech))]
public static class SG_Shop_FullMechDetailPanel_FillInFullMech_Patch
{
    [HarmonyPostfix]
    [HarmonyWrapSafe]
    public static void Postfix(
        LanceStat ___Stat1,
        LanceStat ___Stat2,
        LanceStat ___Stat3,
        LanceStat ___Stat4,
        LanceStat ___Stat5,
        LanceStat ___Stat6
        )
    {
        var settings = OverrideStatTooltipsFeature.Shared.Settings;
        ___Stat1.SetText(settings.FirepowerTitleText);
        ___Stat2.SetText(settings.MovementTitleText);
        ___Stat3.SetText(settings.DurabilityTitleText);
        ___Stat4.SetText(settings.HeatEfficiencyTitleText);
        ___Stat5.SetText(settings.AvgRangeTitleText);
        ___Stat6.SetText(settings.MeleeTitleText);
    }
}
