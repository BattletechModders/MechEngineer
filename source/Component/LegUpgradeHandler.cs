using System;
using System.Collections.Generic;
using BattleTech;
using BattleTech.UI;

namespace MechEngineer
{
    // this isn't yet leg actuators, but we still did reduce the legs size
    internal class LegUpgradeHandler : IDescription, IValidateDrop, IAdjustUpgradeDef
    {
        internal static LegUpgradeHandler Shared = new LegUpgradeHandler();

        private readonly ValidationHelper checker;
        private readonly AdjustCompDefInvSizeHelper resizer;

        private LegUpgradeHandler()
        {
            var identity = new IdentityHelper
            {
                AllowedLocations = ChassisLocations.Legs,
                ComponentType = ComponentType.Upgrade
            };

            checker = new ValidationHelper(identity, this)
            {
                Required = false
            };

            resizer = new AdjustCompDefInvSizeHelper(identity, size => size > 2 ? Math.Max(1, size - 2) : -1);
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

        public MechLabDropResult ValidateDrop(MechLabItemSlotElement dragItem, List<MechLabItemSlotElement> localInventory)
        {
            return checker.ValidateDrop(dragItem, localInventory);
        }
    }
}