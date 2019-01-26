using BattleTech;

namespace MechEngineer
{
    internal class StructureHandler : IAutoFixMechDef
    {
        internal static MELazy<StructureHandler> Lazy = new MELazy<StructureHandler>();
        internal static StructureHandler Shared => Lazy.Value;

        private readonly IdentityHelper identity;
        private readonly AutoFixMechDefHelper fixer;

        public StructureHandler()
        {
            identity = Control.settings.AutoFixStructureCategorizer;

            if (identity == null)
            {
                return;
            }

            if (Control.settings.AutoFixMechDefStructureAdder != null)
            {
                fixer = new AutoFixMechDefHelper(
                    identity,
                    Control.settings.AutoFixMechDefStructureAdder
                );
            }
        }

        public void AutoFixMechDef(MechDef mechDef, float originalTotalTonnage)
        {
            fixer?.AutoFixMechDef(mechDef, originalTotalTonnage);
        }
    }
}