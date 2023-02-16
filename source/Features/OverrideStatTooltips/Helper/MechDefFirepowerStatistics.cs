using BattleTech;
using System;
using System.Linq;
using MechEngineer.Helper;
using UnityEngine;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MechEngineer.Features.OverrideStatTooltips.Helper;
internal static class WeaponRefDataHelper
{
    private delegate float d_GetStatFloat(BaseComponentRef weaponRef);
    private delegate bool d_GetStatBool(BaseComponentRef weaponRef);
    private delegate int d_GetStatInt(BaseComponentRef weaponRef);
    private delegate string d_GetStatString(BaseComponentRef weaponRef);
    private static readonly Dictionary<string, d_GetStatFloat> GetFloatStat = new Dictionary<string, d_GetStatFloat>();
    private static readonly Dictionary<string, d_GetStatBool> GetBoolStat = new Dictionary<string, d_GetStatBool>();
    private static readonly Dictionary<string, d_GetStatInt> GetIntStat = new Dictionary<string, d_GetStatInt>();
    private static readonly Dictionary<string, d_GetStatString> GetStringStat = new Dictionary<string, d_GetStatString>();
    private static readonly string ASSEMBLY_NAME = "CustomAmmoCategories, Version=";
    private static d_GetStatFloat constructFloat(MethodInfo method)
    {
        var dm = new DynamicMethod($"CAC_{method.Name}", typeof(float), new Type[] { typeof(BattleTech.BaseComponentRef) });
        var gen = dm.GetILGenerator();
        gen.Emit(OpCodes.Ldarg_0);
        gen.Emit(OpCodes.Call, method);
        gen.Emit(OpCodes.Ret);
        Log.Main.Info?.Log($" {method.Name} {method.ReturnType.Name}");
        return (d_GetStatFloat)dm.CreateDelegate(typeof(d_GetStatFloat));
    }
    private static d_GetStatBool constructBool(MethodInfo method)
    {
        var dm = new DynamicMethod($"CAC_{method.Name}", typeof(bool), new Type[] { typeof(BattleTech.BaseComponentRef) });
        var gen = dm.GetILGenerator();
        gen.Emit(OpCodes.Ldarg_0);
        gen.Emit(OpCodes.Call, method);
        gen.Emit(OpCodes.Ret);
        Log.Main.Info?.Log($" {method.Name} {method.ReturnType.Name}");
        return (d_GetStatBool)dm.CreateDelegate(typeof(d_GetStatBool));
    }
    private static d_GetStatInt constructInt(MethodInfo method)
    {
        var dm = new DynamicMethod($"CAC_{method.Name}", typeof(int), new Type[] { typeof(BattleTech.BaseComponentRef) });
        var gen = dm.GetILGenerator();
        gen.Emit(OpCodes.Ldarg_0);
        gen.Emit(OpCodes.Call, method);
        gen.Emit(OpCodes.Ret);
        Log.Main.Info?.Log($" {method.Name} {method.ReturnType.Name}");
        return (d_GetStatInt)dm.CreateDelegate(typeof(d_GetStatInt));
    }
    private static d_GetStatString constructString(MethodInfo method)
    {
        var dm = new DynamicMethod($"CAC_{method.Name}", typeof(string), new Type[] { typeof(BattleTech.BaseComponentRef) });
        var gen = dm.GetILGenerator();
        gen.Emit(OpCodes.Ldarg_0);
        gen.Emit(OpCodes.Call, method);
        gen.Emit(OpCodes.Ret);
        Log.Main.Info?.Log($" {method.Name} {method.ReturnType.Name}");
        return (d_GetStatString)dm.CreateDelegate(typeof(d_GetStatString));
    }
    public static void FillGetDelegates(Type helperType)
    {
        var methods = AccessTools.GetDeclaredMethods(helperType);
        foreach (var method in methods)
        {
            var parameters = method.GetParameters();
            if (parameters.Length != 1) { continue; }
            if (parameters[0].ParameterType != typeof(BaseComponentRef)) { continue; }
            if (method.ReturnType == typeof(float))
            {
                GetFloatStat[method.Name] = constructFloat(method);
            }
            else if (method.ReturnType == typeof(int)) 
            {
                GetIntStat[method.Name] = constructInt(method);
            }
            else if (method.ReturnType == typeof(bool))
            {
                GetBoolStat[method.Name] = constructBool(method);
            }
            else if (method.ReturnType == typeof(string))
            {
                GetStringStat[method.Name] = constructString(method);
            }
        }
    }
    public static void InitHelper()
    {
        foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (assembly.FullName.StartsWith(ASSEMBLY_NAME))
            {
                Type helperType = assembly.GetType("CustAmmoCategories.WeaponRefDataHelper");
                if (helperType == null) { continue; }
                Log.Main.Info?.Log($"{helperType.Name} found");
                FillGetDelegates(helperType);
            }
        }
    }
    public static float Damage(this BaseComponentRef weaponRef)
    {
        if (weaponRef == null) { return 0f; }
        if (GetFloatStat.TryGetValue("Damage", out var method)) { return method(weaponRef); }
        if (weaponRef.Def is WeaponDef weaponDef) { return weaponDef.Damage; }
        return 0f;
    }
    public static float HeatDamage(this BaseComponentRef weaponRef)
    {
        if (weaponRef == null) { return 0f; }
        if (GetFloatStat.TryGetValue("HeatDamage", out var method)) { return method(weaponRef); }
        if (weaponRef.Def is WeaponDef weaponDef) { return weaponDef.HeatDamage; }
        return 0f;
    }
    public static float StructureDamage(this BaseComponentRef weaponRef)
    {
        if (weaponRef == null) { return 0f; }
        if (GetFloatStat.TryGetValue("StructureDamage", out var method)) { return method(weaponRef); }
        if (weaponRef.Def is WeaponDef weaponDef) { return weaponDef.StructureDamage; }
        return 0f;
    }
    public static int ShotsWhenFired(this BaseComponentRef weaponRef)
    {
        if (weaponRef == null) { return 0; }
        if (GetIntStat.TryGetValue("ShotsWhenFired", out var method)) { return method(weaponRef); }
        if (weaponRef.Def is WeaponDef weaponDef) { return weaponDef.ShotsWhenFired; }
        return 0;
    }
    public static float Instability(this BaseComponentRef weaponRef)
    {
        if (weaponRef == null) { return 0f; }
        if (GetFloatStat.TryGetValue("Instability", out var method)) { return method(weaponRef); }
        if (weaponRef.Def is WeaponDef weaponDef) { return weaponDef.Instability; }
        return 0f;
    }
    public static float AccuracyModifier(this BaseComponentRef weaponRef)
    {
        if (weaponRef == null) { return 0f; }
        if (GetFloatStat.TryGetValue("AccuracyModifier", out var method)) { return method(weaponRef); }
        if (weaponRef.Def is WeaponDef weaponDef) { return weaponDef.AccuracyModifier; }
        return 0f;
    }
    public static float MinRange(this BaseComponentRef weaponRef)
    {
        if (weaponRef == null) { return 0f; }
        if (GetFloatStat.TryGetValue("MinRange", out var method)) { return method(weaponRef); }
        if (weaponRef.Def is WeaponDef weaponDef) { return weaponDef.MinRange; }
        return 0f;
    }
    public static float MaxRange(this BaseComponentRef weaponRef)
    {
        if (weaponRef == null) { return 0f; }
        if (GetFloatStat.TryGetValue("MaxRange", out var method)) { return method(weaponRef); }
        if (weaponRef.Def is WeaponDef weaponDef) { return weaponDef.MaxRange; }
        return 0f;
    }
    public static bool IndirectFireCapable(this BaseComponentRef weaponRef)
    {
        if (weaponRef == null) { return false; }
        if (GetBoolStat.TryGetValue("IndirectFireCapable", out var method)) { return method(weaponRef); }
        if (weaponRef.Def is WeaponDef weaponDef) { return weaponDef.IndirectFireCapable; }
        return false;
    }
    public static bool CanUseInMelee(this BaseComponentRef weaponRef)
    {
        if (weaponRef == null) { return false; }
        if (GetBoolStat.TryGetValue("CanUseInMelee", out var method)) { return method(weaponRef); }
        if (weaponRef.Def is WeaponDef weaponDef) { return weaponDef.WeaponCategoryValue.CanUseInMelee; }
        return false;
    }
    public static int HeatGenerated(this BaseComponentRef weaponRef)
    {
        if (weaponRef == null) { return 0; }
        if (GetIntStat.TryGetValue("HeatGenerated", out var method)) { return method(weaponRef); }
        if (weaponRef.Def is WeaponDef weaponDef) { return weaponDef.HeatGenerated; }
        return 0;
    }
}

