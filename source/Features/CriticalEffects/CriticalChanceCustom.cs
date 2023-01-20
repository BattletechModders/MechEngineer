using CustomComponents;

namespace MechEngineer.Features.CriticalEffects;

[CustomComponent("CriticalChance")]
public class CriticalChanceCustom : SimpleCustomComponent, IValueComponent<int>
{
    public int Size { get; set; } // allows the override the inventory size in a location

    public void LoadValue(int value)
    {
        Size = value;
    }
}