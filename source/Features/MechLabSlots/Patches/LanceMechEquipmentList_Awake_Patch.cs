using System;
using BattleTech.UI;
using BattleTech.UI.TMProWrapper;
using Harmony;

namespace MechEngineer.Features.MechLabSlots.Patches;

[HarmonyPatch(typeof(LanceMechEquipmentList), nameof(LanceMechEquipmentList.Awake))]
public static class LanceMechEquipmentList_Awake_Patch
{
    [HarmonyPostfix]
    public static void Postfix(LocalizableText ___centerTorsoLabel)
    {
        try
        {
            CustomWidgetsFixLanceMechEquipment.Awake(___centerTorsoLabel);
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
    }
}