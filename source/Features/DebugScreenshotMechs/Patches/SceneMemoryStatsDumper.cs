using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BattleTech;
using BattleTech.UI.TMProWrapper;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MechEngineer.Features.DebugScreenshotMechs.Patches;

internal static class SceneMemoryStatsDumper
{
    internal const string MemoryFilename = "gameobjects.txt";
    private static readonly string OutputFile = Path.Combine(Control.Mod.Directory, MemoryFilename);

    private static Dictionary<string, int>? initial;

    internal static void DumpUiStuff()
    {
        var dict = new Dictionary<string, int>();
        foreach (var obj in Resources.FindObjectsOfTypeAll<Object>())
        {
            if (obj is Component && obj is not LocalizableText)
            {
                continue;
            }

            string key;
            if (obj is GameObject go)
            {
                key = GetParentBreadCrumb(go);
            }
            else if (obj is Mesh mesh)
            {
                key = obj.GetType().Name + "|" + (Mesh_Constructor_Patch.ids.TryGetValue(mesh.GetInstanceID(), out var caller) ? caller : "<unknown>");
            }
            else if (obj is Component component)
            {
                //  + "|" + obj.name
                key = obj.GetType().Name + "|" + GetParentBreadCrumb(component.gameObject);
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
        File.WriteAllLines(OutputFile,
        UnityGameInstance.BattleTechGame.DataManager.GameObjectPool.gameObjectPool
                .OrderByDescending(kv => kv.Value.Count)
                .ThenBy(kv => kv.Key)
                .Select(kv => $"{kv.Value.Count}|Pool {kv.Key} ")
                .Concat(
                    dict
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
                        .ThenBy(kv => kv.Key)
                        .Select(kv => $"{kv.Value}|{kv.Key}"))
        );
    }

    private static string GetParentBreadCrumb(GameObject go)
    {
        string? tmpKey = null;
        void AddName(string name)
        {
            if (tmpKey == null)
            {
                tmpKey = name;
            }
            else
            {
                tmpKey += " -> " + name;
            }
        }
        var uiCount = 0;
        foreach (var name in GatherParents(go).Reverse())
        {
            if (uiCount == 3)
            {
                break;
            }
            if (name.StartsWith("uix", StringComparison.OrdinalIgnoreCase))
            {
                uiCount++;
                AddName(name);
            }
            else if (uiCount < 1)
            {
                AddName(name);
            }
        }
        return tmpKey ?? "<empty>";
    }

    private static IEnumerable<string> GatherParents(GameObject? go)
    {
        do
        {
            if (go == null)
            {
                yield break;
            }
            yield return go.name;
            // ReSharper disable once Unity.NoNullPropagation
            go = go.transform.parent?.gameObject;
        }
        while (true);
    }
}