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

    internal override string LocationId => Location.ToString();

    private string LocationName
    {
        get
        {
            switch (Location)
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
                    return LocationId;
            }
        }
    }

    private string SideName
    {
        get
        {
            switch (Location)
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
                    return LocationId;
            }
        }
    }
}