using System.Linq;
using BattleTech;

namespace MechEngineer.Helper;

internal static class MechComponentDefExtensions
{
    internal static void AddPassiveStatisticEffectIfMissing(this MechComponentDef def, StatisticEffectData statisticData)
    {
        if (def.statusEffects != null && def.statusEffects.Any(x => statisticData.statName == x.statisticData.statName))
        {
            // passive effect already exists
            return;
        }

        var effectData = CreatePassiveEffectData(def.Description.Id, statisticData);

        var statusEffects = def.statusEffects == null ? new[] {effectData} : def.statusEffects.Append(effectData).ToArray();
        def.SetEffectData(statusEffects);
    }

    internal static EffectData CreatePassiveEffectData(string idPrefix, StatisticEffectData statisticData)
    {
        var effectData = new EffectData
        {
            effectType = EffectType.StatisticEffect,
            nature = EffectNature.Buff,
            durationData = new EffectDurationData {duration = -1, stackLimit = -1},
            targetingData = new EffectTargetingData
            {
                effectTriggerType = EffectTriggerType.Passive,
                effectTargetType = EffectTargetType.Creator
            },
            Description = new BaseDescriptionDef(
                idPrefix + "_" + statisticData.statName,
                statisticData.statName,
                "",
                null),
            statisticData = statisticData
        };
        return effectData;
    }
}