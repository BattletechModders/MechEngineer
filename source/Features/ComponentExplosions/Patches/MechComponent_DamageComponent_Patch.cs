using System;
using System.Collections.Generic;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.ComponentExplosions.Patches;

[HarmonyPatch(typeof(MechComponent), nameof(MechComponent.DamageComponent))]
public static class MechComponent_DamageComponent_Patch
{
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return instructions.MethodReplacer(
            AccessTools.Property(typeof(MechComponentDef), nameof(MechComponentDef.CanExplode)).GetGetMethod(),
            AccessTools.Method(typeof(MechComponent_DamageComponent_Patch), nameof(get_CanExplode))
        );
    }

    public static bool get_CanExplode(this MechComponentDef def)
    {
        return false;
    }

    [HarmonyPostfix]
    public static void Postfix(MechComponent __instance, WeaponHitInfo hitInfo, ComponentDamageLevel damageLevel, bool applyEffects)
    {
        try
        {
            ComponentExplosionsFeature.Shared.CheckForExplosion(__instance, hitInfo, damageLevel, applyEffects);
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
    }
}
