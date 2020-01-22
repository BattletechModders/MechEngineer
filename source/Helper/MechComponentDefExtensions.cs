using BattleTech;
using CustomComponents;
using System.Collections.Generic;
using System.Linq;

namespace MechEngineer {
  internal static class MechComponentDefExtensions {
    internal static bool HasCustomFlag(this MechComponentDef def, string flag) {
      return def.Is<Flags>(out var f) && f.IsSet(flag);
    }

    internal static bool HasComponentTag(this MechComponentDef def, string tag) {
      return def.ComponentTags.Contains(tag);
    }

    internal static void AddPassiveStatisticEffectIfMissing(this MechComponentDef def, StatisticEffectData statisticData) {
      if (def.statusEffects != null && def.statusEffects.Any(x => statisticData.statName == x.statisticData.statName)) {
        // passive effect already exists
        return;
      }

      var effectData = new EffectData {
        effectType = EffectType.StatisticEffect,
        nature = EffectNature.Buff
      };

      effectData.durationData = new EffectDurationData {
        duration = -1,
        stackLimit = -1
      };

      effectData.targetingData = new EffectTargetingData {
        effectTriggerType = EffectTriggerType.Passive,
        effectTargetType = EffectTargetType.Creator
      };

      var id = def.Description.Id + "_" + statisticData.statName;
      effectData.Description = new BaseDescriptionDef(id, statisticData.statName, "", null);
      effectData.statisticData = statisticData;
      EffectData[] statusEffects = null;
      if (def.statusEffects == null) {
        statusEffects = new[] { effectData };
      } else {
        List<EffectData> tmp = new List<EffectData>();
        tmp.AddRange(def.statusEffects);
        tmp.Add(effectData);
        statusEffects = tmp.ToArray();
      }
      //var statusEffects = def.statusEffects == null ? new[] { effectData } : def.statusEffects.Append(effectData).ToArray();
      def.SetEffectData(statusEffects);
    }
  }
}