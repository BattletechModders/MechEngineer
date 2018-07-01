using System.Collections.Generic;
using BattleTech;
using BattleTech.UI;

namespace MechEngineer
{
    internal class GyroHandler : IDescription, IValidateAdd, IAdjustUpgradeDef, IAutoFixMechDef, IValidationRulesCheck
    {
        internal static GyroHandler Shared = new GyroHandler();

        private readonly ValidationHelper checker;
        private readonly AutoFixMechDefHelper fixer;
        private readonly AdjustCompDefInvSizeHelper resizer;

        private GyroHandler()
        {
            var identity = new IdentityHelper
            {
                AllowedLocations = ChassisLocations.CenterTorso,
                ComponentType = ComponentType.Upgrade,
                Prefix = Control.settings.GearGryoPrefix,
            };

            checker = new ValidationHelper(identity, this);

            fixer = new AutoFixMechDefHelper(
                identity,
                Control.settings.AutoFixMechDefGyroId,
                ComponentType.Upgrade,
                ChassisLocations.CenterTorso
            );

            resizer = new AdjustCompDefInvSizeHelper(identity, size => size == 3 ? 4 : -1);
        }

        public void AdjustUpgradeDef(UpgradeDef upgradeDef)
        {
            if (!Control.settings.AutoFixGyroUpgrades)
            {
                return;
            }

            resizer.AdjustComponentDef(upgradeDef);
        }

        public void AutoFixMechDef(MechDef mechDef, float originalTotalTonnage)
        {
            if (!Control.settings.AutoFixMechDefGyro)
            {
                return;
            }

            fixer.AutoFixMechDef(mechDef);
        }

        public string CategoryName
        {
            get { return "Gyro"; }
        }

        public void ValidateAdd(MechComponentDef newComponentDef,
            List<MechLabItemSlotElement> localInventory,
            ref string dropErrorMessage,
            ref bool result)
        {
            checker.ValidateAdd(newComponentDef, localInventory, ref dropErrorMessage, ref result);
        }

        public void ValidateMech(MechDef mechDef, Dictionary<MechValidationType, List<string>> errorMessages)
        {
            checker.ValidateMech(mechDef, errorMessages);
        }
    }
}