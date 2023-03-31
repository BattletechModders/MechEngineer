using System;
using BattleTech;
using CustomComponents;

namespace MechEngineer.Features.CriticalEffects;

[CustomComponent("MechCriticalEffects", AllowArray = true)]
public class MechCriticalEffectsCustom : CriticalEffectsCustom
{
    public string[] UnitTypes { get; set; }= Array.Empty<string>();

    protected override UnitType GetUnitType()
    {
        return UnitType.Mech;
    }
}