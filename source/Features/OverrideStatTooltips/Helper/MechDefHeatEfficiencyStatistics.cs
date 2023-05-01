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
    private static readonly HeatConstantsDef s_heatConstants = MechStatisticsRules.Combat.Heat;

    internal int HeatSinking { get; }
    internal int AlphaStrike { get; }
    internal int JumpHeat { get; }
    internal int MaxHeat { get; }
    internal int Overheat { get; }
    internal int EndMoveHeat { get; }

    internal MechDefHeatEfficiencyStatistics(MechDef mechDef)
    {
        this.mechDef = mechDef;

        var weaponStats = mechDef.Inventory
            .Where(x => x.IsFunctionalORInstalling())
            .Where(x => x.Def is WeaponDef)
            .Select(x => new WeaponDefHeatStatistics(mechDef, x, (x.Def as WeaponDef)!))
            .ToList();

        HeatSinkCapacity = GetHeatSinkCapacity();
        HeatSinking = (int)(HeatSinkCapacity * s_heatConstants.GlobalHeatSinkMultiplier);
        AlphaStrike = weaponStats.Select(x => x.HeatGeneratedWhenFired).Sum();
        JumpHeat = GetJumpHeat();
        MaxHeat = GetMaxHeat();
        Overheat = GetOverheat();
        EndMoveHeat = GetEndMoveHeat();
    }

    private int HeatSinkCapacity { get; }

    private readonly MechDef mechDef;
    private readonly StatCollection statCollection = new();

    internal float GetStatisticRating()
    {
        return AlphaStrike < 1
            ? 0
            : MechStatUtils.NormalizeToFraction(HeatSinking - EndMoveHeat, (float)AlphaStrike / 3, AlphaStrike);
    }

    private int GetHeatSinkCapacity()
    {
        var stat = statCollection.HeatSinkCapacity();
        stat.Create();

        var engineEffect = HeatSinkCapacityStatFeature.EngineEffectForMechDef(mechDef);
        stat.Modify(engineEffect);

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

    private int GetEndMoveHeat()
    {
        var stat = statCollection.EndMoveHeat();
        stat.Create();
        return MechDefStatisticModifier.ModifyStatistic(stat, mechDef);
    }

    private class WeaponDefHeatStatistics
    {
        internal int HeatGeneratedWhenFired { get; }

        internal WeaponDefHeatStatistics(MechDef mechDef, BaseComponentRef weaponRef, WeaponDef weaponDef)
        {
            this.mechDef = mechDef;
            this.weaponRef = weaponRef;
            this.weaponDef = weaponDef;

            {
                var heatGeneratedStat = GetHeatGenerated(GetBaseHeatGenerated);
                var weaponHeatMultiplierStat = GetWeaponHeatMultiplier();
                // see Weapon.HeatGenerated
                var heatGeneratedGetter = heatGeneratedStat * s_heatConstants.GlobalHeatIncreaseMultiplier * weaponHeatMultiplierStat;
                // see Weapon.FireWeapon()
                HeatGeneratedWhenFired = (int)heatGeneratedGetter;
            }
        }

        private readonly MechDef mechDef;
        private readonly BaseComponentRef? weaponRef;
        private readonly WeaponDef weaponDef;
        private readonly StatCollection statCollection = new();

        private int GetBaseHeatGenerated => weaponRef?.WeaponRefHelper().HeatGenerated ?? weaponDef.HeatGenerated;

        private float GetHeatGenerated(int baseValue)
        {
            var stat = statCollection.HeatGenerated();
            stat.CreateWithDefault(baseValue);
            return MechDefStatisticModifier.ModifyWeaponStatistic(stat, mechDef, weaponDef);
        }

        private float GetWeaponHeatMultiplier()
        {
            var stat = statCollection.WeaponHeatMultiplier();
            stat.Create();
            return MechDefStatisticModifier.ModifyWeaponStatistic(stat, mechDef, weaponDef);
        }
    }
}