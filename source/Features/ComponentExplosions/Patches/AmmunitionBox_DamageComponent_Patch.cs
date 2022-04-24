using System.Collections.Generic;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.ComponentExplosions.Patches;

[HarmonyPatch(typeof(AmmunitionBox), nameof(AmmunitionBox.DamageComponent))]
public static class AmmunitionBox_DamageComponent_Patch
{
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return instructions.MethodReplacer(
            AccessTools.Property(typeof(MechComponentDef), nameof(MechComponentDef.CanExplode)).GetGetMethod(),
            AccessTools.Method(typeof(AmmunitionBox_DamageComponent_Patch), nameof(get_CanExplode))
        );
    }

    public static bool get_CanExplode(this MechComponentDef def)
    {
        return false;
    }
}