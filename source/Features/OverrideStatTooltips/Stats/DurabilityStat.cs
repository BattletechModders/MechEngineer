using BattleTech;
using Localize;
using MechEngineer.Features.ArmorStructureChanges;
using MechEngineer.Features.OverrideStatTooltips.Helper;
using MechEngineer.Helper;

namespace MechEngineer.Features.OverrideStatTooltips.Stats;

internal class DurabilityStat : IStatHandler
{
    public void SetupTooltip(StatTooltipData tooltipData, MechDef mechDef)
    {
        tooltipData.dataList.Clear();

        {
            var value = mechDef.MechDefAssignedArmor;
            value *= ArmorMultiplier(mechDef);
            value *= DamageReductionMultiplierAll(mechDef);
            tooltipData.dataList.Add("<u>" + Strings.T("Armor") + "</u>", $"{value}");
        }

        {
            var value = DamageReductionMultiplierAll(mechDef);
            tooltipData.dataList.Add("<u>" + Strings.T("Damage Reduction") + "</u>", Strings.T("{0} %", value));
/*
<float>("DamageReductionMultiplierAll", 1f);
<float>("DamageReductionMultiplierMelee", 1f);
<float>("DamageReductionMultiplierEnergy", 1f);
<float>("DamageReductionMultiplierBallistic", 1f);
<float>("DamageReductionMultiplierMissile", 1f);
<float>("DamageReductionMultiplierAntipersonnel", 1f);
*/
        }

        {
            // stats.MaxStructureFactor
            var value = mechDef.MechDefMaxStructure;
            value *= StructureMultiplier(mechDef);
            tooltipData.dataList.Add(Strings.T("Structure"), $"{value}");
        }

        {
            // used by vanilla .. ReceivedInstabilityMultiplier
            var stability = MaxStability(mechDef);
            var unsteadyThreshold = UnsteadyThreshold(mechDef);
            tooltipData.dataList.Add(Strings.T("Stability / Unsteady"), $"{stability} / {unsteadyThreshold}");
        }

        {
            var value = MaxEvasivePips(mechDef);
            tooltipData.dataList.Add(Strings.T("Max Evasive Pips"), $"{value}");
        }

        {
            var value = ToHitThisActor(mechDef);
            value += GetTargetSizeModifier(mechDef.Chassis.weightClass);
            tooltipData.dataList.Add(Strings.T("Defense"), $"{value}");
/*
<float>("ToHitThisActor", 0f);
<float>("ToHitThisActorBallistic", 0f);
<float>("ToHitThisActorEnergy", 0f);
<float>("ToHitThisActorMissile", 0f);
<float>("ToHitThisActorSmall", 0f);
<float>("ToHitThisActorMelee", 0f);
<float>("ToHitThisActorDirectFire", 0f);
*/
        }
    }

    public float BarValue(MechDef mechDef)
    {
        var armor = mechDef.MechDefAssignedArmor;
        armor *= ArmorMultiplier(mechDef);
        armor *= DamageReductionMultiplierAll(mechDef);

        var stats = UnityGameInstance.BattleTechGame.MechStatisticsConstants;
        return MechStatUtils.NormalizeToFraction(armor, 0, stats.MaxArmorFactor);
    }

    private float ArmorMultiplier(MechDef mechDef)
    {
        var statCollection = new StatCollection();
        var stat = statCollection.ArmorMultiplier();
        stat.Create();
        return MechDefStatisticModifier.ModifyStatistic(stat, mechDef);
    }

    private float StructureMultiplier(MechDef mechDef)
    {
        var statCollection = new StatCollection();
        var stat = statCollection.StructureMultiplier();
        stat.Create();
        return MechDefStatisticModifier.ModifyStatistic(stat, mechDef);
    }

    private float DamageReductionMultiplierAll(MechDef mechDef)
    {
        var statCollection = new StatCollection();
        var stat = DamageReductionMultiplierAll(statCollection);
        stat.Create();
        return MechDefStatisticModifier.ModifyStatistic(stat, mechDef);
    }

    private static StatisticAdapter<float> DamageReductionMultiplierAll(StatCollection statCollection)
    {
        return new("DamageReductionMultiplierAll", statCollection, 1);
    }

    private float MaxStability(MechDef mechDef)
    {
        var statCollection = new StatCollection();
        var stat = MaxStability(statCollection, mechDef.Chassis.Stability);
        stat.Create();
        return MechDefStatisticModifier.ModifyStatistic(stat, mechDef);
    }

    private static StatisticAdapter<float> MaxStability(StatCollection statCollection, float defaultValue)
    {
        return new("MaxStability", statCollection, defaultValue);
    }

    private float UnsteadyThreshold(MechDef mechDef)
    {
        var statCollection = new StatCollection();
        var stat = UnsteadyThreshold(statCollection);
        stat.Create();
        return MechDefStatisticModifier.ModifyStatistic(stat, mechDef);
    }

    private static StatisticAdapter<float> UnsteadyThreshold(StatCollection statCollection)
    {
        return new("UnsteadyThreshold", statCollection, MechStatisticsRules.Combat.ResolutionConstants.DefaultUnsteadyThreshold);
    }

    private float MaxEvasivePips(MechDef mechDef)
    {
        var statCollection = new StatCollection();
        var stat = MaxEvasivePips(statCollection);
        stat.Create();
        return MechDefStatisticModifier.ModifyStatistic(stat, mechDef);
    }

    private static StatisticAdapter<int> MaxEvasivePips(StatCollection statCollection)
    {
        return new("MaxEvasivePips", statCollection, 6); // probably is overwritten by many mods
    }

    private float ToHitThisActor(MechDef mechDef)
    {
        var statCollection = new StatCollection();
        var stat = ToHitThisActor(statCollection);
        stat.Create();
        return MechDefStatisticModifier.ModifyStatistic(stat, mechDef);
    }

    private static StatisticAdapter<float> ToHitThisActor(StatCollection statCollection)
    {
        return new("ToHitThisActor", statCollection, 0);
    }

    private static float GetTargetSizeModifier(WeightClass weightClass)
    {
        switch (weightClass)
        {
            case WeightClass.LIGHT:
                return MechStatisticsRules.Combat.ToHit.ToHitLight;
            case WeightClass.MEDIUM:
                return MechStatisticsRules.Combat.ToHit.ToHitMedium;
            case WeightClass.HEAVY:
                return MechStatisticsRules.Combat.ToHit.ToHitHeavy;
            case WeightClass.ASSAULT:
                return MechStatisticsRules.Combat.ToHit.ToHitAssault;
            default:
                return 0;
        }
    }
}