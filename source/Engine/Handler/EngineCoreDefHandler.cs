using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using CustomComponents;

namespace MechEngineer
{
    internal class EngineCoreDefHandler : IDescription, IValidateMech
    {
        internal static EngineCoreDefHandler Shared = new EngineCoreDefHandler();


        private EngineCoreDefHandler()
        {
            var identifier = new IdentityFuncHelper(def => def.GetComponent<EngineCoreDef>() != null);
        }

        public string CategoryName
        {
            get { return "Engine Core"; }
        }


        public void ValidateMech(MechDef mechDef, Dictionary<MechValidationType, List<string>> errorMessages)
        {

            var mainEngine = mechDef.Inventory.GetEngineCoreDef();
            if (mainEngine == null)
            {
                return;
            }

            // jump jets
            {
                var currentCount = mechDef.Inventory.Count(c => c.ComponentDefType == ComponentType.JumpJet);
                var maxCount = mainEngine.GetMovement(mechDef.Chassis.Tonnage).JumpJetCount;
                if (currentCount > maxCount)
                {
                    errorMessages[MechValidationType.InvalidJumpjets].Add($"JUMPJETS: This Mech mounts too many jumpjets ({currentCount} out of {maxCount})");
                }
            }
        }
    }
}