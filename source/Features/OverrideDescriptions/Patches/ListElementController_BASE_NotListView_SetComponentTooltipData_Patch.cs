using BattleTech.UI;

namespace MechEngineer.Features.OverrideDescriptions.Patches;

[HarmonyPatch(typeof(ListElementController_BASE_NotListView), nameof(ListElementController_BASE_NotListView.SetComponentTooltipData))]
internal static class ListElementController_BASE_NotListView_SetComponentTooltipData_Patch
{
    [HarmonyPostfix]
    [HarmonyWrapSafe]
    internal static void Postfix(ListElementController_BASE_NotListView __instance)
    {
        OverrideDescriptionsFeature.Shared.AdjustInventoryElement(__instance);
    }
}
