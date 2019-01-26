using System.Collections.Generic;
using BattleTech;
using CustomComponents;

namespace MechEngineer
{
    // this isn't yet leg actuators, but we still did reduce the legs size
    internal class LegUpgradeHandler : IAdjustUpgradeDef, IPreProcessor
    {
        internal static MELazy<LegUpgradeHandler> Lazy = new MELazy<LegUpgradeHandler>();
        internal static LegUpgradeHandler Shared => Lazy.Value;
        
        private readonly IdentityHelper identity;
        private readonly AdjustCompDefInvSizeHelper resizer;

        public LegUpgradeHandler()
        {
            identity = Control.settings.AutoFixLegUpgradesCategorizer;

            if (identity == null)
            {
                return;
            }

            if (Control.settings.AutoFixLegUpgradesSlotChange != null)
            {
                resizer = new AdjustCompDefInvSizeHelper(identity, Control.settings.AutoFixLegUpgradesSlotChange);
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