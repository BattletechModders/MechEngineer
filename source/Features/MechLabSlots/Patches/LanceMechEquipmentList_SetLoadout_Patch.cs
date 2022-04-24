using System;
using BattleTech.UI;
using Harmony;

namespace MechEngineer.Features.MechLabSlots.Patches;

[HarmonyPatch(
    typeof(LanceMechEquipmentList),
    nameof(LanceMechEquipmentList.SetLoadout),
    new Type[0]
)]
public static class LanceMechEquipmentList_SetLoadout_Patch
{
    [HarmonyPriority(Priority.High)]
    public static void Prefix(LanceMechEquipmentList __instance)
    {
        try
        {
            CustomWidgetsFixLanceMechEquipment.SetLoadout_LabelDefaults(__instance);
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
    }

    [HarmonyPriority(Priority.Low)]
    public static void Postfix(LanceMechEquipmentList __instance)
    {
        try
        {
            CustomWidgetsFixLanceMechEquipment.SetLoadout_LabelOverrides(__instance);
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
    }
}