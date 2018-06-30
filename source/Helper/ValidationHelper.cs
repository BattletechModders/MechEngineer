using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;

namespace MechEngineer
{
    internal class ValidationHelper
    {
        private readonly IDescription description;
        private readonly IIdentifier identifier;

        public bool Required = true;
        public bool Unique = true;

        public ValidationHelper(IIdentifier identifier, IDescription description)
        {
            this.identifier = identifier;
            this.description = description;
        }

        internal void Check(MechDef mechDef, Dictionary<MechValidationType, List<string>> errorMessages)
        {
            var count = mechDef.Inventory
                .Where(x => x.DamageLevel == ComponentDamageLevel.Functional)
                .Count(x => x.Def != null && identifier.IsComponentDef(x.Def));

            if (Required && count == 0)
            {
                errorMessages[MechValidationType.InvalidInventorySlots].Add(
                    string.Format("MISSING: Must mount a functional {0}", description.CategoryName)
                );
            }

            if (Unique && count > 1)
            {
                errorMessages[MechValidationType.InvalidInventorySlots].Add(
                    string.Format("UNIQUE: Can't mount more than one {0}", description.CategoryName)
                );
            }
        }

        internal void ValidateAdd(
            MechComponentDef newComponentDef,
            List<MechLabItemSlotElement> localInventory,
            ref string dropErrorMessage,
            ref bool result)
        {
            if (!result)
            {
                return;
            }

            if (!identifier.IsComponentDef(newComponentDef))
            {
                return;
            }

            if (localInventory.Select(x => x.ComponentRef).All(x => x == null || !identifier.IsComponentDef(x.Def)))
            {
                return;
            }

            dropErrorMessage = string.Format("Cannot add {0}: {1} is already installed", newComponentDef.Description.Name, description.CategoryName);
            result = false;
        }
    }
}