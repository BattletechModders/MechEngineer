﻿using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;

namespace MechEngineer
{
    internal class EngineTypeDefHandler : IDescription, IValidateDrop, IValidateMech
    {
        internal static EngineTypeDefHandler Shared = new EngineTypeDefHandler();

        private readonly ValidationHelper checker;

        private EngineTypeDefHandler()
        {
            var identifier = new IdentityFuncHelper(def => def is EngineTypeDef);
            checker = new ValidationHelper(identifier, this);
        }

        public string CategoryName
        {
            get { return "Engine Shielding"; }
        }

        public MechLabDropResult ValidateDrop(MechLabItemSlotElement dragItem, List<MechLabItemSlotElement> localInventory)
        {
            return checker.ValidateDrop(dragItem, localInventory);
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