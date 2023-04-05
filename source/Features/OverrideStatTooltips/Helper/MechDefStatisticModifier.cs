using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using MechEngineer.Features.OrderedStatusEffects;
using MechEngineer.Helper;
using static BattleTech.StatisticEffectData;

namespace MechEngineer.Features.OverrideStatTooltips.Helper;

internal static class MechDefStatisticModifier
{
    private static readonly Dictionary<string, Func<MechDef, MechComponentDef, EffectData, bool>> filters = new Dictionary<string, Func<MechDef, MechComponentDef, EffectData, bool>>();
    internal static void RegisterFilter(string name, Func<MechDef, MechComponentDef, EffectData, bool> filter)
    {
        filters[name] = filter;
    }
    internal static T ModifyStatistic<T>(StatisticAdapter<T> stat, MechDef mechDef, bool acceptAllDamageLevels = false) where T : notnull
    {
        var effects = new List<EffectData>();
        foreach (var componentDef in mechDef.Inventory.Where(x => acceptAllDamageLevels || x.IsFunctionalORInstalling()).Select(x => x.Def))
        {
            if (componentDef.statusEffects == null)
            {
                continue;
            }

            foreach (var effectData in componentDef.statusEffects)
            {
                if (effectData.effectType != EffectType.StatisticEffect)
                {
                    continue;
                }
                if (effectData.targetingData.effectTriggerType != EffectTriggerType.Passive
                    || effectData.targetingData.effectTargetType != EffectTargetType.Creator)
                {
                    continue;
                }
                if (stat.Key != effectData.statisticData.statName)
                {
                    continue;
                }
                bool can_be_applied = true;
                foreach(var filter in filters)
                {
                    if (filter.Value == null) { continue; }
                    if (filter.Value(mechDef, null, effectData) == false) { can_be_applied = false; break; }
                }
                if (can_be_applied == false) { continue; }
                effects.Add(effectData);
            }
        }
        OrderedStatusEffectsFeature.Shared.SortEffectDataList(effects);
        foreach (var effect in effects)
        {
            stat.Modify(effect);
        }
        return stat.Get();
    }

    internal static T ModifyWeaponStatistic<T>(StatisticAdapter<T> stat, MechDef mechDef, WeaponDef weaponDef) where T : notnull
    {
        var effects = new List<EffectData>();
        foreach (var componentDef in mechDef.Inventory.Where(x => x.IsFunctionalORInstalling()).Select(x => x.Def))
        {
            if (componentDef.statusEffects == null)
            {
                continue;
            }

            foreach (var effectData in componentDef.statusEffects)
            {
                if (effectData.effectType != EffectType.StatisticEffect)
                {
                    continue;
                }
                if (effectData.targetingData.effectTriggerType != EffectTriggerType.Passive
                    || effectData.targetingData.effectTargetType != EffectTargetType.Creator)
                {
                    continue;
                }
                if (stat.Key != effectData.statisticData.statName)
                {
                    continue;
                }
                if (!IsStatusEffectAffectingWeapon(effectData.statisticData, weaponDef.WeaponSubType, weaponDef.Type, weaponDef.WeaponCategoryValue))
                {
                    continue;
                }
                bool can_be_applied = true;
                foreach (var filter in filters)
                {
                    if (filter.Value == null) { continue; }
                    if (filter.Value(mechDef, weaponDef, effectData) == false) { can_be_applied = false; break; }
                }
                if (can_be_applied == false) { continue; }
                effects.Add(effectData);
            }
        }
        OrderedStatusEffectsFeature.Shared.SortEffectDataList(effects);
        foreach (var effect in effects)
        {
            stat.Modify(effect);
        }
        return stat.Get();
    }

    // see EffectsManager.GetTargetComponents for order and logic
    private static bool IsStatusEffectAffectingWeapon(StatisticEffectData statisticData, WeaponSubType subType, WeaponType type, WeaponCategoryValue categoryValue)
    {
        if (statisticData.targetCollection != TargetCollection.Weapon)
        {
            return false;
        }

        if (statisticData.targetWeaponSubType != WeaponSubType.NotSet)
        {
            return statisticData.targetWeaponSubType == subType;
        }
        else if (statisticData.targetWeaponType != WeaponType.NotSet)
        {
            return statisticData.targetWeaponType == type;
        }
        else if (!statisticData.TargetWeaponCategoryValue.Is_NotSet)
        {
            return statisticData.TargetWeaponCategoryValue.ID == categoryValue.ID;
        }

        return true;
    }
}