using BattleTech;

namespace MechEngineer
{
    internal class EngineTypeDef
    {
        internal readonly MechComponentDef Def;
        internal EngineType Type;

        internal EngineTypeDef(MechComponentDef componentDef, EngineType engineType)
        {
            Def = componentDef;
            Type = engineType;
        }
    }
}