using System;
using BattleTech;

namespace MechEngineer
{
    public static class LocationalEffects
    {
        internal static string LocationalStatisticName(string statisticName, ChassisLocations location)
        {
            // this is how Structure and Armor is looked up in Mech initstats
            return $"{location}.{statisticName}";
        }

        internal static EffectData ProcessLocationalEffectData(EffectData effect, MechComponent mechComponent)
        {
            const string placeholderPrefix = "{Location}.";
                
            if (mechComponent.parent is Mech
                && effect.effectType == EffectType.StatisticEffect
                && effect.statisticData.statName.StartsWith(placeholderPrefix))
            {
                var locationValue = $"{mechComponent.mechComponentRef.MountedLocation}.";
                    
                var data = effect.ToJSON();
                effect = new EffectData();
                effect.FromJSON(data);
                effect.statisticData.statName =
                    effect.statisticData.statName
                        .Replace(placeholderPrefix, locationValue);
            }

            return effect;
        }
    }
}
