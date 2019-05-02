using BattleTech;

namespace MechEngineer.Features.LocationalEffects
{
    internal abstract class LocationNaming
    {
        protected const string LocationPlaceholder = "{location}";

        internal abstract string LocationId { get; }
        internal abstract string LocationName { get; }

        internal abstract string InterpolateEffectId(string id);
        internal abstract string InterpolateStatisticName(string id);
        internal abstract string InterpolateText(string text);

        internal static bool Localize(string id, MechComponent mechComponent, out string localizedId)
        {
            if (IsLocational(id) && Create(mechComponent, out var naming))
            {
                localizedId = naming.InterpolateEffectId(id);
                return true;
            }
            localizedId = null;
            return false;
        }

        internal static bool IsLocational(string text)
        {
            return text.Contains(LocationPlaceholder);
        }

        internal static bool Create(MechComponent mechComponent, out LocationNaming naming)
        {
            if (mechComponent.parent is Mech)
            {
                naming = new MechLocationNaming(mechComponent.mechComponentRef.MountedLocation);
                return true;
            }

            if (mechComponent.parent is Vehicle)
            {
                naming = new VehicleLocationNaming(mechComponent.vehicleComponentRef.MountedLocation);
                return true;
            }

            naming = null;
            return false;
        }
    }
}