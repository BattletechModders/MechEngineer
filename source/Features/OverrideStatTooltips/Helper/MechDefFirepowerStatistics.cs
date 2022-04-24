using BattleTech;
using System;
using System.Linq;
using MechEngineer.Helper;
using UnityEngine;

namespace MechEngineer.Features.OverrideStatTooltips.Helper;

internal class MechDefFirepowerStatistics
{
    internal float TotalDamage { get; }
    internal float TotalHeatDamage { get; }
    internal float TotalStructureDamage { get; }
    internal float TotalInstability { get; }
    internal float AverageAccuracy { get; }

    internal MechDefFirepowerStatistics(MechDef mechDef, int minRange, int maxRange)
        : this(mechDef, (x) => RangeOverlap(minRange, maxRange, Mathf.RoundToInt(x.MinRange), Mathf.RoundToInt(x.MaxRange)))
    {
    }

    internal MechDefFirepowerStatistics(MechDef mechDef, Func<WeaponDef, bool> customFilter = null)
    {
        var weaponStats = mechDef.Inventory
            .Where(x => x.IsFunctionalORInstalling())
            .Select(x => x.Def as WeaponDef)
            .Where(x => x != null)
            .Where(customFilter)
            .Select(x => new WeaponDefFirepowerStatistics(mechDef, x))
            .ToList();

        TotalDamage = weaponStats.Select(x => x.Damage).DefaultIfEmpty(0).Sum();
        TotalHeatDamage = weaponStats.Select(x => x.HeatDamage).DefaultIfEmpty(0).Sum();
        TotalStructureDamage = weaponStats.Select(x => x.StructureDamage).DefaultIfEmpty(0).Sum();
        TotalInstability = weaponStats.Select(x => x.Instability).DefaultIfEmpty(0f).Sum();
        AverageAccuracy = weaponStats.Select(x => x.Accuracy).DefaultIfEmpty(0f).Average();
    }

    internal static bool RangeOverlap(int range1Min, int range1Max, int range2Min, int range2Max)
    {
        return range1Min <= range2Max && range2Min <= range1Max;
    }

    internal float BarValue(float totalDamage, bool useMeleeConstants = false)
    {
        var constants = UnityGameInstance.BattleTechGame.MechStatisticsConstants;
        var min = useMeleeConstants ? constants.MinStockMeleeDamage + constants.MinStockFirepower : constants.MinStockFirepower;
        var max = useMeleeConstants ? constants.MaxStockMeleeDamage + constants.MaxStockFirepower : constants.MaxStockFirepower;
        return MechStatUtils.NormalizeToFraction(totalDamage, min, max);
    }

    internal static WeaponDefFirepowerStatistics GetMelee(MechDef mechDef)
    {
        var weaponDef = UnityGameInstance.BattleTechGame.DataManager.WeaponDefs.Get("Weapon_MeleeAttack");
        var stats = new WeaponDefFirepowerStatistics(mechDef, weaponDef, mechDef.Chassis.MeleeDamage, mechDef.Chassis.MeleeInstability, mechDef.Chassis.MeleeToHitModifier);
        return stats;
    }

    internal static WeaponDefFirepowerStatistics GetDFA(MechDef mechDef)
    {
        var weaponDef = UnityGameInstance.BattleTechGame.DataManager.WeaponDefs.Get("Weapon_DFAAttack");
        var stats = new WeaponDefFirepowerStatistics(mechDef, weaponDef, mechDef.Chassis.DFADamage, mechDef.Chassis.DFAInstability, mechDef.Chassis.DFAToHitModifier);
        return stats;
    }

    internal class WeaponDefFirepowerStatistics
    {
        internal float Damage { get; }
        internal float HeatDamage { get; }
        internal float StructureDamage { get; }
        internal float ShotWhenFired { get; }
        internal float Instability { get; }
        internal float Accuracy { get; }

        internal WeaponDefFirepowerStatistics(MechDef mechDef, WeaponDef weaponDef) :
            this(mechDef, weaponDef, weaponDef.Damage, weaponDef.Instability, weaponDef.AccuracyModifier)
        {
        }

        internal WeaponDefFirepowerStatistics(MechDef mechDef, WeaponDef weaponDef, float damage, float instability, float accuracy)
        {
            this.weaponDef = weaponDef;
            this.mechDef = mechDef;

            ShotWhenFired = GetShotsWhenFired(weaponDef.ShotsWhenFired);
            Damage = GetDamagePerShot(damage) * ShotWhenFired;
            HeatDamage = GetHeatDamage(weaponDef.HeatDamage) * ShotWhenFired;
            StructureDamage = GetStructureDamage(weaponDef.StructureDamage) * ShotWhenFired;
            Instability = GetInstability(instability) * ShotWhenFired;
            Accuracy = GetAccuracyModifier(accuracy);
        }

        private readonly WeaponDef weaponDef;
        private readonly MechDef mechDef;
        private readonly StatCollection statCollection = new();

