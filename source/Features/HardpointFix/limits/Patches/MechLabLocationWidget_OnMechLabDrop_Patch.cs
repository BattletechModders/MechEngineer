using System;
using BattleTech;
using BattleTech.UI;
using Harmony;
using UnityEngine.EventSystems;

namespace MechEngineer.Features.HardpointFix.limits.Patches
{
    [HarmonyPatch(typeof(MechLabLocationWidget), "OnMechLabDrop")]
    public static class MechLabLocationWidget_OnMechLabDrop_Patch
    {
        public static bool Prefix(MechLabLocationWidget __instance, PointerEventData eventData, MechLabDropTargetType addToType)
        {
            try
            {
                if (!Control.settings.HardpointFix.EnforceHardpointLimits)
                {
                    return true;
                }

                var vhl = new MechLabLocationWidgetPatchHelper(__instance);
                return vhl.MechLabLocationWidgetOnMechLabDrop(eventData);
            }
            catch (Exception e)
            {
                Control.Logger.Error.Log(e);
                return true;
            }
        }
    }
}