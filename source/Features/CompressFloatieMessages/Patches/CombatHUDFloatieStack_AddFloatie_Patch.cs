using System;
using System.Collections.Generic;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace MechEngineer.Features.CompressFloatieMessages.Patches;

[HarmonyPatch(typeof(CombatHUDFloatieStack), nameof(CombatHUDFloatieStack.AddFloatie), typeof(FloatieMessage))]
public static class CombatHUDFloatieStack_AddFloatie_Patch
{
    [HarmonyPrefix]
    public static bool Prefix(
        CombatHUDFloatieStack __instance,
        FloatieMessage message,
        Queue<FloatieMessage> ___msgQueue
        )
    {
        try
        {
            if (CompressFloatieMessagesFeature.CompressFloatieMessages(message, ___msgQueue))
            {
                return false;
            }
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }

        return true;
    }
}
