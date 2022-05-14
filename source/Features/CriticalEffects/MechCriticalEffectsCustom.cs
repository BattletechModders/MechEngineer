using BattleTech;
using CustomComponents;

namespace MechEngineer.Features.CriticalEffects;

[CustomComponent("MechCriticalEffects")]
public class MechCriticalEffectsCustom : CriticalEffectsCustom
{
    public override UnitType GetUnitType()
    {
        return UnitType.Mech;
    }
}