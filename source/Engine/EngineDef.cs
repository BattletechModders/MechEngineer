using System;
using System.Text.RegularExpressions;
using BattleTech;

namespace MechEngineMod
{
    internal class EngineDef
    {
        internal enum EngineType
        {
            Std, XL
        }

        internal readonly EngineType Type;
        internal readonly int Rating;
        internal readonly MechComponentDef Def;

        private static readonly Regex EngineNameRegex = new Regex(@"^emod_engine_(\w+)_(\d+)$", RegexOptions.Compiled);

        internal EngineDef(MechComponentDef componentDef)
        {
            var match = EngineNameRegex.Match(componentDef.Description.Id);
            Type = (EngineType) Enum.Parse(typeof(EngineType), match.Groups[1].Value, true);
            Rating = int.Parse(match.Groups[2].Value);
            Def = componentDef;
        }

        public override string ToString()
        {
            return Def.Description.Id + " Type=" + Type + " Rating=" + Rating;
        }
    }
}