using System.Linq;
using BattleTech;
using BattleTech.UI;
using CustomComponents;

namespace MechEngineer.Features.Engines.Helper
{
    internal static class EngineExtensions
    {
        internal static Engine GetEngine(this MechDef @this)
        {
            return Engine.GetEngine(@this.Chassis, @this.Inventory);
        }

        internal static Engine GetEngine(this MechLabPanel @this)
        {
            return Engine.GetEngine(@this.activeMechDef.Chassis, @this.activeMechInventory);
        }

        internal static bool HasDestroyedEngine(this MechDef mechDef)
        {
            return mechDef.Inventory.Any(x => x.DamageLevel == ComponentDamageLevel.Destroyed && x.Is<EngineCoreDef>());
        }
    }
}