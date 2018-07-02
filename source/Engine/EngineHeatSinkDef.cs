using System.Collections.Generic;
using System.Linq;
using BattleTech.Data;
using CustomComponents;

namespace MechEngineer
{
    [Custom("EngineHeatSinkDef")]
    public class EngineHeatSinkDef : CustomHeatSinkDef<EngineHeatSinkDef>
    {
        public string FullName { get; set; }
        public string Abbreviation { get; set; }
        public string Tag { get; set; }
        public string HSCategory => Tag;
    }

    internal static class DataManagerEngineHeatSinkDefExtensions
    {
        internal static IEnumerable<EngineHeatSinkDef> GetAllEngineHeatSinkDefs(this DataManager @this)
        {
            return @this.HeatSinkDefs.Keys.Select(key => @this.HeatSinkDefs.Get(key)).OfType<EngineHeatSinkDef>();
        }

        internal static EngineHeatSinkDef GetEngineHeatSinkDef(this DataManager @this, string key)
        {
            return @this.HeatSinkDefs.Get(key) as EngineHeatSinkDef;
        }

        internal static EngineHeatSinkDef GetDefaultEngineHeatSinkDef(this DataManager @this)
        {
            return GetEngineHeatSinkDef(@this, Control.settings.DefaultEngineHeatSinkId);
        }
    }
}
