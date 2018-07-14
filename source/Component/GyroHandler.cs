using System.Collections.Generic;
using BattleTech;
using BattleTech.UI;

namespace MechEngineer
{
    internal class GyroHandler : IDescription, IAdjustUpgradeDef, IAutoFixMechDef
    {
        internal static GyroHandler Shared = new GyroHandler();

        private readonly AutoFixMechDefHelper fixer;
        private readonly AdjustCompDefInvSizeHelper resizer;

        private GyroHandler()
        {
            var identity = new IdentityHelper
            {
                AllowedLocations = ChassisLocations.CenterTorso,
                ComponentType = ComponentType.Upgrade,
                Prefix = Control.settings.AutoFixGyroPrefix,
            };


            fixer = new AutoFixMechDefHelper(
                identity,
                Control.settings.AutoFixMechDefGyroId,
                ComponentType.Upgrade,
                ChassisLocations.CenterTorso
            );

            resizer = new AdjustCompDefInvSizeHelper(identity, Control.settings.AutoFixGyroSlotChange);
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
            fixer.AutoFixMechDef(mechDef, originalTotalTonnage);
        }

        public string CategoryName
        {
            get { return "Gyro"; }
        }
    }
}