using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;

namespace MechEngineer
{
    internal class ValidationHelper : IValidateDrop, IValidateMech
    {
        private readonly IDescription description;
        private readonly IIdentifier identifier;

        public bool Required = true;
        public UniqueConstraint Unique = UniqueConstraint.Location;

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
                    $"MISSING: Must mount a functional {description.CategoryName}"
                );
            }

            if (types.Count == 0)
            {
                return;
            }

            if (Unique == UniqueConstraint.Mech && types.Count > 1)
            {
                errorMessages[MechValidationType.InvalidInventorySlots].Add(
                    $"UNIQUE: Cannot mount more than one {description.CategoryName}"
                );
            }

            if (Unique == UniqueConstraint.Location && types.GroupBy(x => x.MountedLocation).Any(g => g.Count() > 1))
            {
                errorMessages[MechValidationType.InvalidInventorySlots].Add(
                    $"UNIQUE: Cannot mount more than one {description.CategoryName} in the same location"
                );
            }
        }

        public MechLabDropResult ValidateDrop(MechLabItemSlotElement dragItem, MechLabLocationWidget widget)
        {
            if (Unique == UniqueConstraint.None)
            {
                return null;
            }

            if (!identifier.IsCustomType(dragItem.ComponentRef.Def))
            {
                return null;
            }
            
            var adapter = new MechLabLocationWidgetAdapter(widget);

            if (Unique == UniqueConstraint.Location || Unique == UniqueConstraint.Mech)
            {
                var existingElement = adapter.localInventory.FirstOrDefault(s => identifier.IsCustomType(s.ComponentRef.Def));
                if (existingElement != null)
                {
                    return new MechLabDropReplaceItemResult {ToReplaceElement = existingElement};
                }
            }
            
            if (Unique == UniqueConstraint.Mech)
            {
                if (adapter.mechLab.activeMechDef.Inventory.Any(s => identifier.IsCustomType(s.Def)))
                {
                    var componentDef = dragItem.ComponentRef.Def;
                    return new MechLabDropErrorResult($"Cannot add {componentDef.Description.Name}: Cannot mount more than one {description.CategoryName}");
                }
            }

            return null;
        }
    }

    internal enum UniqueConstraint
    {
        None, Location, Mech
    }
}