using BattleTech.UI;
using Harmony;

namespace MechEngineer.Features.DynamicSlots.Patches;

[HarmonyPatch(typeof(MechLabItemSlotElement), nameof(MechLabItemSlotElement.OnPointerExit))]
public static class MechLabItemSlotElement_OnPointerExit_Patch
{
    public static bool Prefix(MechLabItemSlotElement __instance)
    {
        return !__instance.IsDynamicSlotElement();
    }
}