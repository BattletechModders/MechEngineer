using System.Collections.Generic;
using System.Linq;
using BattleTech;
using MechEngineer.Misc;

namespace MechEngineer.Features.AutoFix;

internal class AutoFixMechDefHelper : IAutoFixMechDef
{
    private readonly IIdentifier identifier;
    private readonly AddHelper adder;

    public AutoFixMechDefHelper(IdentityHelper identifier, AddHelper adder)
    {
        this.identifier = identifier;
        this.adder = adder;
    }

    public void AutoFixMechDef(MechDef mechDef)
    {
        if (mechDef.Inventory.Any(x => x.Def != null && identifier.IsCustomType(x.Def)))
        {
            return;
        }

        var componentRefs = new List<MechComponentRef>(mechDef.Inventory);

        var componentRef = new MechComponentRef(adder.ComponentDefId, null, adder.ComponentType, adder.ChassisLocation);
        componentRefs.Add(componentRef);

        mechDef.SetInventory(componentRefs.ToArray());
    }
}