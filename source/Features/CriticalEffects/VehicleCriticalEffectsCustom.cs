using BattleTech;
using CustomComponents;

namespace MechEngineer.Features.CriticalEffects;

[CustomComponent("VehicleCriticalEffects")]
public class VehicleCriticalEffectsCustom : CriticalEffectsCustom
{
    protected override UnitType GetUnitType()
    {
        return UnitType.Vehicle;
    }
}