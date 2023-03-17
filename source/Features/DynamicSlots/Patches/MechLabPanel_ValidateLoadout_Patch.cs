using BattleTech.UI;

namespace MechEngineer.Features.DynamicSlots.Patches;

[HarmonyPatch(typeof(MechLabPanel), nameof(MechLabPanel.ValidateLoadout))]
public static class MechLabPanel_ValidateLoadout_Patch
{
    [HarmonyPostfix]
    [HarmonyWrapSafe]
    public static void Postfix(MechLabPanel __instance)
    {
        DynamicSlotsFeature.Shared.RefreshData(__instance);
    }
}
