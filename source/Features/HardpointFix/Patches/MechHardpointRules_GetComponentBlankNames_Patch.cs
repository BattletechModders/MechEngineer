using System.Collections.Generic;
using BattleTech;
using MechEngineer.Features.CriticalEffects.Patches;
using MechEngineer.Features.HardpointFix.Public;

namespace MechEngineer.Features.HardpointFix.Patches;

[HarmonyPatch(typeof(MechHardpointRules), nameof(MechHardpointRules.GetComponentBlankNames))]
public static class MechHardpointRules_GetComponentBlankNames_Patch
{
    private static WeaponComponentPrefabCalculator? calculator => CalculatorSetup.SharedCalculator;

    [HarmonyPriority(Priority.High)]
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, ChassisLocations location, ref List<string> __result)
    {
        if (!__runOriginal)
        {
            return;
        }

        if (calculator != null)
        {
            __result = calculator.GetRequiredBlankPrefabNamesInLocation(location);
            __runOriginal = false;
        }
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    public static void Postfix(ChassisLocations location, ref List<string> __result)
    {
        Log.Main.Trace?.Log($"GetComponentBlankNames blanks=[{__result?.JoinAsString()}] location={location}");
    }
}
