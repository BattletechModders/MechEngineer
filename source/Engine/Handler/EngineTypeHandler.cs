using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using CustomComponents;

namespace MechEngineer
{
    internal class EngineTypeHandler : IDescription, IValidateDrop, IValidateMech
    {
        internal static EngineTypeHandler Shared = new EngineTypeHandler();

        private readonly ValidationHelper checker;

        private EngineTypeHandler()
        {
            var identifier = new IdentityFuncHelper(def => def.GetComponent<EngineType>() != null);
            checker = new ValidationHelper(identifier, this);
        }

        public string CategoryName
        {
            get { return "Engine Shielding"; }
        }

        public MechLabDropResult ValidateDrop(MechLabItemSlotElement dragItem, MechLabLocationWidget widget)
        {
            return checker.ValidateDrop(dragItem, widget);
        }

        public void ValidateMech(MechDef mechDef, Dictionary<MechValidationType, List<string>> errorMessages)
        {
            checker.ValidateMech(mechDef, errorMessages);

            var result = EngineSearcher.SearchInventory(mechDef.Inventory);
            var typeDef = result.Type;
            if (typeDef != null)
            {
                var requirements = typeDef.Requirements;

                if (result.Parts
                        .Where(x => x.DamageLevel == ComponentDamageLevel.Functional)
                        .Select(c => c.ComponentDefID).Intersect(requirements).Count() != requirements.Length)
                {
                    var engineName = typeDef.Def.Description.UIName.ToUpper();
                    errorMessages[MechValidationType.InvalidInventorySlots].Add(engineName + ": Requires left and right torso slots");
                }
            }

            // jump jets
            if (result.CoreRef != null)
            {
                var coreDef = result.CoreRef.CoreDef;
                var currentCount = mechDef.Inventory.Count(c => c.ComponentDefType == ComponentType.JumpJet);
                var maxCount = coreDef.GetMovement(mechDef.Chassis.Tonnage).JumpJetCount;
                if (currentCount > maxCount)
                {
                    errorMessages[MechValidationType.InvalidJumpjets].Add($"JUMPJETS: This Mech mounts too many jumpjets ({currentCount} out of {maxCount})");
                }
            }
        }
    }
}