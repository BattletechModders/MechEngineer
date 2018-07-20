using BattleTech;

namespace MechEngineer
{
    internal class StructureHandler : IAutoFixMechDef
    {
        internal static StructureHandler Shared = new StructureHandler();

        private readonly IdentityHelper identity;
        private readonly AutoFixMechDefHelper fixer;

        private StructureHandler()
        {
            identity = Control.settings.AutoFixStructureCategorizer;

            fixer = new AutoFixMechDefHelper(
                identity,
                Control.settings.AutoFixMechDefStructureAdder
            );
        }

        public void AutoFixMechDef(MechDef mechDef, float originalTotalTonnage)
        {
            fixer.AutoFixMechDef(mechDef, originalTotalTonnage);
        }
    }
}