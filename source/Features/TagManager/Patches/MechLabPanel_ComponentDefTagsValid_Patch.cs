using BattleTech;
using BattleTech.UI;
using Harmony;
using System;

namespace MechEngineer.Features.TagManager.Patches;

[HarmonyPatch(typeof(MechLabPanel), nameof(MechLabPanel.ComponentDefTagsValid))]
public static class MechLabPanel_ComponentDefTagsValid_Patch
{
    [HarmonyPrefix]
    public static bool Prefix(MechLabPanel __instance, MechComponentDef def, bool ___isDebugLab, ref bool __result)
    {
        try
        {
            __result = __instance.IsSimGame || TagManagerFeature.Shared.ComponentIsValidForSkirmish(def, ___isDebugLab);
            return false;
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }

        return true;
    }
}