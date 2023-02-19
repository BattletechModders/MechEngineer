using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.OverrideStatTooltips.Helper;

internal static class WeaponRefDataHelper
{
    private delegate float d_GetStatFloat(BaseComponentRef weaponRef);
    private delegate bool d_GetStatBool(BaseComponentRef weaponRef);
    private delegate int d_GetStatInt(BaseComponentRef weaponRef);
    private delegate string d_GetStatString(BaseComponentRef weaponRef);
    private static readonly Dictionary<string, d_GetStatFloat> GetFloatStat = new();
    private static readonly Dictionary<string, d_GetStatBool> GetBoolStat = new();
    private static readonly Dictionary<string, d_GetStatInt> GetIntStat = new();
    private static readonly Dictionary<string, d_GetStatString> GetStringStat = new();
    private static readonly string ASSEMBLY_NAME = "CustomAmmoCategories, Version=";
    private static d_GetStatFloat constructFloat(MethodInfo method)
    {
        var dm = new DynamicMethod($"CAC_{method.Name}", typeof(float), new[] { typeof(BaseComponentRef) });
        var gen = dm.GetILGenerator();
        gen.Emit(OpCodes.Ldarg_0);
        gen.Emit(OpCodes.Call, method);
        gen.Emit(OpCodes.Ret);
        Log.Main.Info?.Log($" {method.Name} {method.ReturnType.Name}");
        return (d_GetStatFloat)dm.CreateDelegate(typeof(d_GetStatFloat));
    }
    private static d_GetStatBool constructBool(MethodInfo method)
    {
        var dm = new DynamicMethod($"CAC_{method.Name}", typeof(bool), new[] { typeof(BaseComponentRef) });
        var gen = dm.GetILGenerator();
        gen.Emit(OpCodes.Ldarg_0);
        gen.Emit(OpCodes.Call, method);
        gen.Emit(OpCodes.Ret);
        Log.Main.Info?.Log($" {method.Name} {method.ReturnType.Name}");
        return (d_GetStatBool)dm.CreateDelegate(typeof(d_GetStatBool));
    }
    private static d_GetStatInt constructInt(MethodInfo method)
    {
        var dm = new DynamicMethod($"CAC_{method.Name}", typeof(int), new[] { typeof(BaseComponentRef) });
        var gen = dm.GetILGenerator();
        gen.Emit(OpCodes.Ldarg_0);
        gen.Emit(OpCodes.Call, method);
        gen.Emit(OpCodes.Ret);
        Log.Main.Info?.Log($" {method.Name} {method.ReturnType.Name}");
        return (d_GetStatInt)dm.CreateDelegate(typeof(d_GetStatInt));
    }
    private static d_GetStatString constructString(MethodInfo method)
    {
        var dm = new DynamicMethod($"CAC_{method.Name}", typeof(string), new[] { typeof(BaseComponentRef) });
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
                var helperType = assembly.GetType("CustAmmoCategories.WeaponRefDataHelper");
                if (helperType == null) { continue; }
                Log.Main.Info?.Log($"{helperType.Name} found");
                FillGetDelegates(helperType);
            }
        }
    }
    public static float Damage(this BaseComponentRef weaponRef)
    {
        if (GetFloatStat.TryGetValue("Damage", out var method)) { return method(weaponRef); }
        if (weaponRef.Def is WeaponDef weaponDef) { return weaponDef.Damage; }
        throw new NullReferenceException(nameof(weaponDef));
    }
    public static float HeatDamage(this BaseComponentRef weaponRef)
    {
        if (GetFloatStat.TryGetValue("HeatDamage", out var method)) { return method(weaponRef); }
        if (weaponRef.Def is WeaponDef weaponDef) { return weaponDef.HeatDamage; }
        throw new NullReferenceException(nameof(weaponDef));
    }
    public static float StructureDamage(this BaseComponentRef weaponRef)
    {
        if (GetFloatStat.TryGetValue("StructureDamage", out var method)) { return method(weaponRef); }
        if (weaponRef.Def is WeaponDef weaponDef) { return weaponDef.StructureDamage; }
        throw new NullReferenceException(nameof(weaponDef));
    }
    public static int ShotsWhenFired(this BaseComponentRef weaponRef)
    {
        if (GetIntStat.TryGetValue("ShotsWhenFired", out var method)) { return method(weaponRef); }
        if (weaponRef.Def is WeaponDef weaponDef) { return weaponDef.ShotsWhenFired; }
        throw new NullReferenceException(nameof(weaponDef));
    }
    public static float Instability(this BaseComponentRef weaponRef)
    {
        if (GetFloatStat.TryGetValue("Instability", out var method)) { return method(weaponRef); }
        if (weaponRef.Def is WeaponDef weaponDef) { return weaponDef.Instability; }
        throw new NullReferenceException(nameof(weaponDef));
    }
    public static float AccuracyModifier(this BaseComponentRef weaponRef)
    {
        if (GetFloatStat.TryGetValue("AccuracyModifier", out var method)) { return method(weaponRef); }
        if (weaponRef.Def is WeaponDef weaponDef) { return weaponDef.AccuracyModifier; }
        throw new NullReferenceException(nameof(weaponDef));
    }
    public static float MinRange(this BaseComponentRef weaponRef)
    {
        if (GetFloatStat.TryGetValue("MinRange", out var method)) { return method(weaponRef); }
        if (weaponRef.Def is WeaponDef weaponDef) { return weaponDef.MinRange; }
        throw new NullReferenceException(nameof(weaponDef));
    }
    public static float MaxRange(this BaseComponentRef weaponRef)
    {
        if (GetFloatStat.TryGetValue("MaxRange", out var method)) { return method(weaponRef); }
        if (weaponRef.Def is WeaponDef weaponDef) { return weaponDef.MaxRange; }
        throw new NullReferenceException(nameof(weaponDef));
    }
    public static bool IndirectFireCapable(this BaseComponentRef weaponRef)
    {
        if (GetBoolStat.TryGetValue("IndirectFireCapable", out var method)) { return method(weaponRef); }
        if (weaponRef.Def is WeaponDef weaponDef) { return weaponDef.IndirectFireCapable; }
        throw new NullReferenceException(nameof(weaponDef));
    }
    public static bool CanUseInMelee(this BaseComponentRef weaponRef)
    {
        if (GetBoolStat.TryGetValue("CanUseInMelee", out var method)) { return method(weaponRef); }
        if (weaponRef.Def is WeaponDef weaponDef) { return weaponDef.WeaponCategoryValue.CanUseInMelee; }
        throw new NullReferenceException(nameof(weaponDef));
    }
    public static int HeatGenerated(this BaseComponentRef weaponRef)
    {
        if (GetIntStat.TryGetValue("HeatGenerated", out var method)) { return method(weaponRef); }
        if (weaponRef.Def is WeaponDef weaponDef) { return weaponDef.HeatGenerated; }
        throw new NullReferenceException(nameof(weaponDef));
    }
}