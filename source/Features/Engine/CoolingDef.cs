using BattleTech;
using BattleTech.UI.Tooltips;
using CustomComponents;

namespace MechEngineer
{
    [CustomComponent("Cooling")]
    public class CoolingDef : SimpleCustom<HeatSinkDef>
    {
        public string HeatSinkDefId { get; set; }
    }
}
