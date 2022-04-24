using BattleTech;
using CustomComponents;

namespace MechEngineer.Features.CriticalEffects;

[CustomComponent("VehicleCriticalEffects")]
public class VehicleCriticalEffects : CriticalEffects
{
    public override UnitType GetUnitType()
    {
        return UnitType.Vehicle;
    }
}