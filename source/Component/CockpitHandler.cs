using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using UnityEngine;

namespace MechEngineer
{
    internal class CockpitHandler : IDescription, IAdjustUpgradeDef, IAutoFixMechDef
    {
        internal static CockpitHandler Shared = new CockpitHandler();
        
        private readonly IdentityHelper identity;
        private readonly AutoFixMechDefHelper fixer;
        private readonly AdjustCompDefTonnageHelper resizer;

        private CockpitHandler()
        {
            identity = new IdentityHelper
            {
                AllowedLocations = ChassisLocations.Head,
                ComponentType = ComponentType.Upgrade,
                Prefix = Control.settings.AutoFixCockpitPrefix,
            };


            fixer = new AutoFixMechDefHelper(
                identity,
                Control.settings.AutoFixMechDefCockpitId,
                ComponentType.Upgrade,
                ChassisLocations.Head
            );

            resizer = new AdjustCompDefTonnageHelper(identity, Control.settings.AutoFixCockpitTonnageChange);
        }

        public bool ProtectsAgainstShutdownInjury(MechDef mechDef)
        {
            return mechDef.Inventory.Any(c => c.DamageLevel == ComponentDamageLevel.Functional && identity.IsCustomType(c.Def));
        }

        public void AdjustUpgradeDef(UpgradeDef upgradeDef)
        {
            if (!Control.settings.AutoFixCockpitUpgrades)
            {
                return;
            }

            resizer.AdjustComponentDef(upgradeDef);
        }

        public void AutoFixMechDef(MechDef mechDef, float originalTotalTonnage)
        {
            fixer.AutoFixMechDef(mechDef, originalTotalTonnage);
        }

        public string CategoryName
        {
            get { return "Cockpit"; }
        }
    }
}