using System.Collections.Generic;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.DamageIgnore.Patches
{
    [HarmonyPatch(typeof(Mech), nameof(Mech.GetComponentInSlot))]
    internal static class Mech_GetComponentInSlot_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return instructions.MethodReplacer(
                AccessTools.Property(typeof(MechComponent), nameof(MechComponent.Location)).GetGetMethod(),
                AccessTools.Method(typeof(DamageIgnoreHelper), nameof(DamageIgnoreHelper.OverrideLocation))
            );
        }
    }
}