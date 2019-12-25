using System.Collections.Generic;
using System.Linq;
using BattleTech;

namespace MechEngineer.Features.Engines.Helper
{
    internal static class MechDefStatisticModifier
    {
        internal static T ModifyStatistic<T>(StatisticAdapter<T> stat, MechDef mechDef)
        {
            return ModifyStatistic(stat, mechDef.Inventory.Select(x => x.Def));
        }

        private static T ModifyStatistic<T>(StatisticAdapter<T> stat, IEnumerable<MechComponentDef> componentDefs)
        {
            foreach (var componentDef in componentDefs)
            {
                if (componentDef.statusEffects == null)
                {
                    continue;
                }

                foreach (var effectData in componentDef.statusEffects)
                {
                    if (effectData.targetingData.effectTriggerType != EffectTriggerType.Passive
                        || effectData.targetingData.effectTargetType != EffectTargetType.Creator)
                    {
                        continue;
                    }
                    if (stat.Key != effectData.statisticData.statName)
                    {
                        continue;
                    }
                    stat.Modify(effectData);
                }
            }
            return stat.Get();
        }
    }
}