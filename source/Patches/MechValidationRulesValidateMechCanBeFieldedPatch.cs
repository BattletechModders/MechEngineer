using System;
using System.Collections.Generic;
using BattleTech;
using Harmony;

namespace MechEngineMod
{
    [HarmonyPatch(typeof(MechValidationRules), "ValidateMechCanBeFielded")]
    public static class MechValidationRulesValidateMechCanBeFieldedPatch
    {
        public static void Postfix(MechDef mechDef, ref bool __result)
        {
            try
            {
                if (!__result)
                {
                    return;
                }

                var errorMessages = (Dictionary<MechValidationType, List<string>>)
                    Traverse.Create<MechValidationRules>()
                        .Method("InitializeValidationResults")
                        .GetValue();
                MechValidationRulesMods.Validate(mechDef, ref errorMessages);

                __result = errorMessages.Count == 0;
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}