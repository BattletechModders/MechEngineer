using System;
using System.Collections.Generic;
using BattleTech;
using Harmony;

namespace MechEngineMod
{
    [HarmonyPatch(typeof(MechValidationRules), "ValidateMechPosessesWeapons")]
    public static class MechValidationRulesValidateMechPosessesWeaponsPatch
    {
        // invalidate mech loadouts that have more than 0 endo-steel critical slots but not exactly 14
        // invalidate mech loadouts that have more than 0 ff critical slots but not exactly 14
        public static void Postfix(MechDef mechDef, ref Dictionary<MechValidationType, List<string>> errorMessages)
        {
            try
            {
                EndoSteel.ValidationRulesCheck(mechDef, ref errorMessages);
                FerrosFibrous.ValidationRulesCheck(mechDef, ref errorMessages);
                EngineHeat.ValidationRulesCheck(mechDef, ref errorMessages);
                Engine.ValidationRulesCheck(mechDef, ref errorMessages);
                Gyro.ValidationRulesCheck(mechDef, ref errorMessages);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}