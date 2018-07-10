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

            if (result.CoreRef == null || result.Type == null)
            {
                return null;
            }

            return new Engine(result.CoreRef, result.Type, result.ExternalHeatSinkTonnage);
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

        internal static EngineCoreDef GetEngineCoreDef(this IEnumerable<MechComponentRef> componentRefs)
        {
            var result = EngineSearcher.SearchInventory(componentRefs);
            return result.CoreRef?.CoreDef;
        }
    }
}