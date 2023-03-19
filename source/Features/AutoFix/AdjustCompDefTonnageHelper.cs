using System;
using BattleTech;
using MechEngineer.Misc;

namespace MechEngineer.Features.AutoFix;

internal class AdjustCompDefTonnageHelper
{
    private readonly IIdentifier identifier;
    private readonly ValueChange<float> change;

    internal AdjustCompDefTonnageHelper(IIdentifier identifier, ValueChange<float> change)
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

        var newTonnage = change.Change(def.Tonnage);
        if (!newTonnage.HasValue)
        {
            return;
        }

        def.Tonnage = newTonnage.Value;
    }
}