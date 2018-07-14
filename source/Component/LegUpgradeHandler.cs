using System;
using System.Collections.Generic;
using BattleTech;
using BattleTech.UI;

namespace MechEngineer
{
    // this isn't yet leg actuators, but we still did reduce the legs size
    internal class LegUpgradeHandler : IDescription, IAdjustUpgradeDef
    {
        internal static LegUpgradeHandler Shared = new LegUpgradeHandler();

        private readonly AdjustCompDefInvSizeHelper resizer;

        private LegUpgradeHandler()
        {
            var identity = new IdentityHelper
            {
                AllowedLocations = ChassisLocations.Legs,
                ComponentType = ComponentType.Upgrade,
                Prefix = Control.settings.AutoFixLegUpgradesPrefix,
            };

            resizer = new AdjustCompDefInvSizeHelper(identity, Control.settings.AutoFixLegUpgradesSlotChange);
        }

        public void AdjustUpgradeDef(UpgradeDef upgradeDef)
        {
            if (!Control.settings.AutoFixLegUpgrades)
            {
                return;
            }

            resizer.AdjustComponentDef(upgradeDef);
        }

        public string CategoryName
        {
            get { return "Leg Upgrade"; }
        }
    }
}