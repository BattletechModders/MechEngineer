using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using Harmony;

namespace MechEngineMod
{
    [HarmonyPatch(typeof(MechValidationRules), "ValidateMechPosessesWeapons")]
    public static class EngineMechValidationRulesPatch
    {
        // invalidate mech loadouts that don't have an engine
        // invalidate mech loadouts that have more jump jets than the engine supports

        public static void Postfix(MechDef mechDef, ref Dictionary<MechValidationType, List<string>> errorMessages)
        {
            try
            {
                if (errorMessages.Count > 0)
                {
                    return;
                }

                // engine
                var engineRefs = mechDef.Inventory.Where(x => Control.IsEnginePart(x.Def)).ToList();
                var mainEngine = engineRefs
                    .Where(x => x.DamageLevel == ComponentDamageLevel.Functional || x.DamageLevel == ComponentDamageLevel.NonFunctional)
                    .Select(x => Engine.MainEngineFromDef(x.Def)).FirstOrDefault();
                if (mainEngine == null)
                {
                    errorMessages[MechValidationType.InvalidInventorySlots].Add("MISSING ENGINE: This Mech must mount a functional Fusion Engine");
                    return;
                }

                if ((mainEngine.Type == Engine.EngineType.XL_shs || mainEngine.Type == Engine.EngineType.XL_dhs ) && engineRefs.Count(x => x.DamageLevel == ComponentDamageLevel.Functional || x.DamageLevel == ComponentDamageLevel.NonFunctional) != 3)
                {
                    errorMessages[MechValidationType.InvalidInventorySlots].Add("INCOMPLETE ENGINE: An XL Engine requires left and right torso components");
                    return;
                }

                // jump jets
                {
                    var currentCount = mechDef.Inventory.Count(c => c.ComponentDefType == ComponentType.JumpJet);
                    var maxCount = Control.calc.CalcJumpJetCount(mainEngine, mechDef.Chassis.Tonnage);
                    if (currentCount > maxCount)
                    {
                        errorMessages[MechValidationType.InvalidJumpjets].Add(String.Format("JUMPJETS: This Mech mounts too many jumpjets ({0} out of {1})", currentCount, maxCount));
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}