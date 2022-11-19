using System;
using BattleTech;

namespace MechEngineer.Features.OverrideDescriptions;

internal static class MechComponentDefExtensions
{
    internal static string GetBonusValue(this MechComponentDef element, BonusSlot slot)
    {
        return slot switch
        {
            BonusSlot.A => element.BonusValueA,
            BonusSlot.B => element.BonusValueB,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
