using System;
using System.Collections.Generic;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace MechEngineer.Features.OmniSlots.Patches
{
    [HarmonyPatch(typeof(MechLabLocationWidget), nameof(MechLabLocationWidget.ValidateAdd), typeof(MechComponentDef))]
    internal static class MechLabLocationWidget_ValidateAdd_Patch
    {
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return OmniSlotsFeature.DisableHardpointValidatorsTranspiler(instructions);
        }
    }
}