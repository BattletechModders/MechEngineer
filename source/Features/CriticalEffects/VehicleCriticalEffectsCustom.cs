using BattleTech;
using CustomComponents;

namespace MechEngineer.Features.CriticalEffects;

[CustomComponent("VehicleCriticalEffects")]
public class VehicleCriticalEffectsCustom : CriticalEffectsCustom
{
    public override UnitType GetUnitType()
    {
        return UnitType.Vehicle;
    }
}