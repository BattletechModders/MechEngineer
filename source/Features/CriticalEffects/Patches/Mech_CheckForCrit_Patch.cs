using System.Collections.Generic;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.CriticalEffects.Patches;

[HarmonyPatch(typeof(Mech), nameof(Mech.CheckForCrit))]
public static class Mech_CheckForCrit_Patch
{
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return instructions.MethodReplacer(
            AccessTools.Method(typeof(MessageCenter), nameof(MessageCenter.PublishMessage)),
            AccessTools.Method(typeof(Mech_CheckForCrit_Patch), nameof(PublishMessage))
        );
    }

    public static void PublishMessage(this MessageCenter @this, MessageCenterMessage message)
    {
        // ignore all messages
    }
}