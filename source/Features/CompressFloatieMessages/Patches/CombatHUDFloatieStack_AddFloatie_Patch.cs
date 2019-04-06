using System;
using System.Collections.Generic;
using System.Diagnostics;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(CombatHUDFloatieStack), nameof(CombatHUDFloatieStack.AddFloatie), typeof(FloatieMessage))]
    public static class CombatHUDFloatieStack_AddFloatie_Patch
    {
        public static bool Prefix(
            CombatHUDFloatieStack __instance,
            FloatieMessage message,
            Queue<FloatieMessage> ___msgQueue
            )
        {
            try
            {
                if (FloatieHandler.CompressFloatieMessages(message, ___msgQueue))
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }

            return true;
        }
    }
}