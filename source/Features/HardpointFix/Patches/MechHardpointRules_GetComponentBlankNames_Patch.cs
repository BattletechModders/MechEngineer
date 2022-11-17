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
    private static WeaponComponentPrefabCalculator? calculator => CalculatorSetup.SharedCalculator;

    [HarmonyPriority(Priority.High)]
    [HarmonyPrefix]
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
            Log.Main.Error?.Log(e);
        }
        return true;
    }

    [HarmonyPostfix]
    public static void Postfix(ChassisLocations location, ref List<string> __result)
    {
        try
        {
            Log.Main.Trace?.Log($"GetComponentBlankNames blanks=[{__result?.JoinAsString()}] location={location}");
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
    }
}
