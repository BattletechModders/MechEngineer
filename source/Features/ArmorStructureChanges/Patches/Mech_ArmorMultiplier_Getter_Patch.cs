using BattleTech;

namespace MechEngineer.Features.ArmorStructureChanges.Patches;

[HarmonyPatch(typeof(Mech), nameof(Mech.ArmorMultiplier), MethodType.Getter)]
public static class Mech_ArmorMultiplier_Getter_Patch
{
    [HarmonyPostfix]
    [HarmonyWrapSafe]
    public static void Postfix(Mech __instance, ref float __result)
    {
        __result *= ArmorStructureChangesFeature.GetArmorFactorForMech(__instance);
    }
}
