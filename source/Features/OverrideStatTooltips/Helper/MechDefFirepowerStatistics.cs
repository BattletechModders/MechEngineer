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
        : this(mechDef, x => RangeOverlap(minRange, maxRange, Mathf.RoundToInt(x.WeaponRefHelper().MinRange), Mathf.RoundToInt(x.WeaponRefHelper().MaxRange)))
    {
    }

    internal MechDefFirepowerStatistics(MechDef mechDef, Func<BaseComponentRef, bool> customFilter)
    {
        var weaponStats = mechDef.Inventory
            .Where(x => x.IsFunctionalORInstalling())
            .Where(x => x.Def is WeaponDef)
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
        var stats = new WeaponDefFirepowerStatistics(mechDef, weaponDef, null, mechDef.Chassis.MeleeDamage, mechDef.Chassis.MeleeInstability, mechDef.Chassis.MeleeToHitModifier);
        return stats;
    }

    internal static WeaponDefFirepowerStatistics GetDFA(MechDef mechDef)
    {
        var weaponDef = UnityGameInstance.BattleTechGame.DataManager.WeaponDefs.Get("Weapon_DFAAttack");
        var stats = new WeaponDefFirepowerStatistics(mechDef, weaponDef, null, mechDef.Chassis.DFADamage, mechDef.Chassis.DFAInstability, mechDef.Chassis.DFAToHitModifier);
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

        internal WeaponDefFirepowerStatistics(MechDef mechDef, BaseComponentRef weaponRef) :
            this(mechDef, (weaponRef.Def as WeaponDef)!, weaponRef, weaponRef.WeaponRefHelper().Damage, weaponRef.WeaponRefHelper().Instability, weaponRef.WeaponRefHelper().AccuracyModifier)
        {
        }

        internal WeaponDefFirepowerStatistics(MechDef mechDef, WeaponDef weaponDef, BaseComponentRef? weaponRef, float damage, float instability, float accuracy)
        {
            this.weaponDef = weaponDef;
            this.mechDef = mechDef;
            this.weaponRef = weaponRef;
            ShotWhenFired = GetShotsWhenFired(GetBaseShotsWhenFired);
            Damage = GetDamagePerShot(damage) * ShotWhenFired;
            HeatDamage = GetHeatDamage(GetBaseHeatDamage) * ShotWhenFired;
            StructureDamage = GetStructureDamage(GetBaseStructureDamage) * ShotWhenFired;
            Instability = GetInstability(instability) * ShotWhenFired;
            Accuracy = GetAccuracyModifier(accuracy);
        }

        private readonly WeaponDef weaponDef;
        private readonly BaseComponentRef? weaponRef;
        private readonly MechDef mechDef;
        private readonly StatCollection statCollection = new();

        private int GetBaseShotsWhenFired => weaponRef?.WeaponRefHelper().ShotsWhenFired ?? weaponDef.ShotsWhenFired;
        private float GetBaseHeatDamage => weaponRef?.WeaponRefHelper().HeatDamage ?? weaponDef.HeatDamage;
        private float GetBaseStructureDamage => weaponRef?.WeaponRefHelper().StructureDamage ?? weaponDef.StructureDamage;

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