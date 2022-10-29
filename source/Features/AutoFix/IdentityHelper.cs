using System;
using System.Collections.Generic;
using BattleTech;
using CustomComponents;
using MechEngineer.Helper;
using MechEngineer.Misc;

namespace MechEngineer.Features.AutoFix;

public class IdentityHelper : IIdentifier
{
    public string CategoryId { get; set; } = null!;

    public bool IsCustomType(MechComponentDef def)
    {
        return def.Is<Category>(out var category) && category.CategoryID == CategoryId;
    }
}
