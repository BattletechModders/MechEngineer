using System.Collections.Generic;
using System.Linq;
using BattleTech;
using MechEngineer.Features.OrderedStatusEffects;

namespace MechEngineer.Features.Engines.Helper
{
    internal static class MechDefStatisticModifier
    {
        internal static T ModifyStatistic<T>(StatisticAdapter<T> stat, MechDef mechDef)
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
    }
}