using System;
using System.Linq;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.HardpointFix.sorting.Patches
{
    [HarmonyPatch(typeof(MechRepresentationSimGame), "LoadWeapons")]
    public static class MechRepresentationSimGame_LoadWeapons_Patch
    {
        public static void Prefix(MechRepresentationSimGame __instance)
        {
            try
            {
                MechHardpointRules_GetComponentPrefabName_Patch.SetupCalculator(
                    __instance.mechDef?.Chassis,
                    __instance.mechDef?.Inventory?.ToList());
            }
            catch (Exception e)
            {
                Control.Logger.Error.Log(e);
            }
        }

        public static void Postfix(MechRepresentationSimGame __instance)
        {
            MechHardpointRules_GetComponentPrefabName_Patch.ResetCalculator();
        }
    }
}