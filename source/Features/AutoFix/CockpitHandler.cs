using System.Collections.Generic;
using BattleTech;
using CustomComponents;

namespace MechEngineer
{
    internal class CockpitHandler : IAdjustUpgradeDef, IAutoFixMechDef, IPreProcessor
    {
        internal static MELazy<CockpitHandler> Lazy = new MELazy<CockpitHandler>();
        internal static CockpitHandler Shared => Lazy.Value;
        
        private readonly IdentityHelper identity;
        private readonly AutoFixMechDefHelper fixer;
        private readonly AdjustCompDefTonnageHelper reweighter;
        private readonly AdjustCompDefInvSizeHelper resizer;

        public CockpitHandler()
        {
            identity = Control.settings.AutoFixCockpitCategorizer;

            if (identity == null)
            {
                return;
            }

            if (Control.settings.AutoFixMechDefCockpitAdder != null)
            {
                fixer = new AutoFixMechDefHelper(
                    identity,
                    Control.settings.AutoFixMechDefCockpitAdder
                );
            }

            if (Control.settings.AutoFixCockpitTonnageChange != null)
            {
                reweighter = new AdjustCompDefTonnageHelper(identity, Control.settings.AutoFixCockpitTonnageChange);
            }

            if (Control.settings.AutoFixCockpitSlotChange != null)
            {
                resizer = new AdjustCompDefInvSizeHelper(identity, Control.settings.AutoFixCockpitSlotChange);
            }
        }

        public void PreProcess(object target, Dictionary<string, object> values)
        {
            identity?.PreProcess(target, values);
        }

        public void AdjustUpgradeDef(UpgradeDef upgradeDef)
        {
            reweighter?.AdjustComponentDef(upgradeDef);
            resizer?.AdjustComponentDef(upgradeDef);
        }

        public void AutoFixMechDef(MechDef mechDef, float originalTotalTonnage)
        {
            fixer?.AutoFixMechDef(mechDef, originalTotalTonnage);
        }
    }
}