using System.Collections.Generic;
using System.Linq;
using BattleTech;

namespace MechEngineer
{
    internal class AutoFixMechDefHelper
    {
        private readonly string defId;
        private readonly IIdentifier identifier;
        private readonly ChassisLocations location;
        private readonly ComponentType type;

        internal AutoFixMechDefHelper(IIdentifier identifier, string defId, ComponentType type, ChassisLocations location)
        {
            this.identifier = identifier;
            this.defId = defId;
            this.type = type;
            this.location = location;
        }

        internal void AutoFixMechDef(MechDef mechDef)
        {
            if (mechDef.Inventory.Any(x => x.Def != null && identifier.IsCustomType(x.Def)))
            {
                return;
            }

            var componentRefs = new List<MechComponentRef>(mechDef.Inventory);

            var componentRef = new MechComponentRef(defId, null, type, location);
            componentRefs.Add(componentRef);

            mechDef.SetInventory(componentRefs.ToArray());
        }
    }
}