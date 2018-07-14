using System;
using System.Collections.Generic;
using BattleTech;
using BattleTech.UI;
using CustomComponents;

namespace MechEngineer
{
    // this isn't yet leg actuators, but we still did reduce the legs size
    internal class LegUpgradeHandler : IAdjustUpgradeDef, IPreProcessor
    {
        internal static LegUpgradeHandler Shared = new LegUpgradeHandler();
        
        private readonly IdentityHelper identity;
        private readonly AdjustCompDefInvSizeHelper resizer;

        private LegUpgradeHandler()
        {
            identity = Control.settings.AutoFixLegUpgradesCategorizer;

            resizer = new AdjustCompDefInvSizeHelper(identity, Control.settings.AutoFixLegUpgradesSlotChange);
        }

        public void PreProcess(MechComponentDef target, Dictionary<string, object> values)
        {
            identity.PreProcess(target, values);
        }

        public void AdjustUpgradeDef(UpgradeDef upgradeDef)
        {
            resizer.AdjustComponentDef(upgradeDef);
        }
    }
}