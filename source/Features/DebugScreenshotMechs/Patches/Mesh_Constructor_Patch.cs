using System.Collections.Generic;
using System.IO;
using System.Linq;
using Harmony;
using MechEngineer.Misc;
using UnityEngine;

namespace MechEngineer.Features.DebugScreenshotMechs.Patches;

[HarmonyPatch(typeof(Mesh), MethodType.Constructor)]
public static class Mesh_Constructor_Patch
{
    internal static readonly Dictionary<int, string> ids = new();
    private static readonly string outputFile = Path.Combine(Control.Mod.Directory, "mesh.txt");
    private static readonly Dictionary<string, int> set = new();

    [UsedByHarmony]
    public static bool Prepare()
    {
        return DebugScreenshotMechsFeature.Shared.Settings.DumpGameObjectCounts;
    }

    [HarmonyPostfix]
    public static void Postfix(Mesh __instance)
    {
        var stacktrace = new System.Diagnostics.StackTrace(2, false);
        var caller = "";
        for (var i = 0; i < 10; i++)
        {
            if (i >= stacktrace.FrameCount)
            {
                break;
            }
            var frame = stacktrace.GetFrame(i);
            var method = frame.GetMethod();
            if (i >= 1)
            {
                caller += " -> ";
            }
            caller += method.DeclaringType?.FullName + "." + method.Name;
        }
        ids[__instance.GetInstanceID()] = caller;
        if (set.TryGetValue(caller, out var counter))
        {
            ++counter;
        }
        else
        {
            counter = 1;
        }

        set[caller] = counter;
        File.WriteAllLines(outputFile,
            set
                .OrderByDescending(kv => kv.Value)
                .ThenBy(kv => kv.Key)
                .Select(kv => $"{kv.Value}|{kv.Key}")
        );
    }
}