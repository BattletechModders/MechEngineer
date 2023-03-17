using BattleTech;

namespace MechEngineer.Features.OverrideDescriptions.Patches;

[HarmonyPatch(typeof(MechDef), nameof(MechDef.RefreshChassis))]
public static class MechDef_RefreshChassis_Patch
{
    [HarmonyPostfix]
    [HarmonyWrapSafe]
    public static void Postfix(MechDef __instance)
    {
        var mechDef = __instance;
        var details = mechDef.Chassis.Description.Details;

        mechDef.Description.Details = details;
    }
}
