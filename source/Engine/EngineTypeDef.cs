using BattleTech;
using BattleTech.UI;
using CustomComponents;

namespace MechEngineer
{
    [Custom("EngineTypeDef")]
    public class EngineTypeDef : CustomHeatSinkDef, IEnginePart
    {
        public float WeightMultiplier = 1.0f;
        public string[] Requirements = { };

        //public UIColor Color { get; } = UIColor.GoldHalf;
    }
}