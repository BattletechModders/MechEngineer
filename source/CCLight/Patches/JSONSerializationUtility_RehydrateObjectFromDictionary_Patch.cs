using System.Collections.Generic;
using System.Reflection;
using BattleTech;
using BattleTech.Data;
using Harmony;
using HBS.Util;
using MechEngineer;

namespace CustomComponents
{
    [HarmonyPatch]
    public static class JSONSerializationUtility_RehydrateObjectFromDictionary_Patch
    {
        public static MethodBase TargetMethod()
        {
            return typeof(JSONSerializationUtility).GetMethod("RehydrateObjectFromDictionary", BindingFlags.NonPublic|BindingFlags.Static);
        }

        public static void Prefix(object target, Dictionary<string, object> values)
        {
            if (!Control.settings.TestEnableAllTags)
            {
                return;
            }

            var baseTags = new[] {"ComponentTags", "MechTags"};
            foreach (var baseTag in baseTags)
            if (values.TryGetValue(baseTag, out var Tags))
            {
                if (!(Tags is Dictionary<string, object> tags))
                {
                    Control.mod.Logger.LogDebug("Tags="+Tags.GetType());
                    continue;
                }
                if (tags.TryGetValue("items", out var Items))
                {
                    if (!(Items is List<object> items))
                    {
                        Control.mod.Logger.LogDebug("items="+Items.GetType());
                        continue;
                    }

                    items.Remove("BLACKLISTED");
                    items.Remove("component_type_debug");
                    items.Remove("component_type_lostech");
                    items.Add("component_type_stock");

                    //items.Remove("unit_custom");
                    items.Add("unit_release");
                }
            }
        }

        public static void Postfix(object target, Dictionary<string, object> values)
        {
            Registry.ProcessCustomCompontentFactories(target, values);
        }
    }
}
