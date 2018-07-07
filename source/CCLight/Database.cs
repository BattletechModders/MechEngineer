using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;

namespace CustomComponents
{
    internal static class Database
    {
        internal static readonly Dictionary<string, List<ICustomComponent>> CustomComponents
            = new Dictionary<string, List<ICustomComponent>>();

        internal static T GetCustomComponent<T>(MechComponentDef def) where T: class, ICustomComponent
        {
            var key = Key(def);

            if (!CustomComponents.TryGetValue(key, out var ccs))
            {
                return null;
            }

            return ccs.OfType<T>().FirstOrDefault();
        }

        internal static void SetCustomComponent(MechComponentDef def, ICustomComponent cc)
        {
            var key = Key(def);

            if (!CustomComponents.TryGetValue(key, out var ccs))
            {
                ccs = new List<ICustomComponent>();
                CustomComponents[key] = ccs;
            }

            ccs.Add(cc);
        }

        private static string Key(MechComponentDef def)
        {
            return def.Description.Id;
        }
    }
}