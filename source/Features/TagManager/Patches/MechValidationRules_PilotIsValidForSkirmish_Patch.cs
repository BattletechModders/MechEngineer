using System;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.TagManager.Patches;

[HarmonyPatch(typeof(MechValidationRules), nameof(MechValidationRules.PilotIsValidForSkirmish))]
public static class MechValidationRules_PilotIsValidForSkirmish_Patch
{
    [HarmonyPrefix]
    public static bool Prefix(PilotDef? def, ref bool __result)
    {
        try
        {
            __result = def != null && TagManagerFeature.Shared.PilotIsValidForSkirmish(def);
            return false;
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
        return true;
    }
}