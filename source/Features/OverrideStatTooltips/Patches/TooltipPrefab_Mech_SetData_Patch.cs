using BattleTech.UI;
using BattleTech.UI.Tooltips;

namespace MechEngineer.Features.OverrideStatTooltips.Patches;

[HarmonyPatch(typeof(TooltipPrefab_Mech), nameof(TooltipPrefab_Mech.SetData))]
public static class TooltipPrefab_Mech_SetData_Patch
{
    [HarmonyPostfix]
    [HarmonyWrapSafe]
    public static void Postfix(
        LanceStatGraphic ___FirepowerBar,
        LanceStatGraphic ___HeatEffBar,
        LanceStatGraphic ___AvgRangeBar,
        LanceStatGraphic ___DurabilityBar,
        LanceStatGraphic ___MeleeBar,
        LanceStatGraphic ___MovementBar
        )
    {
        var settings = OverrideStatTooltipsFeature.Shared.Settings;
        ___FirepowerBar.SetText(settings.FirepowerTitleText);
        ___MovementBar.SetText(settings.MovementTitleText);
        ___DurabilityBar.SetText(settings.DurabilityTitleText);
        ___HeatEffBar.SetText(settings.HeatEfficiencyTitleText);
        ___AvgRangeBar.SetText(settings.AvgRangeTitleText);
        ___MeleeBar.SetText(settings.MeleeTitleText);
    }
}
