using CustomComponents;

namespace MechEngineer
{
    [Custom("ArmorDef")]
    public class ArmorDef : CustomUpgradeDef<ArmorDef>, IWeightSavingSlotType
    {
        public int RequiredCriticalSlotCount { get; set; }
        public float WeightSavingsFactor { get; set; }
    }
}