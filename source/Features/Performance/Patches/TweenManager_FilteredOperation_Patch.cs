using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Harmony;
using UnityEngine;

namespace MechEngineer.Features.Performance.Patches;

// we don't do DOKill anymore, but I kept this patch anyway. This wasn't enough and DOKill had to be disabled anyway
[HarmonyPatch]
public static class TweenManager_FilteredOperation_Patch
{
    public static MethodBase TargetMethod()
    {
        return AccessTools.Method("DG.Tweening.Core.TweenManager:FilteredOperation");
    }

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
        if (ReferenceEquals(objA, objB))
        {
            return true;
        }
        if (objA is GameObject goA && objB is GameObject goB)
        {
            return goA.GetInstanceID() == goB.GetInstanceID();
        }
        return RuntimeHelpers.Equals(objA, objB);
    }
}