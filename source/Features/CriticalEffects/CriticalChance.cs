using CustomComponents;

namespace MechEngineer.Features.CriticalEffects;

[CustomComponent("CriticalChance")]
public class CriticalChance : SimpleCustomComponent
{
    public int Size { get; set; } // allows the override the inventory size in a location
}