using System;
using BattleTech;
using MechEngineer.Features.DynamicSlots;

namespace MechEngineer.Features.AccuracyEffects;

internal class AccuracyEffectsFeature : Feature<AccuracyEffectsSettings>
{
    internal static readonly AccuracyEffectsFeature Shared = new();

    internal override AccuracyEffectsSettings Settings => Control.Settings.AccuracyEffects;

    internal static void SetupAccuracyStatistics(StatCollection statCollection)
    {
        foreach (var location in LocationUtils.Locations)
        {
            var statName = GetStatNameForChassisLocation(location);
            statCollection.AddStatistic(statName, 0.0f);
        }
    }

    internal static float AccuracyForLocation(StatCollection statCollection, ChassisLocations location)
    {
        var statName = GetStatNameForChassisLocation(location);
        return statCollection.GetStatistic(statName).Value<float>();
    }

    // this is similar to how Structure and Armor is looked up, see Mech.InitStats
    private static string GetStatNameForChassisLocation(ChassisLocations location)
    {
        return location switch
        {
            ChassisLocations.Head => "Head.Accuracy",
            ChassisLocations.LeftArm => "LeftArm.Accuracy",
            ChassisLocations.LeftTorso => "LeftTorso.Accuracy",
            ChassisLocations.CenterTorso => "CenterTorso.Accuracy",
            ChassisLocations.RightTorso => "RightTorso.Accuracy",
            ChassisLocations.RightArm => "RightArm.Accuracy",
            ChassisLocations.LeftLeg => "LeftLeg.Accuracy",
            ChassisLocations.RightLeg => "RightLeg.Accuracy",
            _ => throw new ArgumentException()
        };
    }
}