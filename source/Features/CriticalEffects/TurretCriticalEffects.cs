using BattleTech;
using CustomComponents;

namespace MechEngineer.Features.CriticalEffects;

[CustomComponent("TurretCriticalEffects")]
public class TurretCriticalEffects : CriticalEffects
{
    public override UnitType GetUnitType()
    {
        return UnitType.Turret;
    }
}