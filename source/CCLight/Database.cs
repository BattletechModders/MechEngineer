using System;
using System.Collections.Generic;
using BattleTech;

namespace CustomComponents
{
    internal static class Database
    {
        internal static readonly Dictionary<string, Dictionary<Type, ICustomComponent>> CustomComponents
            = new Dictionary<string, Dictionary<Type, ICustomComponent>>();

        internal static ICustomComponent GetCustomComponent(MechComponentDef def, Type ccType)
        {
            var key = Key(def);

            if (!CustomComponents.TryGetValue(key, out var ccs))
            {
                return null;
            }

            if (!ccs.TryGetValue(ccType, out var cc))
            {
                return null;
            }

            return cc;
        }

        internal static void SetCustomComponent(MechComponentDef def, ICustomComponent cc)
        {
            var key = Key(def);

            if (!CustomComponents.TryGetValue(key, out var ccs))
            {
                ccs = new Dictionary<Type, ICustomComponent>();
                CustomComponents[key] = ccs;
            }

            var ccType = cc.GetType();
            ccs[ccType] = cc;
        }

        private static string Key(MechComponentDef def)
        {
            return def.Description.Id;
        }
    }
}