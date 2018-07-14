using System;
using System.Collections.Generic;
using System.Reflection;
using BattleTech;
using Harmony;

namespace MechEngineer
{
    internal class ChassisDefAdapter : Adapter<ChassisDef>
    {
        internal ChassisDefAdapter(ChassisDef instance) : base(instance)
        {
        }

        internal LocationDef[] Locations
        {
            get => traverse.Field("Locations").GetValue<LocationDef[]>();
            set => traverse.Field("Locations").SetValue(value);
        }

        internal void refreshLocationReferences()
        {
            traverse.Method("refreshLocationReferences").GetValue();
        }
    }

    [HarmonyPatch(typeof(ChassisDef), "refreshLocationReferences")]
    public static class ChassisDef_refreshLocationReferences_Patch
    {
        public static void Prefix(ChassisDef __instance)
        {
            try
            {
                if (!Control.settings.HardpointFix.AutoFixChassisDefWeaponHardpointCounts)
                {
                    return;
                }

                if (ChassisDef_RefreshHardpointData_Patch.ChassisDefLocationsLocationsCache.TryGetValue(__instance.Description.Id, out var locations))
                {
                    var adapter = new ChassisDefAdapter(__instance);
                    adapter.Locations = locations;
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }

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
                    var location = VHLUtils.GetLocationByString(hardpointData.location);
                    var counter = new HardpointCounter(hardpointData.weapons);
                    hardpointCounts[location] = counter;
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
                Control.mod.Logger.LogError(e);
            }
        }
    }
}