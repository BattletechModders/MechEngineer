using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.Data;
using BattleTech.UI;
using CustomComponents;

namespace MechEngineer
{
    [Custom("EngineHeatSinkKitDef")]
    public class EngineHeatSinkKitDef : CustomHeatSinkDef
    {
        public string HeatSinkDefId { get; set; }

        public EngineHeatSinkDef HeatSinkDef => dataManager.Get(BattleTechResourceType.HeatSinkDef, HeatSinkDefId) as EngineHeatSinkDef;
    }

    internal static class DataManagerEngineHeatSinkKitDefExtensions
    {
        internal static IEnumerable<EngineHeatSinkKitDef> GetAllEngineHeatSinkKitDefDefs(this DataManager @this)
        {
            return @this.HeatSinkDefs.Keys.Select(key => @this.HeatSinkDefs.Get(key)).OfType<EngineHeatSinkKitDef>();
        }
    }
}
