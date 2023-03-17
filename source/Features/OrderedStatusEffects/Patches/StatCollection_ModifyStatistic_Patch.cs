using BattleTech;

namespace MechEngineer.Features.OrderedStatusEffects.Patches;

[HarmonyPatch(typeof(StatCollection), nameof(StatCollection.ModifyStatistic))]
public static class StatCollection_ModifyStatistic_Patch
{
    [HarmonyPostfix]
    [HarmonyWrapSafe]
    public static void Postfix(StatCollection __instance, string statName, int __result)
    {
        if (__result < 0)
        {
            return;
        }
        OrderedStatusEffectsFeature.Shared.ModifyStatisticPostfix(__instance, statName);
    }
}
