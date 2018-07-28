using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.Data;
using BattleTech.UI;
using CustomComponents;

namespace MechEngineer
{
    [CustomComponent("EngineHeatSink")]
    public class EngineHeatSink : SimpleCustomComponent
    {
        public string FullName { get; set; }
        public string Abbreviation { get; set; }
        public string Tag { get; set; }
        public string HSCategory => Tag;

        public HeatSinkDef HeatSinkDef => Def as HeatSinkDef; // TODO reintroduce GenericCustomComponent
    }

    internal static class DataManagerEngineHeatSinkDefExtensions
    {
        internal static IEnumerable<HeatSinkDef> GetAllHeatSinkDefs(this DataManager @this)
        {
            return @this.HeatSinkDefs.Select(hsd => hsd.Value);
        }

        internal static IEnumerable<EngineHeatSink> GetAllEngineHeatSinkDefs(this DataManager @this)
        {
            return @this.GetAllHeatSinkDefs().Select(d => d.GetComponent<EngineHeatSink>()).Where(c => c != null);
        }

        internal static EngineHeatSink GetEngineHeatSinkDef(this DataManager @this, string key)
        {
            return @this.HeatSinkDefs.Get(key)?.GetComponent<EngineHeatSink>();
        }

        internal static EngineHeatSink GetDefaultEngineHeatSinkDef(this DataManager @this)
        {
            return GetEngineHeatSinkDef(@this, Control.settings.DefaultEngineHeatSinkId);
        }
    }
}
