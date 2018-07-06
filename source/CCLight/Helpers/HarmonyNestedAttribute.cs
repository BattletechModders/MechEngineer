using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Harmony;
using MechEngineer;

namespace CustomComponents
{
    /// <summary>
    /// mark a patch for private nested class
    /// </summary>
    public class HarmonyNestedAttribute : HarmonyPatch
    {
        public HarmonyNestedAttribute(Type baseType, string nestedType, string method, Type[] parameters = null)
            : base(null, method)
        {
            info.originalType = baseType.GetNestedType(nestedType, BindingFlags.Static |
                                                                   BindingFlags.Instance |
                                                                   BindingFlags.Public |
                                                                   BindingFlags.NonPublic);
            info.parameter = parameters;
            info.methodName = method;
        }
    }
}
