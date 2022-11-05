using System;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.TagManager.Patches;

[HarmonyPatch(typeof(MechValidationRules), nameof(MechValidationRules.MechIsValidForSkirmish))]
public static class MechValidationRules_MechIsValidForSkirmish_Patch
{
    [HarmonyPrefix]
    public static bool Prefix(MechDef? def, bool includeCustomMechs, ref bool __result)
    {
        try
        {
            __result = def != null && TagManagerFeature.Shared.MechIsValidForSkirmish(def, includeCustomMechs);
            return false;
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
        return true;
    }
}