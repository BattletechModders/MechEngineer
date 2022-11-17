using System;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.ShutdownInjuryProtection.Patches;

[HarmonyPatch(typeof(Pilot), nameof(Pilot.InjuryReasonDescription), MethodType.Getter)]
public static class Pilot_InjuryReasonDescription_Patch
{
    [HarmonyPostfix]
    public static void Postfix(Pilot __instance, ref string __result)
    {
        try
        {
            if (__instance.InjuryReason == InjuryReasonOverheated)
            {
                __result = "OVERHEATED";
            }
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
    }

    public static readonly InjuryReason InjuryReasonOverheated = (InjuryReason)101;
}
