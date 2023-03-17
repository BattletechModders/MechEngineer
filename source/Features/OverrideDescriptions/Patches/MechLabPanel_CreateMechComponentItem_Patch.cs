using BattleTech;
using BattleTech.UI;

namespace MechEngineer.Features.OverrideDescriptions.Patches;

[HarmonyPatch(typeof(MechLabPanel), nameof(MechLabPanel.CreateMechComponentItem))]
internal static class MechLabPanel_CreateMechComponentItem_Patch
{
    [HarmonyPostfix]
    [HarmonyWrapSafe]
    internal static void Postfix(MechLabPanel __instance, MechComponentRef componentRef, MechLabItemSlotElement __result)
    {
        OverrideDescriptionsFeature.Shared.AdjustSlotElement(__result, __instance);
    }
}
