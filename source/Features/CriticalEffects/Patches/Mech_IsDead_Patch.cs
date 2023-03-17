using System.Collections.Generic;
using BattleTech;

namespace MechEngineer.Features.CriticalEffects.Patches;

[HarmonyPatch(typeof(Mech), nameof(Mech.IsDead), MethodType.Getter)]
internal static class Mech_IsDead_Patch
{
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return instructions.MethodReplacer(
            AccessTools.Property(typeof(Mech), nameof(Mech.HeadStructure)).GetGetMethod(),
            AccessTools.Method(typeof(Mech_IsDead_Patch), nameof(get_HeadStructure))
        );
    }

    public static float get_HeadStructure(this Mech @this)
    {
        return 1; // never be dead because of no head structure
    }
}