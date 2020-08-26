﻿using System;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.ArmorStructureChanges.Patches
{
    [HarmonyPatch(typeof(Mech), "InitStats")]
    public static class Mech_InitStats_Patch
    {
        public static void Prefix(Mech __instance)
        {
            try
            {
                if (!__instance.Combat.IsLoadingFromSave)
                {
                    ArmorStructureChangesFeature.Shared.InitStats(__instance);
                }
            }
            catch (Exception e)
            {
                Control.Logger.Error.Log(e);
            }
        }
    }
}