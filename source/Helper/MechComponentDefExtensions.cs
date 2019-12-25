using BattleTech;
using CustomComponents;
using System.Linq;

namespace MechEngineer
{
    internal static class MechComponentDefExtensions
    {
        internal static bool HasCustomFlag(this MechComponentDef def, string flag)
        {
            return def.Is<Flags>(out var f) && f.IsSet(flag);
        }

        internal static bool HasComponentTag(this MechComponentDef def, string tag)
        {
            return def.ComponentTags.Contains(tag);
        }

        internal static void AddPassiveStatisticEffect(this MechComponentDef def, StatisticEffectData statisticData)
        {
            var effectData = new EffectData
            {
                effectType = EffectType.StatisticEffect,
                nature = EffectNature.Buff
            };

            effectData.durationData = new EffectDurationData
            {
                duration = -1,
                stackLimit = -1
            };
             
            effectData.targetingData = new EffectTargetingData
            {
                effectTriggerType = EffectTriggerType.Passive,
                effectTargetType = EffectTargetType.Creator
            };
            
            var id = def.Description.Id + "_" + statisticData.statName;
            effectData.Description = new BaseDescriptionDef(id, statisticData.statName, "", null);
            effectData.statisticData = statisticData;

            var statusEffects = def.statusEffects == null ? new[] { effectData } : def.statusEffects.Append(effectData).ToArray();
            def.SetEffectData(statusEffects);
        }
    }
}