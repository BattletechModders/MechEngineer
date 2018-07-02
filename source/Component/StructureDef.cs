using CustomComponents;

namespace MechEngineer
{
    [Custom("StructureDef")]
    public class StructureDef : CustomUpgradeDef<StructureDef>, IWeightSavingSlotType
    {
        public int RequiredCriticalSlotCount { get; set; }
        public float WeightSavingsFactor { get; set; }
    }
}