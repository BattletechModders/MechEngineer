using System.Collections.Generic;
using BattleTech;
using BattleTech.UI;

namespace MechEngineer
{
    internal class EngineSideDefHandler : IValidateDrop, IDescription
    {
        internal static EngineSideDefHandler Shared = new EngineSideDefHandler();

        private readonly ValidationHelper checker;

        private EngineSideDefHandler()
        {
            var identifier = new IdentityFuncHelper(def => def is EngineSideDef);
            checker = new ValidationHelper(identifier, this) {Required = false};
        }

        public string CategoryName
        {
            get { return "Engine Shielding"; }
        }

        public MechLabDropResult ValidateDrop(MechLabItemSlotElement dragItem, List<MechLabItemSlotElement> localInventory)
        {
            return checker.ValidateDrop(dragItem, localInventory);
        }
    }
}