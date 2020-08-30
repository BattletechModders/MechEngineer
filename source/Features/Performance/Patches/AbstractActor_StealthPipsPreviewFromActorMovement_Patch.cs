﻿using System;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.Performance.Patches
{
    // disable stealth preview, takes alot of resources
    [HarmonyPatch(typeof(AbstractActor), nameof(AbstractActor.StealthPipsPreviewFromActorMovement))]
    public static class AbstractActor_StealthPipsPreviewFromActorMovement_Patch
    {
        public static bool Prefix(AbstractActor __instance, ref int __result)
        {
            try
            {
                __result = 0;
                return false;
            }
            catch (Exception e)
            {
                Control.Logger.Error.Log(e);
            }
            return true;
        }
    }
}