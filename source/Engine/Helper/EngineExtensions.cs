using System.Collections.Generic;
using BattleTech;

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

            if (result.CoreRef == null || result.TypeDef == null)
            {
                return null;
            }

            return new Engine(result.CoreRef, result.TypeDef, result.Parts);
        }

        internal static EngineCoreRef GetEngineCoreRef(this IEnumerable<MechComponentRef> componentRefs)
        {
            var result = EngineSearcher.SearchInventory(componentRefs);
            return result.CoreRef;
        }

        internal static EngineCoreRef GetEngineCoreRef(this MechComponentRef @this)
        {
            if (@this == null || @this.Def == null)
            {
                return null;
            }

            var engineDef = @this.Def.GetEngineCoreDef();
            if (engineDef == null)
            {
                return null;
            }

            return new EngineCoreRef(@this, engineDef);
        }

        internal static EngineCoreDef GetEngineCoreDef(this IEnumerable<MechComponentRef> componentRefs)
        {
            var result = EngineSearcher.SearchInventory(componentRefs);
            return result.CoreRef == null ? null : result.CoreRef.CoreDef;
        }

        internal static EngineCoreDef GetEngineCoreDef(this MechComponentDef @this)
        {
            return @this as EngineCoreDef;
        }

        internal static bool IsEnginePart(this MechComponentDef componentDef)
        {
            return componentDef.CheckComponentDef(ComponentType.HeatSink, Control.settings.EnginePartPrefix);
        }
    }
}