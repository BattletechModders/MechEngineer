using BattleTech;
using CustomComponents;

namespace MechEngineer
{
    [Custom("EngineTypeDef")]
    public class EngineTypeDef : CustomHeatSinkDef<EngineTypeDef>, IEnginePart
    {
        public float WeightMultiplier = 1.0f;
        public string[] Requirements = { };
    }
}