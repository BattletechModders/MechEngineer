using System;
using System.Collections.Generic;
using BattleTech;
using Harmony;
using MechEngineer.Features.HardpointFix.utils;

namespace MechEngineer.Features.HardpointFix.limits.Patches
{
    [HarmonyPatch(typeof(ChassisDef), "RefreshHardpointData")]
    public static class ChassisDef_RefreshHardpointData_Patch
    {
        internal static readonly Dictionary<string, LocationDef[]> ChassisDefLocationsLocationsCache = new Dictionary<string, LocationDef[]>();

        public static void Postfix(ChassisDef __instance)
        {
            try
            {
                if (!Control.settings.HardpointFix.AutoFixChassisDefWeaponHardpointCounts)
                {
                    return;
                }

                var hardpointDataDef = __instance.HardpointDataDef;
                if (hardpointDataDef == null)
                {
                    return;
                }

                if (ChassisDefLocationsLocationsCache.ContainsKey(__instance.Description.Id))
                {
                    return;
                }

                var hardpointCounts = new Dictionary<ChassisLocations, HardpointCounter>();
                foreach (var hardpointData in hardpointDataDef.HardpointData)
                {
                    Control.Logger.Debug?.Log($"id={__instance.Description.Id} location={hardpointData.location}");
                    var location = VHLUtils.GetLocationByString(hardpointData.location);
                    if (location.HasValue)
                    {
                        var counter = new HardpointCounter(hardpointData.weapons);
                        hardpointCounts[location.Value] = counter;
                    }
                }

                var adapter = new ChassisDefAdapter(__instance);
                var chassisLocationDefs = adapter.Locations;
                for (var i = 0; i < chassisLocationDefs.Length; i++)
                {
                    var locationDef = chassisLocationDefs[i];
                    HardpointDef[] hardpointDefs;
                    if (hardpointCounts.ContainsKey(locationDef.Location))
                    {
                        var counter = hardpointCounts[locationDef.Location];
                        hardpointDefs = counter.HardpointsDefs;
                    }
                    else
                    {
                        hardpointDefs = new HardpointDef[0];
                    }

                    chassisLocationDefs[i] = new LocationDef(
                        hardpointDefs,
                        locationDef.Location,
                        locationDef.Tonnage,
                        locationDef.InventorySlots,
                        locationDef.MaxArmor,
                        locationDef.MaxRearArmor,
                        locationDef.InternalStructure
                        );
                }

                ChassisDefLocationsLocationsCache[__instance.Description.Id] = chassisLocationDefs;

                adapter.refreshLocationReferences();
            }
            catch (Exception e)
            {
                Control.Logger.Error.Log(e);
            }
        }
    }
}