using System;
using BattleTech;
using BattleTech.UI;
using fastJSON;
using HBS.Util;

namespace CustomComponents
{
    public class CustomUpgradeDef : UpgradeDef, ICustomComponent
    {
        public string CustomType { get; set; }
    }
}
