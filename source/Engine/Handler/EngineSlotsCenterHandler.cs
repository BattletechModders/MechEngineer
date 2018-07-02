using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;

namespace MechEngineer
{
    internal class EngineSlotsCenterHandler : IDescription, IValidateAdd, IValidationRulesCheck
    {
        internal static EngineSlotsCenterHandler Shared = new EngineSlotsCenterHandler();

        private readonly ValidationHelper checker;

        private EngineSlotsCenterHandler()
        {
            var identifier = new IdentityFuncHelper(def => def is EngineTypeDef);
            checker = new ValidationHelper(identifier, this);
        }

        public string CategoryName
        {
            get { return "Engine Shielding"; }
        }

        public void ValidateAdd(MechComponentDef newComponentDef, List<MechLabItemSlotElement> localInventory, ref string dropErrorMessage, ref bool result)
        {
            checker.ValidateAdd(newComponentDef, localInventory, ref dropErrorMessage, ref result);
        }

        public void ValidateMech(MechDef mechDef, Dictionary<MechValidationType, List<string>> errorMessages)
        {
            checker.ValidateMech(mechDef, errorMessages);

            var result = EngineSearcher.SearchInventory(mechDef.Inventory);
            var typeDef = result.TypeDef;
            if (typeDef != null)
            {
                var requirements = typeDef.Requirements;

                if (result.Parts
                        .Where(x => x.DamageLevel == ComponentDamageLevel.Functional)
                        .Select(c => c.ComponentDefID).Intersect(requirements).Count() != requirements.Length)
                {
                    var engineName = typeDef.Description.UIName.ToUpper();
                    errorMessages[MechValidationType.InvalidInventorySlots].Add(engineName + ": Requires left and right torso slots");
                }
            }

            // jump jets
            if (result.CoreRef != null)
            {
                var coreDef = result.CoreRef.CoreDef;
                var currentCount = mechDef.Inventory.Count(c => c.ComponentDefType == ComponentType.JumpJet);
                var maxCount = Control.calc.CalcJumpJetCount(coreDef, mechDef.Chassis.Tonnage);
                if (currentCount > maxCount)
                {
                    errorMessages[MechValidationType.InvalidJumpjets].Add(string.Format("JUMPJETS: This Mech mounts too many jumpjets ({0} out of {1})", currentCount, maxCount));
                }
            }
        }
    }
}