﻿using System;
using BattleTech.UI;
using Harmony;

namespace MechEngineer.Features.MechLabSlots.Patches
{
    [HarmonyPatch(typeof(MechLabPanel), "RefreshDropHighlights")]
    public static class MechLabPanel_RefreshDropHighlights_Patch
    {
        public static void Prefix(MechLabLocationWidget __instance, IMechLabDraggableItem item)
        {
            try
            {
                MechPropertiesWidget.RefreshDropHighlights(__instance, item);
            }
            catch (Exception e)
            {
                Control.Logger.Error.Log(e);
            }
        }
    }
}