using System.Collections.Generic;
using BattleTech;
using CustomComponents;

namespace MechEngineer
{
    internal class GyroHandler : IAdjustUpgradeDef, IAutoFixMechDef, IPreProcessor
    {
        internal static MELazy<GyroHandler> Lazy = new MELazy<GyroHandler>();
        internal static GyroHandler Shared => Lazy.Value;

        private readonly IdentityHelper identity;
        private readonly AutoFixMechDefHelper fixer;
        private readonly AdjustCompDefInvSizeHelper resizer;

        public GyroHandler()
        {
            identity = Control.settings.AutoFixGyroCategorizer;

            if (identity == null)
            {
                return;
            }

            if (Control.settings.AutoFixMechDefGyroAdder != null)
            {
                fixer = new AutoFixMechDefHelper(
                    identity,
                    Control.settings.AutoFixMechDefGyroAdder
                );
            }

            if (Control.settings.AutoFixGyroSlotChange != null)
            {
                resizer = new AdjustCompDefInvSizeHelper(identity, Control.settings.AutoFixGyroSlotChange);
            }
        }

        public void PreProcess(object target, Dictionary<string, object> values)
        {
            identity?.PreProcess(target, values);
        }

        public void AdjustUpgradeDef(UpgradeDef upgradeDef)
        {
            resizer?.AdjustComponentDef(upgradeDef);
        }

        public void AutoFixMechDef(MechDef mechDef, float originalTotalTonnage)
        {
            fixer?.AutoFixMechDef(mechDef, originalTotalTonnage);
        }
    }
}