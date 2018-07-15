using System.Collections.Generic;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(MechLabPanel), "GetNonFieldableErrorString")]
    public static class MechLabPanel_GetNonFieldableErrorString_Patch
    {
        private static bool _isSimGame;

        public static void Prefix(MechLabPanel __instance)
        {
            _isSimGame = __instance.IsSimGame;
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return instructions.MethodReplacer(
                AccessTools.Method(typeof(MechValidationRules), "GetValidationErrors"),
                AccessTools.Method(typeof(MechLabPanel_GetNonFieldableErrorString_Patch), "GetValidationErrors")
            );
        }

        public static List<string> GetValidationErrors(Dictionary<MechValidationType, List<string>> errorMessages, List<MechValidationType> validationTypes)
        {
            if (_isSimGame)
            {
                validationTypes.Add(MechValidationType.InvalidInventorySlots);
            }

            return MechValidationRules.GetValidationErrors(errorMessages, validationTypes);
        }
    }
}