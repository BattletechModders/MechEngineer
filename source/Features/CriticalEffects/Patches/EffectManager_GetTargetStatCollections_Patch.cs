using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using Harmony;

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
    public static bool Prefix(ref List<StatCollection> __result, EffectData effectData, ICombatant target)
    {
        try
        {
            if (ForcedStatCollections != null)
            {
                __result = ForcedStatCollections;
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