using System.Collections.Generic;
using BattleTech;
using CustomComponents;

namespace MechEngineer.Features.AutoFix
{
    internal class CockpitHandler : IAdjustUpgradeDef, IPreProcessor
    {
        internal static MELazy<CockpitHandler> Lazy = new MELazy<CockpitHandler>();
        internal static CockpitHandler Shared => Lazy.Value;
        
        private readonly IdentityHelper identity;
        private readonly AdjustCompDefTonnageHelper reweighter;
        private readonly AdjustCompDefInvSizeHelper resizer;

        public CockpitHandler()
        {
            identity = Control.settings.AutoFixCockpitCategorizer;

            if (identity == null)
            {
                return;
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
    }
}