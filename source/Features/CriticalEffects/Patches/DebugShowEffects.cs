using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using Harmony;
using MechEngineer.Misc;

namespace MechEngineer.Features.CriticalEffects.Patches;

[HarmonyPatch(typeof(CombatHUDStatusPanel), nameof(CombatHUDStatusPanel.ShowEffectStatuses))]
internal static class CombatHUDStatusPanel_ShowEffectStatuses_Patch
{
    [UsedByHarmony]
    public static bool Prepare()
    {
        return CriticalEffectsFeature.settings.DebugLogEffects;
    }

    [HarmonyPrefix]
    internal static void Prefix(AbstractActor actor)
    {
        try
        {
            DebugUtils.LogActor("ShowEffectStatuses Prefix", actor);
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
    }

    [HarmonyPostfix]
    internal static void Postfix(Dictionary<string, CombatHUDStatusIndicator> ___effectDict)
    {
        try
        {
            Log.Main.Debug?.Log($"ShowEffectStatuses Postfix effectDict {___effectDict.Keys.JoinAsString()}");
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
    }
}

[HarmonyPatch(typeof(EffectManager), nameof(EffectManager.CancelEffect))]
internal static class EffectManager_CancelEffect_Patch
{
    [UsedByHarmony]
    public static bool Prepare()
    {
        return CriticalEffectsFeature.settings.DebugLogEffects;
    }

    [HarmonyPrefix]
    internal static void Prefix(Effect e)
    {
        try
        {
            Log.Main.Debug?.Log($"CancelEffect Prefix {e.EffectData.Description.Id} + {new System.Diagnostics.StackTrace()}");
        }
        catch (Exception e2)
        {
            Log.Main.Error?.Log(e2);
        }
    }
}

[HarmonyPatch(typeof(EffectManager), nameof(EffectManager.EffectComplete))]
internal static class EffectManager_EffectComplete_Patch
{
    [UsedByHarmony]
    public static bool Prepare()
    {
        return CriticalEffectsFeature.settings.DebugLogEffects;
    }

    [HarmonyPrefix]
    internal static void Prefix(Effect e)
    {
        try
        {
            Log.Main.Debug?.Log($"EffectComplete Prefix {e.EffectData.Description.Id} + {new System.Diagnostics.StackTrace()}");
        }
        catch (Exception e2)
        {
            Log.Main.Error?.Log(e2);
        }
    }
}

internal static class DebugUtils
{
    internal static void LogActor(string topic, AbstractActor actor)
    {
        var effects = actor.Combat.EffectManager.GetAllEffectsTargeting(actor);
        Log.Main.Debug?.Log($"{topic} effects on actor {actor.GUID} {effects.Select(x => x.EffectData.Description.Id).JoinAsString()}");
    }

    internal static string JoinAsString<T>(this IEnumerable<T> @this) where T : class
    {
        return string.Join(", ", @this.Select(x => x.ToString()).ToArray());
    }
}

[HarmonyPatch(typeof(CombatHUDStatusPanel), nameof(CombatHUDStatusPanel.ShouldShowEffect))]
internal static class CombatHUDStatusPanel_ShouldShowEffect_Patch
{
    [UsedByHarmony]
    public static bool Prepare()
    {
        return CriticalEffectsFeature.settings.DebugLogEffects;
    }

    [HarmonyPostfix]
    internal static void Postfix(ref bool __result)
    {
        Log.Main.Debug?.Log($"ShouldShowEffect {__result}");
    }
}
