﻿using System.Linq;
using BattleTech;
using BattleTech.UI;
using CustomComponents;
using Localize;
using MechEngineer.Helper;
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
        // locations not used by vehicles in mechlab use maxarmor = 0
        internal static bool ShouldHide(this MechLabLocationWidget widget)
        {
            return Mathf.Approximately(widget.maxArmor, 0);
        }
    }
}