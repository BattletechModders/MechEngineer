using BattleTech;

namespace MechEngineer
{
    internal class ArmorHandler : IAutoFixMechDef
    {
        internal static MELazy<ArmorHandler> Lazy = new MELazy<ArmorHandler>();
        internal static ArmorHandler Shared => Lazy.Value;

        private readonly IdentityHelper identity;
        private readonly AutoFixMechDefHelper fixer;

        public ArmorHandler()
        {
            identity = Control.settings.AutoFixArmorCategorizer;

            if (identity == null)
            {
                return;
            }

            if (Control.settings.AutoFixMechDefArmorAdder != null)
            {
                fixer = new AutoFixMechDefHelper(
                    identity,
                    Control.settings.AutoFixMechDefArmorAdder
                );
            }
        }

        public void AutoFixMechDef(MechDef mechDef, float originalTotalTonnage)
        {
            fixer?.AutoFixMechDef(mechDef, originalTotalTonnage);
        }
    }
}