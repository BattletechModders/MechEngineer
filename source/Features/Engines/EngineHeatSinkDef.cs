using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.Data;
using CustomComponents;

namespace MechEngineer.Features.Engines
{
    [CustomComponent("EngineHeatSink")]
    public class EngineHeatSinkDef : SimpleCustom<HeatSinkDef>
    {
        public string FullName { get; set; }
        public string Abbreviation { get; set; }
        public string Tag { get; set; }
        public string HSCategory => Tag;
    }

    internal static class DataManagerEngineHeatSinkDefExtensions
    {
        internal static IEnumerable<HeatSinkDef> GetAllHeatSinkDefs(this DataManager @this)
        {
            return @this.HeatSinkDefs.Select(hsd => hsd.Value);
        }

        internal static IEnumerable<EngineHeatSinkDef> GetAllEngineHeatSinkDefs(this DataManager @this)
        {
            return @this.GetAllHeatSinkDefs().Select(d => d.GetComponent<EngineHeatSinkDef>()).Where(c => c != null);
        }

        internal static EngineHeatSinkDef GetEngineHeatSinkDef(this DataManager @this, string key)
        {
            return @this.HeatSinkDefs.Get(key)?.GetComponent<EngineHeatSinkDef>();
        }

        internal static EngineHeatSinkDef GetDefaultEngineHeatSinkDef(this DataManager @this)
        {
            return GetEngineHeatSinkDef(@this, EngineFeature.settings.DefaultEngineHeatSinkId);
        }
    }
}
