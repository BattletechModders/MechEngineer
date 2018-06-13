using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace MechEngineMod
{
    [HarmonyPatch(typeof(MechLabPanel), "GetCantSaveErrorString")]
    public static class MechLabPanelGetCantSaveErrorStringPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return instructions.MethodReplacer(
                AccessTools.Method(typeof(MechValidationRules), "GetValidationErrors"),
                AccessTools.Method(typeof(MechLabPanelGetCantSaveErrorStringPatch), "GetValidationErrors")
            );
        }

        public static List<string> GetValidationErrors(Dictionary<MechValidationType, List<string>> errorMessages, List<MechValidationType> validationTypes)
        {
            validationTypes.Remove(MechValidationType.InvalidInventorySlots);
            return MechValidationRules.GetValidationErrors(errorMessages, validationTypes);
        }
    }
}