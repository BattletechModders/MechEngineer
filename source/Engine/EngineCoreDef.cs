using CustomComponents;

namespace MechEngineer
{
    [Custom("EngineCoreDef")]
    public class EngineCoreDef : CustomHeatSinkDef<EngineCoreDef>, IEnginePart
    {
        private int _rating;

        public int Rating
        {
            get { return _rating; }
            set { _rating = value;
                Control.calc.CalcHeatSinks(this, out MinHeatSinks, out MaxHeatSinks);
            }
        }

        public int MinHeatSinks, MaxHeatSinks;

        public int MaxAdditionalHeatSinks => MaxHeatSinks - MinHeatSinks;

        public float GyroTonnage => Control.calc.CalcGyroWeight(this);

        public float StandardEngineTonnage => Tonnage - GyroTonnage;

        public override string ToString()
        {
            return Description.Id + " Rating=" + Rating;
        }
    }
}