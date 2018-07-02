using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;

namespace MechEngineer
{
    internal class ValidationHelper : IValidateAdd, IValidateMech
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

        public void ValidateMech(MechDef mechDef, Dictionary<MechValidationType, List<string>> errorMessages)
        {
            var types = mechDef.Inventory
                .Where(x => x.Def != null && identifier.IsCustomType(x.Def))
                .ToList();

            if (Required && types.Count(x => x.DamageLevel == ComponentDamageLevel.Functional) == 0)
            {
                errorMessages[MechValidationType.InvalidInventorySlots].Add(
                    string.Format("MISSING: Must mount a functional {0}", description.CategoryName)
                );
            }

            if (Unique && types.Count > 1)
            {
                errorMessages[MechValidationType.InvalidInventorySlots].Add(
                    string.Format("UNIQUE: Can't mount more than one {0}", description.CategoryName)
                );
            }
        }

        public void ValidateAdd(
            MechComponentDef newComponentDef,
            List<MechLabItemSlotElement> localInventory,
            ref string dropErrorMessage,
            ref bool result)
        {
            if (!Unique)
            {
                return;
            }

            if (!result)
            {
                return;
            }

            if (!identifier.IsCustomType(newComponentDef))
            {
                return;
            }

            if (localInventory.Select(x => x.ComponentRef).All(x => x == null || !identifier.IsCustomType(x.Def)))
            {
                return;
            }

            dropErrorMessage = string.Format("Cannot add {0}: {1} is already installed", newComponentDef.Description.Name, description.CategoryName);
            result = false;
        }
    }
}