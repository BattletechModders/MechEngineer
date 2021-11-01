using System.Linq;
using BattleTech;
using BattleTech.UI;
using CustomComponents;
using Localize;
using MechEngineer.Features.OverrideTonnage;

namespace MechEngineer.Features.MechLabSlots
{
    internal class MechLabLocationNaming
    {
        internal static void AdjustLocationNaming(MechLabLocationWidget widget, ChassisLocations location)
        {
            // just hide armor = 0 stuff
            widget.gameObject.SetActive(!widget.ShouldHide());

            var mechLab = (MechLabPanel)widget.parentDropTarget;
            var text = GetLocationName(mechLab.activeMechDef.Chassis, location);

            widget.locationName.SetText(text);
        }

        private static Text GetLocationName(ChassisDef chassisDef, ChassisLocations location)
        {
            if (chassisDef.Is<ChassisLocationNaming>(out var naming))
            {
                var text = naming.Names
                    .Where(x => x.location == location)
                    .Select(x => x.text)
                    .FirstOrDefault();

                if (text != null)
                {
                    return new Text(text);
                }
            }

            return Mech.GetLongChassisLocation(location);
        }
    }

    internal static class MechLabLocationWidgetExtensions
    {
        // hide any location with maxArmor <= 0 && structure <= 1
        // for vehicles and troopers
        internal static bool ShouldHide(this MechLabLocationWidget widget)
        {
            var def = widget.chassisLocationDef;
            return PrecisionUtils.SmallerOrEqualsTo(def.MaxArmor, 0)
                && PrecisionUtils.SmallerOrEqualsTo(def.InternalStructure, 1);
        }
    }
}