using BattleTech;
using CustomComponents;

namespace MechEngineer.Features.Engines;

[CustomComponent("EngineHeatSink")]
public class EngineHeatSinkDef : SimpleCustom<HeatSinkDef>
{
    public string FullName { get; set; }
    public string Abbreviation { get; set; }
    public string Tag { get; set; }
    public string HSCategory => Tag;
}