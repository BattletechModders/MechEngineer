using System;
using BattleTech.UI;
using BattleTech.UI.TMProWrapper;

namespace MechEngineer.Features.OverrideDescriptions;

internal static class MechLabItemSlotElementExtensions
{
    internal static LocalizableText GetBonusText(this MechLabItemSlotElement element, BonusSlot slot)
    {
        return slot switch
        {
            BonusSlot.A => element.bonusTextA,
            BonusSlot.B => element.bonusTextB,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
