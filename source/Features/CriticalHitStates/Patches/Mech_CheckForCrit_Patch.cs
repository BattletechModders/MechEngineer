using System.Collections.Generic;
using BattleTech;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(Mech), "CheckForCrit")]
    public static class Mech_CheckForCrit_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return instructions.MethodReplacer(
                AccessTools.Method(typeof(MessageCenter), nameof(MessageCenter.PublishMessage)),
                AccessTools.Method(typeof(Mech_CheckForCrit_Patch), nameof(Mech_CheckForCrit_Patch.PublishMessage))
            );
        }

        public static void PublishMessage(this MessageCenter @this, MessageCenterMessage message)
        {
            // ignore all messages
        }
    }
}