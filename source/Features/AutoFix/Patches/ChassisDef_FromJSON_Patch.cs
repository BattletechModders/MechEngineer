using BattleTech;

namespace MechEngineer.Features.AutoFix.Patches;

[HarmonyPatch(typeof(ChassisDef), nameof(ChassisDef.FromJSON))]
public static class ChassisDef_FromJSON_Patch
{
    [HarmonyPostfix]
    [HarmonyWrapSafe]
    public static void Postfix(ChassisDef __instance)
    {
        ChassisHandler.OverrideChassisSettings(__instance);
    }
}
