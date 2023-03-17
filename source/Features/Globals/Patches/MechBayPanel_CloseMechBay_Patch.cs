using BattleTech.UI;

namespace MechEngineer.Features.Globals.Patches;

[HarmonyPatch(typeof(MechBayPanel), nameof(MechBayPanel.CloseMechBay))]
public static class MechBayPanel_CloseMechBay_Patch
{
    [HarmonyPostfix]
    [HarmonyWrapSafe]
    public static void Postfix(MechBayPanel __instance)
    {
        Global.ActiveMechBayPanel = null;
    }
}
