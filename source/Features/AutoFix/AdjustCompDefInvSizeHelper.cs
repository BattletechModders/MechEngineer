using System;
using BattleTech;
using MechEngineer.Misc;

namespace MechEngineer.Features.AutoFix;

internal class AdjustCompDefInvSizeHelper
{
    private readonly IIdentifier identifier;
    private readonly ValueChange<int> change;

    internal AdjustCompDefInvSizeHelper(IIdentifier identifier, ValueChange<int> change)
    {
        this.identifier = identifier;
        this.change = change;
    }

    internal void AdjustComponentDef(MechComponentDef def)
    {
        if (change == null)
        {
            return;
        }

        if (!identifier.IsCustomType(def))
        {
            return;
        }

        var newSize = change.Change(def.InventorySize);
        if (!newSize.HasValue)
        {
            return;
        }

        def.InventorySize = newSize.Value;
    }
}