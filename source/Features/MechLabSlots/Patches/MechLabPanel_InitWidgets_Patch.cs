using BattleTech.UI;

namespace MechEngineer.Features.MechLabSlots.Patches;

[HarmonyPatch(typeof(MechLabPanel), nameof(MechLabPanel.InitWidgets))]
public static class MechLabPanel_InitWidgets_Patch
{
    [HarmonyPostfix]
    [HarmonyWrapSafe]
    public static void Postfix(MechLabPanel __instance)
    {
        MechLabLayoutUtils.FixMechLabLayouts(__instance);
        CustomWidgetsFixMechLab.Setup(__instance);
        MechLabMoveUIElements.MoveMechUIElements(__instance);
    }
}
