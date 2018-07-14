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
            identity = Control.settings.AutoFixGyroCategorizer;

            fixer = new AutoFixMechDefHelper(
                identity,
                Control.settings.AutoFixMechDefGyroAdder
            );

            resizer = new AdjustCompDefInvSizeHelper(identity, Control.settings.AutoFixGyroSlotChange);
        }

        public void PreProcess(MechComponentDef target, Dictionary<string, object> values)
        {
            identity.PreProcess(target, values);
        }

        public void AdjustUpgradeDef(UpgradeDef upgradeDef)
        {
            resizer.AdjustComponentDef(upgradeDef);
        }

        public void AutoFixMechDef(MechDef mechDef, float originalTotalTonnage)
        {
            fixer.AutoFixMechDef(mechDef, originalTotalTonnage);
        }
    }
}