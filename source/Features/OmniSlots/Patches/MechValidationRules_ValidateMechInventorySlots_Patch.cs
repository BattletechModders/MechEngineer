using System.Collections.Generic;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.OmniSlots.Patches
{
    [HarmonyPatch(typeof(MechValidationRules), nameof(MechValidationRules.ValidateMechInventorySlots))]
    internal static class MechValidationRules_ValidateMechInventorySlots_Patch
    {
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return OmniSlotsFeature.DisableHardpointValidatorsTranspiler(instructions);
        }
    }
}