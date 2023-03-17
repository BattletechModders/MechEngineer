using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;
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
    [HarmonyWrapSafe]
    internal static void Prefix(ref bool __runOriginal, AbstractActor actor)
    {
        if (!__runOriginal)
        {
            return;
        }

        DebugUtils.LogActor("ShowEffectStatuses Prefix", actor);
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    internal static void Postfix(Dictionary<string, CombatHUDStatusIndicator> ___effectDict)
    {
        Log.Main.Debug?.Log($"ShowEffectStatuses Postfix effectDict {___effectDict.Keys.JoinAsString()}");
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
    [HarmonyWrapSafe]
    internal static void Prefix(ref bool __runOriginal, Effect e)
    {
        if (!__runOriginal)
        {
            return;
        }

        Log.Main.Debug?.Log($"CancelEffect Prefix {e.EffectData.Description.Id} + {new System.Diagnostics.StackTrace()}");
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
    [HarmonyWrapSafe]
    internal static void Prefix(ref bool __runOriginal, Effect e)
    {
        if (!__runOriginal)
        {
            return;
        }

        Log.Main.Debug?.Log($"EffectComplete Prefix {e.EffectData.Description.Id} + {new System.Diagnostics.StackTrace()}");
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
    [HarmonyWrapSafe]
    internal static void Postfix(ref bool __result)
    {
        Log.Main.Debug?.Log($"ShouldShowEffect {__result}");
    }
}
