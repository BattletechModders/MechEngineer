using System.Linq;
using BattleTech;
using CustomComponents;

namespace MechEngineer.Features.Engines.Helper
{
    internal static class EngineExtensions
    {
        internal static Engine GetEngine(this MechDef @this)
        {
            return Engine.GetEngine(@this.Chassis, @this.Inventory);
        }

        internal static bool HasDestroyedEngine(this MechDef mechDef)
        {
            return mechDef.Inventory.Any(x => x.DamageLevel == ComponentDamageLevel.Destroyed && x.Is<EngineCoreDef>());
        }
    }
}