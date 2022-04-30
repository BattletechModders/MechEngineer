using System.Linq;
using BattleTech;
using CustomComponents;

namespace MechEngineer.Features.Engines.Helper;

// Extensions method became public for BV reasons -- bhtrail
public static class EngineExtensions
{
    // Extensions method became public for BV reasons -- bhtrail
    public static Engine GetEngine(this MechDef @this)
    {
        return Engine.GetEngine(@this.Chassis, @this.Inventory);
    }

    // Extensions method became public for BV reasons -- bhtrail
    public static bool HasDestroyedEngine(this MechDef mechDef)
    {
        return mechDef.Inventory.Any(x => x.DamageLevel == ComponentDamageLevel.Destroyed && x.Is<EngineCoreDef>());
    }
}