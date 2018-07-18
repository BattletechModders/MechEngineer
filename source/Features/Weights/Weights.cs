using CustomComponents;

namespace MechEngineer
{
    [CustomComponent("Weights")]
    public class Weights : SimpleCustomComponent
    {
        public int ReservedSlots { get; set; } = 0;
        public float ArmorFactor { get; set; } = 1;
        public float StructureFactor { get; set; } = 1;
        public float EngineFactor { get; set; } = 1;
        public float GyroFactor { get; set; } = 1;

        public void Combine(Weights savings)
        {
            ReservedSlots *= savings.ReservedSlots;
            ArmorFactor *= savings.ArmorFactor;
            StructureFactor *= savings.StructureFactor;
            EngineFactor *= savings.EngineFactor;
            GyroFactor *= savings.GyroFactor;
        }
    }
}