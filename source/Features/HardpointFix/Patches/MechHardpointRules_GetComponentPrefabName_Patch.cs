using System;
using BattleTech;
using Harmony;
using MechEngineer.Features.HardpointFix.Public;

namespace MechEngineer.Features.HardpointFix.Patches;

[HarmonyPatch(typeof(MechHardpointRules), nameof(MechHardpointRules.GetComponentPrefabName))]
public static class MechHardpointRules_GetComponentPrefabName_Patch
{
    [HarmonyPriority(Priority.High)]
    public static bool Prefix(BaseComponentRef componentRef, ref string __result)
    {
        try
        {
            if (CalculatorSetup.SharedCalculator != null)
            {
                __result = CalculatorSetup.SharedCalculator.GetPrefabName(componentRef);
                return false;
            }
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
        return true;
    }

    public static void Postfix(BaseComponentRef componentRef, ref string __result)
    {
        try
        {
            Control.Logger.Trace?.Log($"GetComponentPrefabName prefabName={__result} ComponentDefID={componentRef.ComponentDefID} PrefabIdentifier={componentRef.Def.PrefabIdentifier}");
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
    }
}