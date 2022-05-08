using System.Collections.Generic;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.TurretLimitedAmmo.Patches;

[HarmonyPatch(typeof(Turret), nameof(Turret.OnNewRound))]
public static class Turret_OnNewRound_Patch
{
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return instructions.MethodReplacer(
            AccessTools.Property(typeof(Weapon), nameof(Weapon.HasAmmo)).GetGetMethod(),
            AccessTools.Method(typeof(Turret_OnNewRound_Patch), nameof(HasAmmo))
        );
    }

    public static bool HasAmmo(this Weapon @this)
    {
        return true;
    }
}