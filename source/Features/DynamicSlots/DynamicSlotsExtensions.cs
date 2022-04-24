using BattleTech.UI;

namespace MechEngineer.Features.DynamicSlots;

internal static class DynamicSlotsExtensions
{
    internal static bool IsDynamicSlotElement(this MechLabItemSlotElement element)
    {
        return element.DropParent == null;
    }
}