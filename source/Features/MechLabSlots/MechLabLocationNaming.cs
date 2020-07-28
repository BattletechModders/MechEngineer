using BattleTech.UI;
using System.Linq;
using BattleTech;
using CustomComponents;
using Localize;
using UnityEngine;

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

            var adapter = new MechLabLocationWidgetAdapter(widget);
            adapter.locationName.SetText(text);
        }

        static Text GetLocationName(ChassisDef chassisDef, ChassisLocations location)
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
        // locations not used by vehicles in mechlab use maxarmor = 0
        internal static bool ShouldHide(this MechLabLocationWidget widget)
        {
            return Mathf.Approximately(widget.maxArmor, 0);
        }
    }
}