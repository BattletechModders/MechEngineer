using System;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.DamageIgnore.Patches;

[HarmonyPatch(typeof(MechComponent), nameof(MechComponent.DamageComponent))]
public static class MechComponent_DamageComponent_Patch
{
    [HarmonyPrefix]
    public static bool Prefix(MechComponent __instance, ref ComponentDamageLevel damageLevel)
    {
        try
        {
            if (__instance.componentDef.IsIgnoreDamage())
            {
                damageLevel = ComponentDamageLevel.Functional;
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
