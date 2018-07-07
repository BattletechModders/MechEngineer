using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BattleTech;
using MechEngineer;

namespace CustomComponents
{
    public static class Registry
    {
        private static readonly List<ICustomComponentFactory> Factories = new List<ICustomComponentFactory>();

        public static void RegisterFactory(ICustomComponentFactory factory)
        {
            Factories.Add(factory);
        }

        public static void RegisterSimpleCustomComponents(Assembly assembly)
        {
            var sccType = typeof(SimpleCustomComponent);
            foreach (var type in assembly.GetTypes().Where(t => sccType.IsAssignableFrom(t)))
            {
                var factoryGenericType = typeof(SimpleCustomComponentFactory<>);
                var genericTypes = new[] { type };
                var factoryType = factoryGenericType.MakeGenericType(genericTypes);
                var factory = Activator.CreateInstance(factoryType) as ICustomComponentFactory;
                Factories.Add(factory);
            }
        }

        internal static void ProcessCustomCompontentFactories(object target, Dictionary<string, object> values)
        {
            if (!(target is MechComponentDef componentDef))
            {
                return;
            }

            foreach (var factory in Factories)
            {
                var component = factory.Create(componentDef, values);
                if (component == null)
                {
                    continue;
                }
                Database.SetCustomComponent(componentDef, component);
            }
        }
    }
}
