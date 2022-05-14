using BattleTech;
using CustomComponents;

namespace MechEngineer.Features.CriticalEffects;

[CustomComponent("TurretCriticalEffects")]
public class TurretCriticalEffectsCustom : CriticalEffectsCustom
{
    public override UnitType GetUnitType()
    {
        return UnitType.Turret;
    }
}