        private float GetDamagePerShot(float baseValue)
        {
            var stat = statCollection.DamagePerShot(baseValue);
            stat.Create();
            return MechDefStatisticModifier.ModifyWeaponStatistic(stat, mechDef, weaponDef);
        }

        private float GetShotsWhenFired(int baseValue)
        {
            var stat = statCollection.ShotsWhenFired(baseValue);
            stat.Create();
            return MechDefStatisticModifier.ModifyWeaponStatistic(stat, mechDef, weaponDef);
        }

        private float GetHeatDamage(float baseValue)
        {
            var stat = statCollection.HeatDamagePerShot(baseValue);
            stat.Create();
            return MechDefStatisticModifier.ModifyWeaponStatistic(stat, mechDef, weaponDef);
        }

        private float GetStructureDamage(float baseValue)
        {
            var stat = statCollection.StructureDamagePerShot(baseValue);
            stat.Create();
            return MechDefStatisticModifier.ModifyWeaponStatistic(stat, mechDef, weaponDef);
        }

        private float GetInstability(float baseValue)
        {
            var stat = statCollection.Instability(baseValue);
            stat.Create();
            return MechDefStatisticModifier.ModifyWeaponStatistic(stat, mechDef, weaponDef);
        }

        private float GetAccuracyModifier(float baseValue)
        {
            var stat = statCollection.AccuracyModifier(baseValue);
            stat.Create();
            return MechDefStatisticModifier.ModifyWeaponStatistic(stat, mechDef, weaponDef);
        }
    }
}

internal static class StatCollectionExtension
{
    internal static StatisticAdapter<float> DamagePerShot(this StatCollection statCollection, float baseValue)
    {
        return new("DamagePerShot", statCollection, baseValue);
    }

    internal static StatisticAdapter<int> ShotsWhenFired(this StatCollection statCollection, int baseValue)
    {
        return new("ShotsWhenFired", statCollection, baseValue);
    }

    internal static StatisticAdapter<float> Instability(this StatCollection statCollection, float baseValue)
    {
        return new("Instability", statCollection, baseValue);
    }

    internal static StatisticAdapter<float> AccuracyModifier(this StatCollection statCollection, float baseValue)
    {
        return new("AccuracyModifier", statCollection, baseValue);
    }

    internal static StatisticAdapter<float> StructureDamagePerShot(this StatCollection statCollection, float baseValue)
    {
        return new("StructureDamagePerShot", statCollection, baseValue);
    }

    internal static StatisticAdapter<float> HeatDamageModifier(this StatCollection statCollection, float baseValue)
    {
        return new("HeatDamageModifier", statCollection, baseValue);
    }

    internal static StatisticAdapter<float> HeatDamagePerShot(this StatCollection statCollection, float baseValue)
    {
        return new("HeatDamagePerShot", statCollection, baseValue);
    }
}

/*
<float>("MinRangeMultiplier", 1f);
<float>("LongRangeModifier", 0f);
<float>("MaxRangeModifier", 0f);
<float>("ShortRange", this.weaponDef.ShortRange);
<float>("MediumRange", this.weaponDef.MediumRange);
<float>("LongRange", this.weaponDef.LongRange);
<float>("HeatGenerated", (float)this.weaponDef.HeatGenerated);
<float>("CriticalChanceMultiplier", this.weaponDef.CriticalChanceMultiplier);
<int>("RefireModifier", this.weaponDef.RefireModifier);
<int>("ProjectilesPerShot", this.weaponDef.ProjectilesPerShot);
<int>("VolleyDivisor", this.weaponDef.VolleyDivisor);
<int>("AttackRecoil", this.weaponDef.AttackRecoil);
<int>("InternalAmmo", this.weaponDef.StartingAmmoCapacity);
<float>("EvasivePipsIgnored", this.weaponDef.EvasivePipsIgnored);
<bool>("TemporarilyDisabled", false);
<float>("JumpingWeaponDamageModifier", 1f);
<float>("AccuracyModifier", this.weaponDef.AccuracyModifier);
<int>("DamageVariance", this.weaponDef.DamageVariance);
<float>("OverheatedDamageMultiplier", this.weaponDef.OverheatedDamageMultiplier);
<float>("ClusteringModifier", this.weaponDef.ClusteringModifier);

		public float GetRangeModifierForDist(Weapon weapon, float dist)
		{
			if (dist < weapon.MinRange)
			{
				return this.combat.Constants.ToHit.ToHitMinimumRange * weapon.StatCollection.GetValue<float>("MinRangeMultiplier");
			}
			if (dist < weapon.ShortRange)
			{
				return this.combat.Constants.ToHit.ToHitShortRange;
			}
			if (dist < weapon.MediumRange)
			{
				return this.combat.Constants.ToHit.ToHitMediumRange;
			}
			if (dist < weapon.LongRange)
			{
				return this.combat.Constants.ToHit.ToHitLongRange + weapon.StatCollection.GetValue<float>("LongRangeModifier");
			}
			return this.combat.Constants.ToHit.ToHitMaximumRange + weapon.StatCollection.GetValue<float>("MaxRangeModifier");
		}
 */