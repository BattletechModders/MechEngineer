using System;
using System.Collections.Generic;
using BattleTech;
using Harmony;
using MechEngineer.Features.CriticalEffects.Patches;
using MechEngineer.Features.HardpointFix.Public;

namespace MechEngineer.Features.HardpointFix.Patches;

[HarmonyPatch(typeof(MechHardpointRules), nameof(MechHardpointRules.GetComponentBlankNames))]
public static class MechHardpointRules_GetComponentBlankNames_Patch
{
    private static WeaponComponentPrefabCalculator calculator => CalculatorSetup.SharedCalculator;

    [HarmonyPriority(Priority.High)]
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
            Control.Logger.Trace?.Log($"GetComponentBlankNames blanks=[{__result?.JoinAsString()}] location={location}");
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
    }
}