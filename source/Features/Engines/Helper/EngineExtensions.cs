using System.Linq;
using BattleTech;
using CustomComponents;
using MechEngineer.Misc;

namespace MechEngineer.Features.Engines.Helper;

[UsedBy(User.BattleValue)]
public static class EngineExtensions
{
    [UsedBy(User.BattleValue)]
    public static Engine? GetEngine(this MechDef @this)
    {
        return Engine.GetEngine(@this.Chassis, @this.Inventory);
    }

    [UsedBy(User.BattleValue)]
    public static bool HasDestroyedEngine(this MechDef mechDef)
    {
        return mechDef.Inventory.Any(x => x.DamageLevel == ComponentDamageLevel.Destroyed && x.Is<EngineCoreDef>());
    }
}