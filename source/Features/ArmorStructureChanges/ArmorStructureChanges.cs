using CustomComponents;

namespace MechEngineer.Features.ArmorStructureChanges;

[CustomComponent("ArmorStructureChanges")]
public class ArmorStructureChanges : SimpleCustomComponent
{
    public float StructureFactor { get; set; } = 1;
    public float ArmorFactor { get; set; } = 1;
}