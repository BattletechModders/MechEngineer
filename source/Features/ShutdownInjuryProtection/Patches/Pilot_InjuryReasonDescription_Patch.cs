using BattleTech;

namespace MechEngineer.Features.ShutdownInjuryProtection.Patches;

[HarmonyPatch(typeof(Pilot), nameof(Pilot.InjuryReasonDescription), MethodType.Getter)]
public static class Pilot_InjuryReasonDescription_Patch
{
    [HarmonyPostfix]
    [HarmonyWrapSafe]
    public static void Postfix(Pilot __instance, ref string __result)
    {
        if (__instance.InjuryReason == InjuryReasonOverheated)
        {
            __result = "OVERHEATED";
        }
    }

    public static readonly InjuryReason InjuryReasonOverheated = (InjuryReason)101;
}
