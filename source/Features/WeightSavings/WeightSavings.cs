using CustomComponents;

namespace MechEngineer
{
    [CustomComponent("WeightSavings")]
    public class WeightSavings : SimpleCustomComponent
    {
        public int RequiredSlots { get; set; }
        public float ArmorWeightSavingsFactor { get; set; }
        public float StructureWeightSavingsFactor { get; set; }
    }
}