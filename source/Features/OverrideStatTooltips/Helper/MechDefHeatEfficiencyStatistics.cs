using BattleTech;
using MechEngineer.Features.Engines.Helper;
using MechEngineer.Features.Engines.StaticHandler;
using MechEngineer.Features.OverrideTonnage;
using System.Linq;
using MechEngineer.Features.HeatSinkCapacityStat;
using MechEngineer.Helper;

namespace MechEngineer.Features.OverrideStatTooltips.Helper;

internal class MechDefHeatEfficiencyStatistics
{
    internal int HeatSinking { get; }
    internal int AlphaStrike { get; }
    internal int JumpHeat { get; }
    internal int MaxHeat { get; }
    internal int Overheat { get; }

    internal MechDefHeatEfficiencyStatistics(MechDef mechDef)
    {
        this.mechDef = mechDef;

        HeatSinkCapacity = GetHeatSinkCapacity();
        HeatSinking = (int)(HeatSinkCapacity * MechStatisticsRules.Combat.Heat.GlobalHeatSinkMultiplier);
        AlphaStrike = (int)(GetHeatGenerated() * GetWeaponHeatMultiplier());
        JumpHeat = GetJumpHeat();
        MaxHeat = GetMaxHeat();
        Overheat = GetOverheat();
    }

    private int HeatSinkCapacity { get; }

    private readonly MechDef mechDef;
    private readonly StatCollection statCollection = new();

    internal float GetStatisticRating()
    {
        return AlphaStrike < 1
            ? 0
            : MechStatUtils.NormalizeToFraction(HeatSinking, (float)AlphaStrike / 3, AlphaStrike);
    }

    private int GetHeatSinkCapacity()
    {
        var stat = statCollection.HeatSinkCapacity();
        stat.Create();

        var engineEffect = HeatSinkCapacityStatFeature.EngineEffectForMechDef(mechDef);
        stat.Modify(engineEffect);

        return MechDefStatisticModifier.ModifyStatistic(stat, mechDef);
    }

    private int GetHeatGenerated()
    {
        var defaultValue = mechDef.Inventory
            .Where(x => x.IsFunctionalORInstalling())
            .Select(x => x.Def as WeaponDef)
            .Where(x => x != null)
            .Select(x => x!)
            .Sum(x => x.HeatGenerated);

        // TODO HeatDivisor support or not? nah we just don't support COIL, who needs that anyway...

        var stat = statCollection.HeatGenerated();
        stat.CreateWithDefault(defaultValue);
        var value = MechDefStatisticModifier.ModifyStatistic(stat, mechDef);
        return PrecisionUtils.RoundUpToInt(value);
    }

    private float GetWeaponHeatMultiplier()
    {
        var stat = statCollection.WeaponHeatMultiplier();
        stat.Create();
        return MechDefStatisticModifier.ModifyStatistic(stat, mechDef);
    }

    private int GetJumpHeat()
    {
        var stat = statCollection.JumpHeat();
        stat.Create();
        MechDefStatisticModifier.ModifyStatistic(stat, mechDef);
        return statCollection.GetJumpHeat(1);
    }

    private int GetMaxHeat()
    {
        var stat = statCollection.MaxHeat();
        stat.Create();
        return MechDefStatisticModifier.ModifyStatistic(stat, mechDef);
    }

    private int GetOverheat()
    {
        var stat = statCollection.OverheatLevel();
        stat.Create();
        return MechDefStatisticModifier.ModifyStatistic(stat, mechDef);
    }
}