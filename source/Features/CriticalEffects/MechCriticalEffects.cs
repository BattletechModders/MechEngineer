using BattleTech;
using CustomComponents;

namespace MechEngineer.Features.CriticalEffects;

[CustomComponent("MechCriticalEffects")]
public class MechCriticalEffects : CriticalEffects
{
    public override UnitType GetUnitType()
    {
        return UnitType.Mech;
    }
}