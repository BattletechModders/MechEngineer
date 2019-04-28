using System.Collections.Generic;
using System.Linq;
using BattleTech;
using CustomComponents;

namespace MechEngineer.Features.Engine.Helper
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

            if (result.CoolingDef == null || result.CoreDef == null || result.HeatBlockDef == null)
            {
                return null;
            }

            return new Engine(result.CoolingDef, result.HeatBlockDef, result.CoreDef, result.Weights, result.HeatSinks);
        }

        internal static bool IsEnginePart(this MechComponentDef componentDef)
        {
            return componentDef.IsCategory("EnginePart");
        }

        internal static bool HasDestroyedEngine(this MechDef mechDef)
        {
            return mechDef.Inventory.Any(x => x.DamageLevel == ComponentDamageLevel.Destroyed && x.Is<EngineCoreDef>());
        }
    }
}