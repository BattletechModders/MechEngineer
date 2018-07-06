using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;

namespace MechEngineer
{
    internal class CockpitHandler : IDescription, IValidateDrop, IAdjustUpgradeDef, IAutoFixMechDef, IValidateMech
    {
        internal static CockpitHandler Shared = new CockpitHandler();
        
        private readonly IdentityHelper identity;
        private readonly ValidationHelper checker;
        private readonly AutoFixMechDefHelper fixer;
        private readonly AdjustCompDefTonnageHelper resizer;

        private CockpitHandler()
        {
            identity = new IdentityHelper
            {
                AllowedLocations = ChassisLocations.Head,
                ComponentType = ComponentType.Upgrade,
                Prefix = Control.settings.AutoFixCockpitPrefix,
            };

            checker = new ValidationHelper(identity, this);

            fixer = new AutoFixMechDefHelper(
                identity,
                Control.settings.AutoFixMechDefCockpitId,
                ComponentType.Upgrade,
                ChassisLocations.Head
            );

            resizer = new AdjustCompDefTonnageHelper(identity, ton => ton < 0.1f ? 3 : -1);
        }

        public bool ProtectsAgainstShutdownInjury(MechDef mechDef)
        {
            return mechDef.Inventory.Any(c => c.DamageLevel == ComponentDamageLevel.Functional && identity.IsCustomType(c.Def));
        }

        public void AdjustUpgradeDef(UpgradeDef upgradeDef)
        {
            if (!Control.settings.AutoFixCockpitUpgrades)
            {
                return;
            }

            resizer.AdjustComponentDef(upgradeDef);
        }

        public void AutoFixMechDef(MechDef mechDef, float originalTotalTonnage)
        {
            fixer.AutoFixMechDef(mechDef, originalTotalTonnage);
        }

        public string CategoryName
        {
            get { return "Cockpit"; }
        }

        public MechLabDropResult ValidateDrop(MechLabItemSlotElement dragItem, MechLabLocationWidget widget)
        {
            return checker.ValidateDrop(dragItem, widget);
        }

        public void ValidateMech(MechDef mechDef, Dictionary<MechValidationType, List<string>> errorMessages)
        {
            checker.ValidateMech(mechDef, errorMessages);
        }
    }
}