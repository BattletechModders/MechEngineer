using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.Data;
using BattleTech.UI;
using CustomComponents;

namespace MechEngineer
{
    [CustomComponent("EngineHeatSinkKit")]
    public class EngineHeatSinkKit : SimpleCustomComponent
    {
        public string HeatSinkDefId { get; set; }
    }

    internal static class DataManagerEngineHeatSinkKitDefExtensions
    {
        internal static IEnumerable<EngineHeatSinkKit> GetAllEngineHeatSinkKitDefs(this DataManager @this)
        {
            return @this.GetAllHeatSinkDefs().Select(d => d.GetComponent<EngineHeatSinkKit>()).Where(c => c != null);
        }
    }
}
