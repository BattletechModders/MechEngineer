using System;
using System.Linq;
using System.Text.RegularExpressions;
using BattleTech;

namespace MechEngineMod
{
    internal class EngineDef
    {
        internal readonly EngineType Type;
        internal readonly int Rating;
        internal readonly MechComponentDef Def;

        private static readonly Regex EngineNameRegex = new Regex(@"^emod_engine_(\w+)_(\d+)$", RegexOptions.Compiled);

        internal EngineDef(MechComponentDef componentDef)
        {
            var id = componentDef.Description.Id;
            var match = EngineNameRegex.Match(id);
            Type = Control.settings.EngineTypes.FirstOrDefault(c => id.StartsWith(c.Prefix));
            Rating = int.Parse(match.Groups[2].Value);
            Def = componentDef;

            Control.calc.CalcHeatSinks(this, out MinHeatSinks, out MaxHeatSinks);
        }

        public override string ToString()
        {
            return Def.Description.Id + " Rating=" + Rating;
        }

        internal int MinHeatSinks, MaxHeatSinks;

        internal int MaxAdditionalHeatSinks
        {
            get { return MaxHeatSinks - MinHeatSinks; }
        }
    }
}