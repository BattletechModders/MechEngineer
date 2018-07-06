using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using BattleTech;
using BattleTech.UI.Tooltips;
using MechEngineer;

namespace CustomComponents
{
    public static class Registry
    {
        private static readonly Dictionary<string, CustomComponentDescriptor> Descriptors = new Dictionary<string, CustomComponentDescriptor>();

        public static void RegisterCustomTypes(Assembly assembly)
        {
            var descList = assembly.GetTypes()
                .Select(type => new { type, attributes = type.GetCustomAttributes(typeof(CustomAttribute), true) })
                .Where(@t => @t.attributes != null && @t.attributes.Length > 0)
                .Select(@t => new { @t, attribute = @t.attributes[0] as CustomAttribute })
                .Select(@t => new CustomComponentDescriptor(@t.attribute.CustomType, @t.@t.type));

            foreach (var item in descList)
            {
                if (Descriptors.TryGetValue(item.CustomType, out _))
                {
                    Descriptors[item.CustomType] = item;
                }
                else
                {
                    Descriptors.Add(item.CustomType, item);
                }

                Control.mod.Logger.Log($"added custom type {item.CustomType}");
            }
        }

        private static readonly Regex CustomTypeRegex = new Regex(@"""CustomType""\s*:\s*""([^""]+)""", RegexOptions.Compiled);
        
        internal static CustomComponentDescriptor GetDescriptorFromJSON(string json)
        {
            var match = CustomTypeRegex.Match(json);
            if (match.Success)
            {
                var customType = match.Groups[1].Value;
                return GetDescriptorFromCustomType(customType);
            }

            return null;
        }

        internal static Type GetTooltipTypeForCustomType(string customType)
        {
            return GetDescriptorFromCustomType(customType)?.TooltipType;
        }

        internal static CustomComponentDescriptor GetDescriptorFromCustomType(string customType)
        {
            if (Descriptors.TryGetValue(customType, out var value))
            {
                return value;
            }

            return null;
        }
    }

    internal class CustomComponentDescriptor
    {
        internal string CustomType { get; }
        internal Type ActualType { get; }
        internal Type TooltipType { get; }

        private readonly ConstructorInfo constructor;

        internal CustomComponentDescriptor(string customType, Type type)
        {
            CustomType = customType;
            ActualType = type;
            TooltipType = GetTooltipType(type);
            constructor = type.GetDefaultConstructor();
        }

        internal ICustomComponent CreateNew()
        {
            return constructor.Invoke() as ICustomComponent;
        }

        private static Type GetTooltipType(Type customType)
        {
            var rootType = typeof(MechComponentDef);
            var directType = customType;
            while (directType != null && directType.BaseType != rootType)
            {
                directType = directType.BaseType;
            }

            var baseInstance = directType?.GetDefaultConstructor()?.Invoke() as MechComponentDef;
            var defHandler = TooltipUtilities.MechComponentDefHandlerForTooltip(baseInstance) as MechComponentDef;
            return defHandler?.GetType();
        }
    }

    internal static class ReflectionExtension
    {
        internal static ConstructorInfo GetDefaultConstructor(this Type @this)
        {
            return @this.GetConstructor(new Type[] { });
        }
        
        internal static object Invoke(this ConstructorInfo @this)
        {
            return @this.Invoke(null);
        }
    }
}
