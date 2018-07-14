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
            identity = new IdentityHelper
            {
                AllowedLocations = ChassisLocations.Legs,
                ComponentType = ComponentType.Upgrade,
                Prefix = Control.settings.AutoFixLegUpgradesPrefix,
                CategoryId = Control.settings.AutoFixLegUpgradesCategoryId,
            };

            resizer = new AdjustCompDefInvSizeHelper(identity, Control.settings.AutoFixLegUpgradesSlotChange);
        }

        public void PreProcess(MechComponentDef target, Dictionary<string, object> values)
        {
            identity.PreProcess(target, values);
        }

        public void AdjustUpgradeDef(UpgradeDef upgradeDef)
        {
            if (!Control.settings.AutoFixLegUpgrades)
            {
                return;
            }

            resizer.AdjustComponentDef(upgradeDef);
        }
    }
}