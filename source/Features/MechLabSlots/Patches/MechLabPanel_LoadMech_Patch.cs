using BattleTech.UI;

namespace MechEngineer.Features.MechLabSlots.Patches;

[HarmonyPatch(typeof(MechLabPanel), nameof(MechLabPanel.LoadMech))]
public static class MechLabPanel_LoadMech_Patch
{
    [HarmonyPostfix]
    [HarmonyWrapSafe]
    public static void Postfix(MechLabPanel __instance)
    {
        MechLabAutoZoom.LoadMech(__instance);
    }
}
