using System;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.HardpointFix.limits.Patches
{
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
                Control.Logger.Error.Log(e);
            }
        }
    }
}