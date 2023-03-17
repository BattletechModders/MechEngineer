using System.Collections.Generic;
using BattleTech;
using BattleTech.UI;

namespace MechEngineer.Features.CompressFloatieMessages.Patches;

[HarmonyPatch(typeof(CombatHUDFloatieStack), nameof(CombatHUDFloatieStack.AddFloatie), typeof(FloatieMessage))]
public static class CombatHUDFloatieStack_AddFloatie_Patch
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, 
        CombatHUDFloatieStack __instance,
        FloatieMessage message,
        Queue<FloatieMessage> ___msgQueue
        )
    {
        if (!__runOriginal)
        {
            return;
        }

        if (CompressFloatieMessagesFeature.CompressFloatieMessages(message, ___msgQueue))
        {
            __runOriginal = false;
        }
    }
}
