using BattleTech;

namespace MechEngineer.Features.LocationalEffects
{
    internal class MechLocationNaming : LocationNaming
    {
        internal const string SidePlaceholder = "{side}";
        private readonly ChassisLocations location;

        internal MechLocationNaming(ChassisLocations location)
        {
            this.location = location;
        }

        internal string LocationalStatisticName(string statisticName)
        {
            // this is how Structure and Armor is looked up in Mech initstats
            return InterpolateStatisticName($"{{location}}.{statisticName}");
        }

        internal override string InterpolateEffectId(string id)
        {
            return id.Replace(LocationPlaceholder, LocationId);
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

        internal override string LocationId => location.ToString();

        internal override string LocationName
        {
            get
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
                        return LocationId;
                }
            }
        }

        private string SideName
        {
            get
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
                        return LocationId;
                }
            }
        }
    }
}