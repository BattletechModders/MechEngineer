using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace MechEngineer.Features.CriticalEffects.Patches
{
    //[HarmonyPatch(typeof(CombatHUDStatusPanel), "ShowEffectStatuses")]
    internal static class CombatHUDStatusPanel_ShowEffectStatuses_Patch
    {
        internal static void Prefix(AbstractActor actor)
        {
            try
            {
                DebugUtils.LogActor("ShowEffectStatuses Prefix", actor);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }

        internal static void Postfix(Dictionary<string, CombatHUDStatusIndicator> ___effectDict)
        {
            try
            {
                Control.mod.Logger.LogDebug($"ShowEffectStatuses Postfix effectDict {___effectDict.Keys.JoinAsString()}");
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }

    //[HarmonyPatch(typeof(EffectManager), "CancelEffect")]
    internal static class EffectManager_CancelEffect_Patch
    {
        internal static void Prefix(Effect e)
        {
            try
            {
                Control.mod.Logger.LogDebug($"CancelEffect Prefix {e.EffectData.Description.Id} + {new System.Diagnostics.StackTrace()}");
            }
            catch (Exception e2)
            {
                Control.mod.Logger.LogError(e2);
            }
        }
    }

    //[HarmonyPatch(typeof(EffectManager), "EffectComplete")]
    internal static class EffectManager_EffectComplete_Patch
    {
        internal static void Prefix(Effect e)
        {
            try
            {
                Control.mod.Logger.LogDebug($"EffectComplete Prefix {e.EffectData.Description.Id} + {new System.Diagnostics.StackTrace()}");
            }
            catch (Exception e2)
            {
                Control.mod.Logger.LogError(e2);
            }
        }
    }

    internal static class DebugUtils
    {
        internal static void LogActor(string topic, AbstractActor actor)
        {
            var effects = actor.Combat.EffectManager.GetAllEffectsTargeting(actor);
            Control.mod.Logger.LogDebug($"{topic} effects on actor {actor.GUID} {effects.Select(x => x.EffectData.Description.Id).JoinAsString()}");
        }

        internal static string JoinAsString<T>(this IEnumerable<T> @this) where T : class
        {
            return string.Join(", ", @this.Select(x => x.ToString()).ToArray());
        }
    }

    //[HarmonyPatch(typeof(CombatHUDStatusPanel), "ShouldShowEffect")]
    internal static class CombatHUDStatusPanel_ShouldShowEffect_Patch
    {
        internal static void Postfix(ref bool __result)
        {
            Control.mod.Logger.LogDebug($"ShouldShowEffect {__result}");
        }
    }
}
