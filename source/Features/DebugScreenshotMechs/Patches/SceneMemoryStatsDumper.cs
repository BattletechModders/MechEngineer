using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MechEngineer.Features.DebugScreenshotMechs.Patches;

internal static class SceneMemoryStatsDumper
{
    private static Dictionary<string, int> initial;
    internal const string MemoryFilename = "gameobjects.txt";

    internal static void DumpUiStuff()
    {
        var dict = new Dictionary<string, int>();
        foreach (var obj in Resources.FindObjectsOfTypeAll<Object>())
        {
            if (obj is Component)
            {
                continue;
            }

            string key;
            if (obj is GameObject go)
            {
                var parents = GatherParents(go)
                    .Reverse()
                    .Where(name => name.StartsWith("ui", StringComparison.OrdinalIgnoreCase))
                    .Take(2);
                key = string.Join(" -> ", parents);
            }
            else
            {
                key = obj.GetType().Name + "|" + obj.name;
            }

            if (dict.TryGetValue(key, out var counter))
            {
                counter++;
            }
            else
            {
                counter = 1;
            }
            dict[key] = counter;
        }
        if (initial == null)
        {
            initial = dict;
            return;
        }
        var outputFile = Path.Combine(Control.Mod.Directory, MemoryFilename);
        using var writer = new StreamWriter(outputFile);
        foreach (var kv in dict
                     .Select(kv =>
                     {
                         if (initial.TryGetValue(kv.Key, out var counter) && counter != kv.Value)
                         {
                             return new KeyValuePair<string?, int>(kv.Key, kv.Value - counter);
                         }
                         return new(null, 0);
                     })
                     .Where(kv => kv.Key != null)
                     .OrderByDescending(kv => kv.Value)
                     .ThenBy(kv => kv.Key))
        {
            writer.WriteLine($"{kv.Value}|{kv.Key}");
        }
    }

    private static IEnumerable<string> GatherParents(GameObject go)
    {
        do
        {
            if (go == null)
            {
                yield break;
            }
            yield return go.name;
            go = go.transform.parent?.gameObject;
        }
        while (true);
    }
}