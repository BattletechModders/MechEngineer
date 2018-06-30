using System.Collections.Generic;
using BattleTech;

namespace MechEngineer
{
    internal static class EngineConstructExtensions
    {
        internal static Engine GetEngineConstruct(this MechDef @this)
        {
            return GetEngineConstruct(@this.Inventory);
        }

        internal static Engine GetEngineConstruct(this IEnumerable<MechComponentRef> componentRefs)
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
            return result.CoreRef.CoreDef;
        }

        internal static EngineCoreDef GetEngineCoreDef(this MechComponentDef @this)
        {
            if (@this == null || !@this.IsEngineCore())
            {
                return null;
            }

            return new EngineCoreDef(@this);
        }

        internal static bool IsEnginePart(this MechComponentDef componentDef)
        {
            return componentDef.CheckComponentDef(ComponentType.HeatSink, Control.settings.EnginePartPrefix);
        }
    }
}