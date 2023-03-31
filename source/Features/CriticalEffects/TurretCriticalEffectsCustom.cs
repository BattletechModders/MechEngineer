using BattleTech;
using CustomComponents;

namespace MechEngineer.Features.CriticalEffects;

[CustomComponent("TurretCriticalEffects")]
public class TurretCriticalEffectsCustom : CriticalEffectsCustom
{
    protected override UnitType GetUnitType()
    {
        return UnitType.Turret;
    }
}