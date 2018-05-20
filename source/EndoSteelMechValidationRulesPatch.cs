using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using Harmony;

namespace MechEngineMod
{
    [HarmonyPatch(typeof(MechValidationRules), "ValidateMechPosessesWeapons")]
    public static class EndoSteelMechValidationRulesPatch
    {
        // invalidate mech loadouts that have more than one endo-steel critical slot but not exactly 14

        public static void Postfix(MechDef mechDef, ref Dictionary<MechValidationType, List<string>> errorMessages)
        {
            try
            {
                if (errorMessages.Count > 0)
                {
                    return;
                }

                // endo-steel
                {
                    var currentCount = mechDef.Inventory.Count(x => Control.IsEndoSteel(x.Def));
                    var exactCount = Control.settings.EndoSteelRequiredCriticals;
                    if (currentCount > 0 && currentCount != exactCount)
                    {
                        errorMessages[MechValidationType.InvalidInventorySlots].Add(String.Format("ENDO-STEEL: Critical slots count does not match ({0} instead of {1})", currentCount, exactCount));
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