using BattleTech.UI;

namespace MechEngineer.Features.DynamicSlots.Patches;

[HarmonyPatch(typeof(MechLabItemSlotElement), nameof(MechLabItemSlotElement.OnPointerEnter))]
public static class MechLabItemSlotElement_OnPointerEnter_Patch
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, MechLabItemSlotElement __instance)
    {
        if (!__runOriginal)
        {
            return;
        }

        __runOriginal = !__instance.IsDynamicSlotElement();
    }
}