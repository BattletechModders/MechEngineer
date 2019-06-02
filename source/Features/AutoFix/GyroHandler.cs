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
            identity = AutoFixerFeature.settings.GyroCategorizer;

            if (identity == null)
            {
                return;
            }

            if (AutoFixerFeature.settings.GyroSlotChange != null)
            {
                resizer = new AdjustCompDefInvSizeHelper(identity, AutoFixerFeature.settings.GyroSlotChange);
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