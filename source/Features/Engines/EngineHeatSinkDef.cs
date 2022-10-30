using BattleTech;
using CustomComponents;

namespace MechEngineer.Features.Engines;

[CustomComponent("EngineHeatSink")]
public class EngineHeatSinkDef : SimpleCustom<HeatSinkDef>
{
    public string FullName { get; set; } = null!;
    public string Abbreviation { get; set; } = null!;
}