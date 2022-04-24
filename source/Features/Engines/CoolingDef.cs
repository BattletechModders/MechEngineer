using BattleTech;
using CustomComponents;

namespace MechEngineer.Features.Engines;

[CustomComponent("Cooling")]
public class CoolingDef : SimpleCustom<HeatSinkDef>
{
    public string HeatSinkDefId { get; set; }
}