internal class MechDefFirepowerStatistics
{
    internal float TotalDamage { get; }
    internal float TotalHeatDamage { get; }
    internal float TotalStructureDamage { get; }
    internal float TotalInstability { get; }
    internal float AverageAccuracy { get; }

    internal MechDefFirepowerStatistics(MechDef mechDef, int minRange, int maxRange)
        : this(mechDef, (x) => RangeOverlap(minRange, maxRange, Mathf.RoundToInt(x.MinRange()), Mathf.RoundToInt(x.MaxRange())))
    {
    }

    internal MechDefFirepowerStatistics(MechDef mechDef, Func<BaseComponentRef, bool> customFilter)
    {
        var weaponStats = mechDef.Inventory
            .Where(x => x.IsFunctionalORInstalling())
            .Where(x => (x.Def as WeaponDef != null))
            .Where(x => x != null)
            .Select(x => x!)
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

        internal WeaponDefFirepowerStatistics(MechDef mechDef, WeaponDef weaponDef) :
            this(mechDef, weaponDef, null, weaponDef.Damage, weaponDef.Instability, weaponDef.AccuracyModifier)
        {
        }
        internal WeaponDefFirepowerStatistics(MechDef mechDef, BaseComponentRef weaponRef) :
            this(mechDef, weaponRef.Def as WeaponDef, weaponRef, weaponRef.Damage(), weaponRef.Instability(), weaponRef.AccuracyModifier())
        {
        }

        internal int ref_ShotsWhenFired
        {
            get { return weaponRef == null ? weaponDef.ShotsWhenFired : weaponRef.ShotsWhenFired(); }
        }
        internal float ref_HeatDamage
        {
            get { return weaponRef == null ? weaponDef.HeatDamage : weaponRef.HeatDamage(); }
        }
        internal float ref_StructureDamage
        {
            get { return weaponRef == null ? weaponDef.StructureDamage : weaponRef.StructureDamage(); }
        }

        internal WeaponDefFirepowerStatistics(MechDef mechDef, WeaponDef weaponDef, BaseComponentRef weaponRef, float damage, float instability, float accuracy)
        {
            this.weaponDef = weaponDef;
            this.mechDef = mechDef;
            this.weaponRef = weaponRef;
            ShotWhenFired = GetShotsWhenFired(this.ref_ShotsWhenFired);
            Damage = GetDamagePerShot(damage) * ShotWhenFired;
            HeatDamage = GetHeatDamage(this.ref_HeatDamage) * ShotWhenFired;
            StructureDamage = GetStructureDamage(this.ref_StructureDamage) * ShotWhenFired;
            Instability = GetInstability(instability) * ShotWhenFired;
            Accuracy = GetAccuracyModifier(accuracy);
        }

        private readonly WeaponDef weaponDef;
        private readonly BaseComponentRef weaponRef;
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