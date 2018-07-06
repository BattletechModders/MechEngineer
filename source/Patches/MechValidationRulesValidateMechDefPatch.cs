using System;
using System.Collections.Generic;
using BattleTech;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(MechValidationRules), "ValidateMechDef")]
    public static class MechValidationRulesValidateMechDefPatch
    {
        public static void Postfix(MechDef mechDef, ref Dictionary<MechValidationType, List<string>> __result)
        {
            try
            {
                ValidationFacade.ValidateMech(mechDef, ref __result);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}
