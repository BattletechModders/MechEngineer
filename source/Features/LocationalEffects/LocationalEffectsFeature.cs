using System;
using BattleTech;
using MechEngineer.Features.LocationalEffects.Patches;

namespace MechEngineer.Features.LocationalEffects
{
    internal class LocationalEffectsFeature : Feature
    {
        internal static LocationalEffectsFeature Shared = new LocationalEffectsFeature();

        internal override bool Enabled => Control.settings.FeatureAccuracyEffectsEnabled || Control.settings.FeatureCriticalEffectsEnabled;

        internal override Type[] Patches => new[]
        {
            typeof(MechComponent_ApplyPassiveEffectToTarget_Patch)
        };

        internal static string LocationalStatisticName(string statisticName, ChassisLocations location)
        {
            // this is how Structure and Armor is looked up in Mech initstats
            return InterpolateStatisticName($"{{location}}.{statisticName}", location);
        }

        internal static bool ProcessLocationalEffectData(ref EffectData effect, MechComponent mechComponent)
        {
                
            if (mechComponent.parent is Mech
                && effect.effectType == EffectType.StatisticEffect
                && effect.Description.Id.Contains(LocationPlaceholder))
            {
                var data = effect.ToJSON();
                effect = new EffectData();
                effect.FromJSON(data);

                var location = mechComponent.mechComponentRef.MountedLocation;

                Control.mod.Logger.LogDebug($"Replacing location in {effect.Description.Id} with {location}");

                effect.statisticData.statName = InterpolateStatisticName(effect.statisticData.statName, location);
                
                effect.Description = new BaseDescriptionDef(
                    InterpolateEffectId(effect.Description.Id, location),
                    InterpolateText(effect.Description.Name, location),
                    InterpolateText(effect.Description.Details, location),
                    InterpolateText(effect.Description.Icon, location)
                );

                return true;
            }

            return false;
        }

        const string LocationPlaceholder = "{location}";

        internal static string InterpolateEffectId(string id, ChassisLocations location)
        {
            // the effect id is used independent of EffectData
            return id.Replace(LocationPlaceholder, location.ToString());
        }

        internal static string InterpolateStatisticName(string id, ChassisLocations location)
        {
            return id.Replace(LocationPlaceholder, location.ToString());
        }
        
        const string SidePlaceholder = "{side}";

        internal static string InterpolateText(string text, ChassisLocations location)
        {
            return text
                .Replace(LocationPlaceholder, GetLocationName(location))
                .Replace(SidePlaceholder, GetSideName(location));
        }

        internal static string GetLocationName(ChassisLocations location)
        {
            switch (location)
            {
                case ChassisLocations.LeftArm:
                    return "left arm";
                case ChassisLocations.LeftLeg:
                    return "left leg";
                case ChassisLocations.LeftTorso:
                    return "left torso";
                case ChassisLocations.RightArm:
                    return "right arm";
                case ChassisLocations.RightLeg:
                    return "right leg";
                case ChassisLocations.RightTorso:
                    return "right torso";
                case ChassisLocations.CenterTorso:
                    return "center torso";
                case ChassisLocations.Head:
                    return "head";
                default:
                    return "ERROR";
            }
        }

        internal static string GetSideName(ChassisLocations location)
        {
            switch (location)
            {
                case ChassisLocations.LeftArm:
                case ChassisLocations.LeftLeg:
                case ChassisLocations.LeftTorso:
                    return "left";
                case ChassisLocations.RightArm:
                case ChassisLocations.RightLeg:
                case ChassisLocations.RightTorso:
                    return "right";
                case ChassisLocations.CenterTorso:
                    return "center";
                case ChassisLocations.Head:
                    return "head";
                default:
                    return "ERROR";
            }
        }
    }
}
