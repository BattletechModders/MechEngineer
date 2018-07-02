using System.Collections.Generic;
using BattleTech;
using BattleTech.UI;

namespace MechEngineer
{
    internal class EngineSlotsHandler : IValidateAdd, IDescription
    {
        internal static EngineSlotsHandler Shared = new EngineSlotsHandler();

        private readonly ValidationHelper checker;

        private EngineSlotsHandler()
        {
            var identifier = new IdentityFuncHelper(def => def is EngineSideDef);
            checker = new ValidationHelper(identifier, this) {Required = false};
        }

        public string CategoryName
        {
            get { return "Engine Shielding"; }
        }

        public void ValidateAdd(MechComponentDef newComponentDef, List<MechLabItemSlotElement> localInventory, ref string dropErrorMessage, ref bool result)
        {
            checker.ValidateAdd(newComponentDef, localInventory, ref dropErrorMessage, ref result);
        }
    }
}