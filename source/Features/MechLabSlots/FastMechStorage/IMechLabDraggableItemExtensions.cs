using System;
using BattleTech.UI;

namespace MechEngineer.Features.MechLabSlots.FastMechStorage;

static class IMechLabDraggableItemExtensions
{
    internal static HBSDOTweenToggle GetToggle(this IMechLabDraggableItem item)
    {
        switch (item)
        {
            case LanceLoadoutMechItem mechItem:
                return mechItem.toggleObj;
            case MechBayMechUnitElement mechUnitElement:
                return mechUnitElement.toggleObj;
            case MechBayChassisUnitElement chassisUnitElement:
                return chassisUnitElement.toggleObj;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}