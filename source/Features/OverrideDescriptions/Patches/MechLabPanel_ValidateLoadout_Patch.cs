﻿using System;
using BattleTech.UI;
using Harmony;

namespace MechEngineer.Features.OverrideDescriptions.Patches
{
    [HarmonyPatch(typeof(MechLabPanel), "ValidateLoadout")]
    public static class MechLabPanel_ValidateLoadout_Patch
    {
        public static void Postfix(MechLabPanel __instance)
        {
            try
            {
                OverrideDescriptionsFeature.Shared.RefreshData(__instance);
            }
            catch (Exception e)
            {
                Control.Logger.Error.Log(e);
            }
        }
    }
}