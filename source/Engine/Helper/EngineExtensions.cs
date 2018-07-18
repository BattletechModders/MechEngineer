using System.Collections.Generic;
using BattleTech;
using CustomComponents;

namespace MechEngineer
{
    internal static class EngineExtensions
    {
        internal static Engine GetEngine(this MechDef @this)
        {
            return GetEngine(@this.Inventory);
        }

        internal static Engine GetEngine(this IEnumerable<MechComponentRef> componentRefs)
        {
            var result = EngineSearcher.SearchInventory(componentRefs);

            if (result.CoreRef == null)
            {
                return null;
            }

            return new Engine(result.CoreRef, result.Weights, result.HeatSinks);
        }

        internal static EngineCoreRef GetEngineCoreRef(this IEnumerable<MechComponentRef> componentRefs)
        {
            var result = EngineSearcher.SearchInventory(componentRefs);
            return result.CoreRef;
        }

        internal static EngineCoreRef GetEngineCoreRef(this MechComponentRef @this)
        {
            var engineDef = @this?.Def?.GetComponent<EngineCoreDef>();
            if (engineDef == null)
            {
                return null;
            }

            return new EngineCoreRef(@this, engineDef);
        }

        internal static bool IsEnginePart(this MechComponentDef componentDef)
        {
            return componentDef.Is<Flags>(out var flags) && flags.IsSet("engine_part");
        }
    }
}