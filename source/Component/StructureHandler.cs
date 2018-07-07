using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using CustomComponents;

namespace MechEngineer
{
    internal class StructureHandler : IIdentifier, IDescription, IValidateDrop, IValidateMech, IProcessWeaponHit
    {
        internal static StructureHandler Shared = new StructureHandler();

        private readonly IdentityHelper identity;
        private readonly ValidationHelper checker;

        private StructureHandler()
        {
            identity = new IdentityHelper { Prefix = "emod_structureslots_" };

            checker = new ValidationHelper(this, this) {Required = false, Unique = UniqueConstraint.Mech};
        }

        public string CategoryName { get; } = "Structure";

        public bool IsCustomType(MechComponentDef def)
        {
            return identity.IsCustomType(def);
        }

        public MechLabDropResult ValidateDrop(MechLabItemSlotElement dragItem, MechLabLocationWidget widget)
        {
            return checker.ValidateDrop(dragItem, widget);
        }

        public void ValidateMech(MechDef mechDef, Dictionary<MechValidationType, List<string>> errorMessages)
        {
            checker.ValidateMech(mechDef, errorMessages);
        }

        public bool ProcessWeaponHit(MechComponent mechComponent, WeaponHitInfo hitInfo, ComponentDamageLevel damageLevel, bool applyEffects)
        {
            if (IsCustomType(mechComponent.componentDef))
            {
                return false;
            }

            return true;
        }
    }
}