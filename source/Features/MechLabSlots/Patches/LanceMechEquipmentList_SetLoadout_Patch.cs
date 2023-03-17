using System;
using BattleTech.UI;

namespace MechEngineer.Features.MechLabSlots.Patches;

[HarmonyPatch(
    typeof(LanceMechEquipmentList),
    nameof(LanceMechEquipmentList.SetLoadout),
    new Type[0]
)]
public static class LanceMechEquipmentList_SetLoadout_Patch
{
    [HarmonyPriority(Priority.High)]
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, LanceMechEquipmentList __instance)
    {
        if (!__runOriginal)
        {
            return;
        }

        CustomWidgetsFixLanceMechEquipment.SetLoadout_LabelDefaults(__instance);
    }

    [HarmonyPriority(Priority.Low)]
    [HarmonyPostfix]
    [HarmonyWrapSafe]
    public static void Postfix(LanceMechEquipmentList __instance)
    {
        CustomWidgetsFixLanceMechEquipment.SetLoadout_LabelOverrides(__instance);
    }
}
