using System;
using System.Text.RegularExpressions;
using BattleTech;

namespace MechEngineMod
{
    internal class Engine
    {
        internal enum EngineType
        {
            Std, XL
        }

        internal EngineType Type;
        internal int Rating;
        internal MechComponentDef Def;

        private static readonly Regex EngineNameRegex = new Regex(@"^emod_engine_(\w+)_(\d+)$", RegexOptions.Compiled);

        internal static Engine MainEngineFromDef(MechComponentDef componentDef)
        {
            if (!componentDef.IsMainEngine())
            {
                return null;
            }

            var match = EngineNameRegex.Match(componentDef.Description.Id);
            return new Engine
            {
                Type = (EngineType)Enum.Parse(typeof(EngineType), match.Groups[1].Value, true),
                Rating = int.Parse(match.Groups[2].Value),
                Def = componentDef
            };
        }

        public override string ToString()
        {
            return Def.Description.Id + " Type=" + Type + " Rating=" + Rating;
        }
    }
}