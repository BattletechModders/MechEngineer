using CustomComponents;

namespace MechEngineer
{
    [CustomComponent("ArmorStructureChanges")]
    public class ArmorStructureChanges : SimpleCustomComponent
    {
        public float StructureFactor { get; set; } = 1;
        public float ArmorFactor { get; set; } = 1;
    }
}