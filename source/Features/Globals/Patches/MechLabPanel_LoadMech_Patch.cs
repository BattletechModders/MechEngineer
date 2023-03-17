using BattleTech.UI;

namespace MechEngineer.Features.Globals.Patches;

[HarmonyPatch(typeof(MechLabPanel), nameof(MechLabPanel.LoadMech))]
public static class MechLabPanel_LoadMech_Patch
{
    [HarmonyPostfix]
    [HarmonyWrapSafe]
    public static void Postfix(MechLabPanel __instance)
    {
        Global.ActiveMechLabPanel = __instance;
    }
}
