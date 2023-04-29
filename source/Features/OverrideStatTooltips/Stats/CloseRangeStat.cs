using BattleTech;
using Localize;
using MechEngineer.Features.OverrideStatTooltips.Helper;

namespace MechEngineer.Features.OverrideStatTooltips.Stats;

internal class CloseRangeStat : IStatHandler
{
    public void SetupTooltip(StatTooltipData tooltipData, MechDef mechDef)
    {
        {
            var melee = MechDefFirepowerStatistics.GetMelee(mechDef);
            var meleeFirepower = GetFirepower(mechDef, true);

            tooltipData.dataList.Add("<u>" + Strings.T("Melee Damage") + "</u>", $"{melee.Damage + meleeFirepower.TotalDamage}");
            //tooltipData.dataList.Add(Strings.T("Melee Total Damage"), $"{melee.Damage + meleeFirepower.TotalDamage}");
            tooltipData.dataList.Add(Strings.T("Melee Stability Dmg"), $"{melee.Instability + meleeFirepower.TotalInstability}");
            tooltipData.dataList.Add(Strings.T("M.Heat/Struct.Dmg"), $"{melee.HeatDamage + meleeFirepower.TotalHeatDamage}/{melee.StructureDamage + meleeFirepower.TotalStructureDamage}");
            //tooltipData.dataList.Add(Strings.T("Melee Accuracy"), $"{melee.Accuracy}");
            //tooltipData.dataList.Add(Strings.T("Melee Sup. Accuracy"), $"{meleeFirepower.AverageAccuracy}");
        }

        {
            var firepower = GetFirepower(mechDef, false);
            tooltipData.dataList.Add("<u>" + Strings.T("Ranged Damage") + "</u>", $"{firepower.TotalDamage}");
            tooltipData.dataList.Add(Strings.T("Ranged Stability Dmg"), $"{firepower.TotalInstability}");
            //tooltipData.dataList.Add(Strings.T("Ranged Average Accuracy"), $" {firepower.AverageAccuracy}");
            tooltipData.dataList.Add(Strings.T("R.Heat/Struct.Dmg"), $"{firepower.TotalHeatDamage}/{firepower.TotalStructureDamage}");
        }

        //{
        //	var dfa = MechDefFirepowerStatistics.GetDFA(mechDef);
        //	tooltipData.dataList.Add(Strings.T("DFA D/I/A/S"), $"{dfa.Damage} / {dfa.Instability} / {dfa.Accuracy} / {dfa.StructureDamage}");
        //}
    }

    public float BarValue(MechDef mechDef)
    {
        var firepower = GetFirepower(mechDef);
        var melee = MechDefFirepowerStatistics.GetMelee(mechDef);
        // TODO: make Mathf.Max between non-melee damage and melee+support damage, dont just add
        var totalDamage = firepower.TotalDamage + melee.Damage; // + firepower.GetDFA().Damage;
        return firepower.BarValue(totalDamage, true);
    }

    private MechDefFirepowerStatistics GetFirepower(MechDef mechDef, bool? canUseInMelee = null)
    {
        if (canUseInMelee.HasValue)
        {
            return new MechDefFirepowerStatistics(mechDef, x => x.WeaponRefHelper().CanUseInMelee == canUseInMelee.Value);
        }
        else
        {
            return new MechDefFirepowerStatistics(mechDef, 0, OverrideStatTooltipsFeature.Shared.Settings.CloseRangeMax);
        }
    }
}