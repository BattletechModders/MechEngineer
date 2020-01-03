using System;
using System.Collections.Generic;
using BattleTech;
using Harmony;
using MechEngineer.Features.HardpointFix.utils;

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
                if (calculator == null)
                {
                    return true;
                }

                __result = calculator.GetRequiredBlankPrefabNamesInLocation(location);
                return false;
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
            return true;
        }
    }
}