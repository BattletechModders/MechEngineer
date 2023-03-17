using BattleTech;
using MechEngineer.Features.HardpointFix.Public;

namespace MechEngineer.Features.HardpointFix.Patches;

[HarmonyPatch(typeof(MechHardpointRules), nameof(MechHardpointRules.GetComponentPrefabName))]
public static class MechHardpointRules_GetComponentPrefabName_Patch
{
    [HarmonyPriority(Priority.High)]
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, BaseComponentRef componentRef, ref string? __result)
    {
        if (!__runOriginal)
        {
            return;
        }

        if (CalculatorSetup.SharedCalculator != null)
        {
            __result = CalculatorSetup.SharedCalculator.GetPrefabName(componentRef);
            __runOriginal = false;
        }
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    public static void Postfix(BaseComponentRef componentRef, ref string __result)
    {
        Log.Main.Trace?.Log($"GetComponentPrefabName prefabName={__result} ComponentDefID={componentRef.ComponentDefID} PrefabIdentifier={componentRef.Def.PrefabIdentifier}");
    }
}
