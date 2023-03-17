using BattleTech;

namespace MechEngineer.Features.ArmorStructureChanges.Patches;

[HarmonyPatch(typeof(Mech), nameof(Mech.GetMaxStructure))]
public static class Mech_GetMaxStructure_Patch
{
    [HarmonyPostfix]
    [HarmonyWrapSafe]
    public static void Postfix(Mech __instance, ref float __result)
    {
        __result *= ArmorStructureChangesFeature.GetStructureFactorForMech(__instance);
    }
}
