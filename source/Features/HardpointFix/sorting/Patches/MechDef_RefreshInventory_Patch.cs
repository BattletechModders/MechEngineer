using System;
using System.Linq;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.HardpointFix.sorting.Patches
{
    // too slow :(
    //[HarmonyPatch(typeof(MechDef), "RefreshInventory")]
    public static class MechDef_RefreshInventory_Patch
    {
        public static void Prefix(MechDef __instance)
        {
            try
            {
                var mechDef = __instance;
                
                // missing fixed equipment :/
                MechHardpointRules_GetComponentPrefabName_Patch.SetupCalculator(
                    mechDef.Chassis,
                    mechDef.Inventory?.ToList()
                );
            }
            catch (Exception e)
            {
                Control.Logger.Error.Log(e);
            }
        }

        public static void Postfix()
        {
            MechHardpointRules_GetComponentPrefabName_Patch.ResetCalculator();
        }
    }
}