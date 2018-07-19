using System.Collections.Generic;
using BattleTech;
using CustomComponents;

namespace MechEngineer
{
    internal class ArmorHandler : IAutoFixMechDef
    {
        internal static ArmorHandler Shared = new ArmorHandler();

        private readonly IdentityHelper identity;
        private readonly AutoFixMechDefHelper fixer;

        private ArmorHandler()
        {
            identity = Control.settings.AutoFixArmorCategorizer;

            fixer = new AutoFixMechDefHelper(
                identity,
                Control.settings.AutoFixMechDefArmorAdder
            );
        }

        public void AutoFixMechDef(MechDef mechDef, float originalTotalTonnage)
        {
            fixer.AutoFixMechDef(mechDef, originalTotalTonnage);
        }
    }
}