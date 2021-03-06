﻿using System;
using System.Linq;
using BattleTech;
using Harmony;
using MechEngineer.Features.HardpointFix.Public;
using MechEngineer.Misc;

namespace MechEngineer.Features.HardpointFix.Patches
{
    [HarmonyPatch(typeof(MechRepresentationSimGame), "LoadWeapons")]
    public static class MechRepresentationSimGame_LoadWeapons_Patch
    {
        [HarmonyBefore(Mods.CU)]
        [HarmonyPriority(Priority.High)]
        public static void Prefix(MechRepresentationSimGame __instance)
        {
            try
            {
                CalculatorSetup.Setup(
                    __instance.mechDef?.Chassis,
                    __instance.mechDef?.Inventory?.ToList());
            }
            catch (Exception e)
            {
                Control.Logger.Error.Log(e);
            }
        }

        public static void Postfix()
        {
            CalculatorSetup.Reset();
        }
    }
}