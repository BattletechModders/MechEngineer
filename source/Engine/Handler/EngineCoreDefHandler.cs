using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;

namespace MechEngineer
{
    internal class EngineCoreDefHandler : IValidateAdd, IDescription, IValidateMech
    {
        internal static EngineCoreDefHandler Shared = new EngineCoreDefHandler();

        private readonly ValidationHelper checker;

        private EngineCoreDefHandler()
        {
            var identifier = new IdentityFuncHelper(def => def is EngineCoreDef);
            checker = new ValidationHelper(identifier, this);
        }

        public string CategoryName
        {
            get { return "Engine Core"; }
        }

        public void ValidateAdd(MechComponentDef newComponentDef, List<MechLabItemSlotElement> localInventory, ref string dropErrorMessage, ref bool result)
        {
            checker.ValidateAdd(newComponentDef, localInventory, ref dropErrorMessage, ref result);
        }

        public void ValidateMech(MechDef mechDef, Dictionary<MechValidationType, List<string>> errorMessages)
        {
            checker.ValidateMech(mechDef, errorMessages);

            var mainEngine = mechDef.Inventory.GetEngineCoreDef();
            if (mainEngine == null)
            {
                return;
            }

            // jump jets
            {
                var currentCount = mechDef.Inventory.Count(c => c.ComponentDefType == ComponentType.JumpJet);
                var maxCount = Control.calc.CalcJumpJetCount(mainEngine, mechDef.Chassis.Tonnage);
                if (currentCount > maxCount)
                {
                    errorMessages[MechValidationType.InvalidJumpjets].Add(string.Format("JUMPJETS: This Mech mounts too many jumpjets ({0} out of {1})", currentCount, maxCount));
                }
            }
        }
    }
}