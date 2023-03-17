using BattleTech;

namespace MechEngineer.Features.ArmorStructureChanges.Patches;

[HarmonyPatch(typeof(Mech), nameof(Mech.GetMaxArmor))]
public static class Mech_GetMaxArmor_Patch
{
    [HarmonyPostfix]
    [HarmonyWrapSafe]
    public static void Postfix(Mech __instance, ref float __result)
    {
        __result *= ArmorStructureChangesFeature.GetArmorFactorForMech(__instance);
    }
}
