using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace MechEngineMod
{
    [HarmonyPatch(typeof(MechLabPanel), "GetNonFieldableErrorString")]
    public static class MechLabPanelGetNonFieldableErrorStringPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return instructions.MethodReplacer(
                AccessTools.Method(typeof(MechValidationRules), "GetValidationErrors"),
                AccessTools.Method(typeof(MechLabPanelGetNonFieldableErrorStringPatch), "GetValidationErrors")
            );
        }

        public static List<string> GetValidationErrors(Dictionary<MechValidationType, List<string>> errorMessages, List<MechValidationType> validationTypes)
        {
            validationTypes.Add(MechValidationType.InvalidInventorySlots);
            return MechValidationRules.GetValidationErrors(errorMessages, validationTypes);
        }
    }
}