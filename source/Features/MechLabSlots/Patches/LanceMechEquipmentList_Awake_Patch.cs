using BattleTech.UI;
using BattleTech.UI.TMProWrapper;

namespace MechEngineer.Features.MechLabSlots.Patches;

[HarmonyPatch(typeof(LanceMechEquipmentList), nameof(LanceMechEquipmentList.Awake))]
public static class LanceMechEquipmentList_Awake_Patch
{
    [HarmonyPostfix]
    [HarmonyWrapSafe]
    public static void Postfix(LocalizableText ___centerTorsoLabel)
    {
        CustomWidgetsFixLanceMechEquipment.Awake(___centerTorsoLabel);
    }
}
