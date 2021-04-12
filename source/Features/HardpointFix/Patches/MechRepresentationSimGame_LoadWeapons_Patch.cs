using System;
using System.Linq;
using BattleTech;
using Harmony;
using MechEngineer.Misc;

namespace MechEngineer.Features.HardpointFix.Patches
{
    [HarmonyPatch(typeof(MechRepresentationSimGame), "LoadWeapons")]
    public static class MechRepresentationSimGame_LoadWeapons_Patch
    {
        [HarmonyBefore(KFix.CU)]
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

        public static void Postfix()
        {
            MechHardpointRules_GetComponentPrefabName_Patch.ResetCalculator();
        }
    }
}