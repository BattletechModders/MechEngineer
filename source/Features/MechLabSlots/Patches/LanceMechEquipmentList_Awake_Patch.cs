using System;
using BattleTech.UI;
using BattleTech.UI.TMProWrapper;
using Harmony;

namespace MechEngineer.Features.MechLabSlots.Patches
{
    [HarmonyPatch(typeof(LanceMechEquipmentList), "Awake")]
    public static class LanceMechEquipmentList_Awake_Patch
    {
        public static void Postfix(LocalizableText ___centerTorsoLabel)
        {
            try
            {
                CustomWidgetsFixLanceMechEquipment.SetupContainers(___centerTorsoLabel.transform.parent.gameObject);
            }
            catch (Exception e)
            {
                Control.Logger.Error.Log(e);
            }
        }
    }
}