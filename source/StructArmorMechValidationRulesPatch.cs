using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using Harmony;

namespace MechEngineMod
{
    [HarmonyPatch(typeof(MechValidationRules), "ValidateMechPosessesWeapons")]
    public static class StructArmorMechValidationRulesPatch
    {
        // invalidate mech loadouts that have more than 0 endo-steel critical slots but not exactly 14
        // invalidate mech loadouts that have more than 0 ff critical slots but not exactly 14
        public static void Postfix(MechDef mechDef, ref Dictionary<MechValidationType, List<string>> errorMessages)
        {
            try
            {
                {

                    var currentCount = mechDef.Inventory.Count(x => x.Def.IsEndoSteel());
                    var exactCount = Control.settings.EndoSteelRequiredCriticals;
                    if (currentCount > 0 && currentCount != exactCount)
                    {
                        errorMessages[MechValidationType.InvalidInventorySlots].Add(String.Format("ENDO-STEEL: Critical slots count does not match ({0} instead of {1})", currentCount, exactCount));
                    }
                }

                {

                    var currentCount = mechDef.Inventory.Count(x => x.Def.IsFerrosFibrous());
                    var exactCount = Control.settings.FerrosFibrousRequiredCriticals;
                    if (currentCount > 0 && currentCount != exactCount)
                    {
                        errorMessages[MechValidationType.InvalidInventorySlots].Add(String.Format("FERROS-FIBROUS: Critical slots count does not match ({0} instead of {1})", currentCount, exactCount));
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