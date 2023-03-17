using BattleTech.UI;

namespace MechEngineer.Features.Globals.Patches;

[HarmonyPatch(typeof(MechLabPanel), nameof(MechLabPanel.ExitMechLab))]
public static class MechLabPanel_ExitMechLab_Patch
{
    [HarmonyPostfix]
    [HarmonyWrapSafe]
    public static void Postfix(MechLabPanel __instance)
    {
        Global.ActiveMechLabPanel = null;
    }
}
