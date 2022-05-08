using System;
using BattleTech;
using BattleTech.UI.TMProWrapper;
using BattleTech.UI.Tooltips;
using Harmony;

namespace MechEngineer.Features.OverrideStatTooltips.Patches;

[HarmonyPatch(typeof(TooltipPrefab_Stats), nameof(TooltipPrefab_Stats.SetData))]
public static class TooltipPrefab_Stats_SetData_Patch
{
    [HarmonyPostfix]
    public static void Postfix(object data, LocalizableText ___Title)
    {
        try
        {
            var settings = OverrideStatTooltipsFeature.Shared.Settings;
            var tooltipData = (StatTooltipData)data;
            switch (tooltipData.dataType)
            {
                case StatType.Movement:
                    ___Title.SetText(settings.MovementTitleText);
                    break;
                case StatType.HeatEffeciency:
                    ___Title.SetText(settings.HeatEfficiencyTitleText);
                    break;
                case StatType.Durability:
                    ___Title.SetText(settings.DurabilityTitleText);
                    break;
                case StatType.AvgRange:
                    ___Title.SetText(settings.AvgRangeTitleText);
                    break;
                case StatType.Melee:
                    ___Title.SetText(settings.MeleeTitleText);
                    break;
                case StatType.Firepower:
                    ___Title.SetText(settings.FirepowerTitleText);
                    break;
            }
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
    }
}