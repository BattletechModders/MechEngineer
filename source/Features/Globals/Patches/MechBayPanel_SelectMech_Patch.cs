using BattleTech.UI;

namespace MechEngineer.Features.Globals.Patches;

[HarmonyPatch(typeof(MechBayPanel), nameof(MechBayPanel.SelectMech))]
public static class MechBayPanel_SelectMech_Patch
{
    [HarmonyPostfix]
    [HarmonyWrapSafe]
    public static void Postfix(MechBayPanel __instance)
    {
        Global.ActiveMechBayPanel = __instance;
    }
}
