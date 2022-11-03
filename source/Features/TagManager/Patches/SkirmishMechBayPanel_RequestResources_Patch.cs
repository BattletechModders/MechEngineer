using BattleTech;
using BattleTech.UI;
using Harmony;
using System.Collections.Generic;
using MechEngineer.Helper;

namespace MechEngineer.Features.TagManager.Patches;

[HarmonyPatch(typeof(SkirmishMechBayPanel), nameof(SkirmishMechBayPanel.RequestResources))]
public static class SkirmishMechBayPanel_RequestResources_Patch
{
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return instructions.StringReplacer("I do not care as this shouldn't exist", MechValidationRules.MechTag_Unlocked);
    }
}