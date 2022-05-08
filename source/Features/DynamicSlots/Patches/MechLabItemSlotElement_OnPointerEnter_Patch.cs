using BattleTech.UI;
using Harmony;

namespace MechEngineer.Features.DynamicSlots.Patches;

[HarmonyPatch(typeof(MechLabItemSlotElement), nameof(MechLabItemSlotElement.OnPointerEnter))]
public static class MechLabItemSlotElement_OnPointerEnter_Patch
{
    [HarmonyPrefix]
    public static bool Prefix(MechLabItemSlotElement __instance)
    {
        return !__instance.IsDynamicSlotElement();
    }
}