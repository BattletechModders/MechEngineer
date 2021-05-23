using System;
using System.IO;
using BattleTech.ModSupport.Utils;
using Harmony;
using HBS.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MechEngineer.Features.ModTekLoader.Patches
{
    [HarmonyPatch(typeof(AssetBundle), nameof(AssetBundle.LoadAsset), typeof(string), typeof(Type))]
    public static class AssetBundle_LoadAsset_Patch
    {
        public static void Postfix(AssetBundle __instance, string name, Type type, ref Object __result)
        {
            try
            {
                if (typeof(TextAsset) != type || !ModTekLoaderFeature.Shared.Manifest.TryGetValue(name, out var path))
                {
                    return;
                }

                Control.Logger.Debug?.Log($"Merging resource {name} from AssetBundle {__instance.name}");
                var ta = (TextAsset) __result;
                
                var originalStrippedString = Traverse.Create(typeof(JSONSerializationUtility)).Method("StripHBSCommentsFromJSON", ta.text).GetValue<string>();
                var originalAsJObject = JObject.Parse(originalStrippedString);

                var mergeAsJObject = JObject.Parse(File.ReadAllText(path));

                JSONMerger.MergeIntoTarget(originalAsJObject, mergeAsJObject);

                var result = originalAsJObject.ToString(Formatting.Indented);

                __result = new TextAsset(result); //maybe a memory leak
            }
            catch (Exception e)
            {
                Control.Logger.Error.Log(e);
            }
        }
    }
}