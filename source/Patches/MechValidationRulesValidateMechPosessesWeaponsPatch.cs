using System;
using System.Collections.Generic;
using BattleTech;
using Harmony;

namespace MechEngineMod
{
    [HarmonyPatch(typeof(MechValidationRules), "ValidateMechPosessesWeapons")]
    public static class MechValidationRulesValidateMechPosessesWeaponsPatch
    {
        public static void Postfix(MechDef mechDef, ref Dictionary<MechValidationType, List<string>> errorMessages)
        {
            try
            {
                MechValidationRulesMods.Validate(mechDef, ref errorMessages);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}