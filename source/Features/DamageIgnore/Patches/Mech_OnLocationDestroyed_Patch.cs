using System.Collections.Generic;
using BattleTech;

namespace MechEngineer.Features.DamageIgnore.Patches;

[HarmonyPatch(typeof(Mech), nameof(Mech.OnLocationDestroyed))]
internal static class Mech_OnLocationDestroyed_Patch
{
    [HarmonyTranspiler]
    internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return instructions.MethodReplacer(
            AccessTools.Property(typeof(MechComponent), nameof(MechComponent.Location)).GetGetMethod(),
            AccessTools.Method(typeof(Mech_OnLocationDestroyed_Patch), nameof(OverrideLocation))
        );
    }

    private static int OverrideLocation(this MechComponent component)
    {
        return component.componentDef.IsIgnoreDamage() ? 0 : component.Location;
    }
}