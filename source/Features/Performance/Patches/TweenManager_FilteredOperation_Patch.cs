#nullable disable
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening.Core;
using UnityEngine;

namespace MechEngineer.Features.Performance.Patches;

[HarmonyPatch(typeof(TweenManager), nameof(TweenManager.FilteredOperation))]
public static class TweenManager_FilteredOperation_Patch
{
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return instructions.MethodReplacer(
            AccessTools.Method(typeof(object), nameof(object.Equals), new[] {typeof(object)}),
            AccessTools.Method(typeof(TweenManager_FilteredOperation_Patch), nameof(ObjectEquals))
        ).MethodReplacer(
            AccessTools.Method(typeof(object), nameof(object.Equals), new[] {typeof(object), typeof(object)}),
            AccessTools.Method(typeof(TweenManager_FilteredOperation_Patch), nameof(ObjectEquals))
        );
    }

    public static bool ObjectEquals(object objA, object objB)
    {
        if (objA is null && objB is null)
        {
            return true;
        }

        if (objA is GameObject || objB is GameObject)
        {
            // reference equals check, hope that's good enough
            return objA == objB;
        }

        // Fallback for all other types, dont think this can even happen
        return RuntimeHelpers.Equals(objA, objB);
    }
}