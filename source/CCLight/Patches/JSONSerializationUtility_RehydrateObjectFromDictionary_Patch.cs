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

        public static void Postfix(object target, Dictionary<string, object> values)
        {
            Registry.ProcessCustomCompontentFactories(target, values);
        }
    }
}
