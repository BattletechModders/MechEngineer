using System.Collections.Generic;
using System.Linq;
using BattleTech;

namespace MechEngineer.Features.CriticalEffects.Patches;


[HarmonyPatch(typeof(EffectManager), nameof(EffectManager.GetTargetStatCollections))]
internal static class EffectManager_GetTargetStatCollections_Patch
{
    internal static void SetContext(MechComponent component, EffectData effectData)
    {
        var tags = effectData.tagData?.tagList;
        if (tags != null && tags.Length > 0 && tags.Contains("TargetComponentStatCollection"))
        {
            ForcedStatCollections = new List<StatCollection>
            {
                component.StatCollection
            };
        }
    }

    internal static void ClearContext()
    {
        ForcedStatCollections = null;
    }

    private static List<StatCollection>? ForcedStatCollections;

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, ref List<StatCollection> __result, EffectData effectData, ICombatant target)
    {
        if (!__runOriginal)
        {
            return;
        }

        if (ForcedStatCollections != null)
        {
            __result = ForcedStatCollections;
            __runOriginal = false;
        }
    }
}