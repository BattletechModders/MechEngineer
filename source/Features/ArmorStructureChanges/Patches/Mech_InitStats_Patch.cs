using BattleTech;

namespace MechEngineer.Features.ArmorStructureChanges.Patches;

[HarmonyPatch(typeof(Mech), nameof(Mech.InitStats))]
public static class Mech_InitStats_Patch
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, Mech __instance)
    {
        if (!__runOriginal)
        {
            return;
        }

        if (!__instance.Combat.IsLoadingFromSave)
        {
            ArmorStructureChangesFeature.Shared.InitStats(__instance);
        }
    }
}
