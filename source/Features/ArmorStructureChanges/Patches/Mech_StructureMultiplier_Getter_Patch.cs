using System;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.ArmorStructureChanges.Patches;

[HarmonyPatch(typeof(Mech), nameof(Mech.StructureMultiplier), MethodType.Getter)]
public static class Mech_StructureMultiplier_Getter_Patch
{
    [HarmonyPostfix]
    public static void Postfix(Mech __instance, ref float __result)
    {
        try
        {
            __result *= ArmorStructureChangesFeature.GetStructureFactorForMech(__instance);
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
    }
}
