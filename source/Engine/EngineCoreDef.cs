using BattleTech;
using BattleTech.UI;
using CustomComponents;

namespace MechEngineer
{
    [CustomComponent("EngineCore")]
    public class EngineCoreDef : SimpleCustomComponent
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

        public float StandardEngineTonnage => Def.Tonnage - GyroTonnage;

        public HeatSinkDef HeatSinkDef => Def as HeatSinkDef; // TODO reintroduce GenericCustomComponent

        public override string ToString()
        {
            return Def.Description.Id + " Rating=" + Rating;
        }

        //public UIColor Color { get; } = UIColor.GoldHalf;
    }
}