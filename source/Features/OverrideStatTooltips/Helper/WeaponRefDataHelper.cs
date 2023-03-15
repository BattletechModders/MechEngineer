using System;
using System.Linq;
using BattleTech;

namespace MechEngineer.Features.OverrideStatTooltips.Helper;

internal readonly struct WeaponRefDataHelper
{
    private readonly BaseComponentRef _componentRef;
    internal WeaponRefDataHelper(BaseComponentRef componentRef)
    {
        _componentRef = componentRef;
    }

    internal float Damage => s_delegate.Damage(_componentRef);
    internal float HeatDamage => s_delegate.HeatDamage(_componentRef);
    internal float StructureDamage => s_delegate.StructureDamage(_componentRef);
    internal int ShotsWhenFired => s_delegate.ShotsWhenFired(_componentRef);
    internal float Instability => s_delegate.Instability(_componentRef);
    internal float AccuracyModifier => s_delegate.AccuracyModifier(_componentRef);
    internal float MinRange => s_delegate.MinRange(_componentRef);
    internal float MaxRange => s_delegate.MaxRange(_componentRef);
    internal bool IndirectFireCapable => s_delegate.IndirectFireCapable(_componentRef);
    internal bool CanUseInMelee => s_delegate.CanUseInMelee(_componentRef);
    internal int HeatGenerated => s_delegate.HeatGenerated(_componentRef);

    internal static void InitHelper()
    {
        var assemblyName = "CustomAmmoCategories";
        var typeName = "CustAmmoCategories.WeaponRefDataHelper";

        var cacHelperType = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(assembly => assembly.GetName().Name == assemblyName)
            .Select(assembly => assembly.GetType(typeName))
            .SingleOrDefault(type => type != null);

        if (cacHelperType != null)
        {
            Log.Main.Info?.Log($"{typeName} found");
            s_delegate = new(cacHelperType);
        }
    }

    // creating a dynamic class would run faster than Func delegates
    private static DynamicCacWeaponRefDataHelper s_delegate = new(null);
    private readonly struct DynamicCacWeaponRefDataHelper
    {
        internal DynamicCacWeaponRefDataHelper(Type? cacType)
        {
            Damage = FindMethod(cacType, nameof(Damage), componentRef => ToWeaponDef(componentRef).Damage);
            HeatDamage = FindMethod(cacType, nameof(HeatDamage), componentRef => ToWeaponDef(componentRef).HeatDamage);
            StructureDamage = FindMethod(cacType, nameof(StructureDamage), componentRef => ToWeaponDef(componentRef).StructureDamage);
            ShotsWhenFired = FindMethod(cacType, nameof(ShotsWhenFired), componentRef => ToWeaponDef(componentRef).ShotsWhenFired);
            Instability = FindMethod(cacType, nameof(Instability), componentRef => ToWeaponDef(componentRef).Instability);
            AccuracyModifier = FindMethod(cacType, nameof(AccuracyModifier), componentRef => ToWeaponDef(componentRef).AccuracyModifier);
            MinRange = FindMethod(cacType, nameof(MinRange), componentRef => ToWeaponDef(componentRef).MinRange);
            MaxRange = FindMethod(cacType, nameof(MaxRange), componentRef => ToWeaponDef(componentRef).MaxRange);
            IndirectFireCapable = FindMethod(cacType, nameof(IndirectFireCapable), componentRef => ToWeaponDef(componentRef).IndirectFireCapable);
            CanUseInMelee = FindMethod(cacType, nameof(CanUseInMelee), componentRef => ToWeaponDef(componentRef).WeaponCategoryValue.CanUseInMelee);
            HeatGenerated = FindMethod(cacType, nameof(HeatGenerated), componentRef => ToWeaponDef(componentRef).HeatGenerated);
        }

        internal readonly Func<BaseComponentRef, float> Damage;
        internal readonly Func<BaseComponentRef, float> HeatDamage;
        internal readonly Func<BaseComponentRef, float> StructureDamage;
        internal readonly Func<BaseComponentRef, int> ShotsWhenFired;
        internal readonly Func<BaseComponentRef, float> Instability;
        internal readonly Func<BaseComponentRef, float> AccuracyModifier;
        internal readonly Func<BaseComponentRef, float> MinRange;
        internal readonly Func<BaseComponentRef, float> MaxRange;
        internal readonly Func<BaseComponentRef, bool> IndirectFireCapable;
        internal readonly Func<BaseComponentRef, bool> CanUseInMelee;
        internal readonly Func<BaseComponentRef, int> HeatGenerated;

        private static Func<BaseComponentRef, R> FindMethod<R>(Type? cacType, string name, Func<BaseComponentRef, R> fallback)
        {
            return MethodFinder.FindStatic(cacType, name, fallback);
        }

        private static WeaponDef ToWeaponDef(BaseComponentRef componentRef)
        {
            return (WeaponDef)componentRef.Def;
        }
    }
}