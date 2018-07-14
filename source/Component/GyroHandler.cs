using System.Collections.Generic;
using BattleTech;
using BattleTech.UI;
using CustomComponents;

namespace MechEngineer
{
    internal class GyroHandler : IAdjustUpgradeDef, IAutoFixMechDef, IPreProcessor
    {
        internal static GyroHandler Shared = new GyroHandler();

        private readonly IdentityHelper identity;
        private readonly AutoFixMechDefHelper fixer;
        private readonly AdjustCompDefInvSizeHelper resizer;

        private GyroHandler()
        {
            identity = new IdentityHelper
            {
                AllowedLocations = ChassisLocations.CenterTorso,
                ComponentType = ComponentType.Upgrade,
                Prefix = Control.settings.AutoFixGyroPrefix,
                CategoryId = Control.settings.AutoFixGyroCategoryId,
            };


            fixer = new AutoFixMechDefHelper(
                identity,
                Control.settings.AutoFixMechDefGyroId,
                ComponentType.Upgrade,
                ChassisLocations.CenterTorso
            );

            resizer = new AdjustCompDefInvSizeHelper(identity, Control.settings.AutoFixGyroSlotChange);
        }

        public void PreProcess(MechComponentDef target, Dictionary<string, object> values)
        {
            identity.PreProcess(target, values);
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
    }
}