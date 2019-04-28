using System.Collections.Generic;
using BattleTech;
using CustomComponents;

namespace MechEngineer.Features.AutoFix
{
    internal class GyroHandler : IAdjustUpgradeDef, IPreProcessor
    {
        internal static MELazy<GyroHandler> Lazy = new MELazy<GyroHandler>();
        internal static GyroHandler Shared => Lazy.Value;

        private readonly IdentityHelper identity;
        private readonly AdjustCompDefInvSizeHelper resizer;

        public GyroHandler()
        {
            identity = Control.settings.AutoFixGyroCategorizer;

            if (identity == null)
            {
                return;
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
    }
}