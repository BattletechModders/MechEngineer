using BattleTech.UI;

namespace MechEngineer.Features.ArmorMaximizer.Patches;

[HarmonyPatch(typeof (MechLabLocationWidget), nameof(MechLabLocationWidget.RefreshArmor))]
public static class MechLabLocationWidget_RefreshArmor_Patch
{
    [HarmonyPostfix]
    [HarmonyWrapSafe]
    public static void Postfix(MechLabLocationWidget __instance)
    {
        ArmorMaximizerHandler.OnRefreshArmor(__instance);
    }
}
