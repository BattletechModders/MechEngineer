using System.Collections.Generic;
using BattleTech;
using BattleTech.UI;
using CustomComponents;

namespace MechEngineer
{
    internal class EngineSideHandler : IValidateDrop, IDescription
    {
        internal static EngineSideHandler Shared = new EngineSideHandler();

        private readonly ValidationHelper checker;

        private EngineSideHandler()
        {
            var identifier = new IdentityFuncHelper(def => def.GetComponent<EngineSide>() != null);
            checker = new ValidationHelper(identifier, this) {Required = false};
        }

        public string CategoryName
        {
            get { return "Engine Shielding"; }
        }

        public MechLabDropResult ValidateDrop(MechLabItemSlotElement dragItem, MechLabLocationWidget widget)
        {
            return checker.ValidateDrop(dragItem, widget);
        }
    }
}