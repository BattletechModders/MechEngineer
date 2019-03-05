using System;
using BattleTech;
using CustomComponents;
using UnityEngine;

namespace MechEngineer
{
    public static class CriticalEffectsMechComponentExtensions
    {
        internal static int CriticalSlots(this MechComponent mechComponent)
        {
            if (mechComponent.DamageLevel == ComponentDamageLevel.Destroyed)
            {
                return 0;
            }

            var max = mechComponent.CriticalSlotsMax();
            var hits = mechComponent.CriticalSlotsHit();
            return Mathf.Max(max - hits,0);
        }

        internal static int CriticalSlotsMax(this MechComponent mechComponent)
        {
            return mechComponent.componentDef.InventorySize;
        }

        private const string HitsStatisticName = "MECriticalSlotsHit";

        internal static int CriticalSlotsHit(this MechComponent mechComponent, int? setHits = null)
        {
            if (!mechComponent.mechComponentRef.Is<CriticalEffects>(out var ce))
            {
                switch (mechComponent.DamageLevel)
                {
                    case ComponentDamageLevel.Destroyed:
                        return mechComponent.CriticalSlotsMax();
                    case ComponentDamageLevel.Penalized:
                        return 1;
                    default:
                        return 0;
                }
            }

            var statisticName = HitsStatisticName;
            var collection = mechComponent.StatCollection;
            var critStat = collection.GetStatistic(HitsStatisticName);
            if (setHits.HasValue)
            {
                if (critStat == null)
                {
                    critStat = collection.AddStatistic(statisticName, setHits.Value);
                }
                else
                {
                    critStat.SetValue(setHits.Value);
                }
            }

            return critStat?.Value<int>() ?? 0;
        }

        internal static int CriticalSlotsHitLinked(this MechComponent mechComponent, int? setHits = null)
        {
            if (!mechComponent.mechComponentRef.Is<CriticalEffects>(out var ce))
            {
                throw new Exception("should not happen");
            }

            if (!ce.HasLinked)
            {
                throw new Exception("shouldn't really happen too");
            }

            var statisticName = mechComponent.ScopedId(ce.LinkedStatisticName, true);
            var collection = mechComponent.parent.StatCollection;

            var critStat = collection.GetStatistic(statisticName) ?? collection.AddStatistic(statisticName, 0);
            if (setHits.HasValue)
            {
                critStat.SetValue(setHits.Value);
            }
            return critStat?.Value<int>() ?? 0;
        }

        internal static string ScopedId(this MechComponent mechComponent, string id, bool isLinked)
        {
            var scopeId = LocationalEffects.InterpolateEffectId(id, mechComponent.mechComponentRef.MountedLocation);
            if (scopeId == id && !isLinked)
            {
                var uid = mechComponent.uid;
                return $"{id}_{uid}";
            }
            return scopeId;
        }
    }
}