using BattleTech;

namespace MechEngineer.Features.PlaceholderEffects;

internal class MechPlaceholderInterpolation : PlaceholderInterpolation
{
    private const string SidePlaceholder = "{side}";

    private readonly ChassisLocations Location;

    internal MechPlaceholderInterpolation(MechComponent mechComponent)
    {
        MechComponent = mechComponent;
        Location = mechComponent.mechComponentRef.MountedLocation;
    }

    internal MechPlaceholderInterpolation(ChassisLocations location)
    {
        Location = location;
    }

    internal string LocationalStatisticName(string statisticName)
    {
        // this is how Structure and Armor is looked up in Mech initstats
        return InterpolateStatisticName($"{{location}}.{statisticName}");
    }

    internal override string InterpolateEffectId(string id)
    {
        return base.InterpolateEffectId(id).Replace(LocationPlaceholder, LocationId);
    }

    internal override string InterpolateStatisticName(string id)
    {
        return id.Replace(LocationPlaceholder, LocationId);
    }

    internal override string InterpolateText(string text)
    {
        return text
            .Replace(LocationPlaceholder, LocationName)
            .Replace(SidePlaceholder, SideName);
    }

    internal override string LocationId => Location switch
    {
        ChassisLocations.Head => "Head",
        ChassisLocations.LeftArm => "LeftArm",
        ChassisLocations.LeftTorso => "LeftTorso",
        ChassisLocations.CenterTorso => "CenterTorso",
        ChassisLocations.RightTorso => "RightTorso",
        ChassisLocations.RightArm => "RightArm",
        ChassisLocations.LeftLeg => "LeftLeg",
        ChassisLocations.RightLeg => "RightLeg",
        _ => "LocationId"
    };

    private string LocationName => Location switch
    {
        ChassisLocations.Head => "head",
        ChassisLocations.LeftArm => "left arm",
        ChassisLocations.LeftTorso => "left torso",
        ChassisLocations.RightArm => "right arm",
        ChassisLocations.RightTorso => "right torso",
        ChassisLocations.CenterTorso => "center torso",
        ChassisLocations.LeftLeg => "left leg",
        ChassisLocations.RightLeg => "right leg",
        _ => "LocationName"
    };

    private string SideName => Location switch
    {
        ChassisLocations.Head => "head",
        ChassisLocations.LeftArm or ChassisLocations.LeftLeg or ChassisLocations.LeftTorso => "left",
        ChassisLocations.RightArm or ChassisLocations.RightLeg or ChassisLocations.RightTorso => "right",
        ChassisLocations.CenterTorso => "center",
        _ => "SideName"
    };
}