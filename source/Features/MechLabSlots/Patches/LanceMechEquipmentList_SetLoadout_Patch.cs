using System;
using System.Collections.Generic;
using BattleTech;
using BattleTech.Data;
using BattleTech.UI;
using BattleTech.UI.TMProWrapper;
using Harmony;
using UnityEngine;

namespace MechEngineer.Features.MechLabSlots.Patches
{
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
}