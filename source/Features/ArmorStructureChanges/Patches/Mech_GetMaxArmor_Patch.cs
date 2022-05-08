using System;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.ArmorStructureChanges.Patches;

[HarmonyPatch(typeof(Mech), nameof(Mech.GetMaxArmor))]
public static class Mech_GetMaxArmor_Patch
{
    [HarmonyPostfix]
    public static void Postfix(Mech __instance, ref float __result)
    {
        try
        {
            __result *= ArmorStructureChangesFeature.GetArmorFactorForMech(__instance);
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
    }
}