using System;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.TagManager.Patches;

[HarmonyPatch(typeof(MechValidationRules), nameof(MechValidationRules.LanceIsValidForSkirmish))]
public static class MechValidationRules_LanceIsValidForSkirmish_Patch
{
    [HarmonyPrefix]
    public static bool Prefix(LanceDef? def, bool requireFullLance, bool includeCustomLances, ref bool __result)
    {
        try
        {
            __result = def != null && TagManagerFeature.Shared.LanceIsValidForSkirmish(def, requireFullLance, includeCustomLances);
            return false;
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
        return true;
    }
}