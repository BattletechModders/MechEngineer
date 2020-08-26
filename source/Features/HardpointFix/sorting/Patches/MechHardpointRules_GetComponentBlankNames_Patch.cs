using BattleTech;
using Harmony;
using MechEngineer.Features.CriticalEffects.Patches;
using MechEngineer.Features.HardpointFix.utils;
using System;
using System.Collections.Generic;

namespace MechEngineer.Features.HardpointFix.sorting.Patches
{
    [HarmonyPatch(typeof(MechHardpointRules), nameof(MechHardpointRules.GetComponentBlankNames))]
    public static class MechHardpointRules_GetComponentBlankNames_Patch
    {
        private static WeaponComponentPrefabCalculator calculator => MechHardpointRules_GetComponentPrefabName_Patch.calculator;

        public static bool Prefix(ChassisLocations location, ref List<string> __result)
        {
            try
            {
                if (calculator != null)
                {
                    __result = calculator.GetRequiredBlankPrefabNamesInLocation(location);
                    return false;
                }
            }
            catch (Exception e)
            {
                Control.Logger.Error.Log(e);
            }
            return true;
        }

        public static void Postfix(ChassisLocations location, ref List<string> __result)
        {
            try
            {
                if (HardpointFixFeature.Shared.Settings.TraceLogDebugMappings || (__result != null && __result.Count > 0))
                {
                    Control.Logger.Debug?.Log($"GetComponentBlankNames blanks=[{__result?.JoinAsString()}] location={location}");
                }
            }
            catch (Exception e)
            {
                Control.Logger.Error.Log(e);
            }
        }
    }
}