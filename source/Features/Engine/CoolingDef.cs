using BattleTech;
using CustomComponents;

namespace MechEngineer.Features.Engine
{
    [CustomComponent("Cooling")]
    public class CoolingDef : SimpleCustom<HeatSinkDef>
    {
        public string HeatSinkDefId { get; set; }
    }
}
