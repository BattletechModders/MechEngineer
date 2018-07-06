using System;
using BattleTech;
using fastJSON;

namespace CustomComponents
{
    public class CustomHeatSinkDef : HeatSinkDef, ICustomComponent
    {
        public string CustomType { get; set; }
    }
}
