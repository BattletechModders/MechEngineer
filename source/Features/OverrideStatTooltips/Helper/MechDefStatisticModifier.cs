using System.Collections.Generic;
using System.Linq;
using BattleTech;
using MechEngineer.Features.OrderedStatusEffects;
using MechEngineer.Helper;
using static BattleTech.StatisticEffectData;

namespace MechEngineer.Features.OverrideStatTooltips.Helper;

internal static class MechDefStatisticModifier
{
    internal static T ModifyStatistic<T>(StatisticAdapter<T> stat, MechDef mechDef, bool acceptAllDamageLevels = false)
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

    internal static T ModifyWeaponStatistic<T>(StatisticAdapter<T> stat, MechDef mechDef, WeaponDef weaponDef)
    {
        return ModifyWeaponStatistic<T>(stat, mechDef, weaponDef.WeaponSubType, weaponDef.Type, weaponDef.WeaponCategoryValue);
    }

    internal static T ModifyWeaponStatistic<T>(StatisticAdapter<T> stat, MechDef mechDef, WeaponSubType subType, WeaponType type, WeaponCategoryValue categoryValue)
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
                if (!IsStatusEffectAffectingWeapon(effectData.statisticData, subType, type, categoryValue))
                {
                    continue;
                }
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
    internal static bool IsStatusEffectAffectingWeapon(StatisticEffectData statisticData, WeaponSubType subType, WeaponType type, WeaponCategoryValue categoryValue)
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