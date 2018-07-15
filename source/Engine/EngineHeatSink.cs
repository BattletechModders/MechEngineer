using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.Data;
using BattleTech.UI;
using CustomComponents;

namespace MechEngineer
{
    [CustomComponent("EngineHeatSink")]
    public class EngineHeatSink : SimpleCustomComponent, IPreValidateDrop
    {
        public string FullName { get; set; }
        public string Abbreviation { get; set; }
        public string Tag { get; set; }
        public string HSCategory => Tag;

        public HeatSinkDef HeatSinkDef => Def as HeatSinkDef; // TODO reintroduce GenericCustomComponent


        public string PreValidateDrop(MechLabItemSlotElement item, LocationHelper location, MechLabHelper mechlab)
        {
            if (Control.settings.AllowMixingHeatSinkTypes)
                return string.Empty;

            var engine =
                mechlab.MechLab.activeMechDef.Inventory.FirstOrDefault(i => i.Def.Is<EngineCoreDef>());


            // if engine exist - check its heatsink type
            if (engine != null)
            {
                var engineRef = engine.GetEngineCoreRef();
                if (engineRef.HeatSinkDef.HSCategory != HSCategory)
                    return $"Cannot add {Def.Description.Name}: Mixing heat sink types is not allowed";
            }
            // else checking elaready installed heatsink
            else 
                if(mechlab.MechLab.activeMechDef.Inventory.Any(i => i.Def.Is<EngineHeatSink>(out var hs) && hs.HSCategory != HSCategory))
                    return $"Cannot add {Def.Description.Name}: Mixing heat sink types is not allowed";

            return string.Empty;
        }
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
