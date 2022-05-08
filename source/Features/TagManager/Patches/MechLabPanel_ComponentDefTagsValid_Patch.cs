using BattleTech;
using BattleTech.UI;
using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace MechEngineer.Features.TagManager.Patches;

[HarmonyPatch(typeof(MechLabPanel), nameof(MechLabPanel.ComponentDefTagsValid))]
public static class MechLabPanel_ComponentDefTagsValid_Patch
{
    [HarmonyPrefix]
    public static bool Prefix(MechLabPanel __instance, MechComponentDef def, ref bool __result, bool ___isDebugLab)
    {
        try
        {
            var tags = def.ComponentTags;

            if (__instance.IsSimGame)
            {
                __result = true;
            }
            else if (!___isDebugLab && tags.Contains(MechValidationRules.ComponentTag_Debug))
            {
                __result = false;
            }
            else if (tags.Contains(MechValidationRules.Tag_Blacklisted))
            {
                __result = false;
            }
            else if (tags.ContainsAny(TagManagerFeature.Shared.Settings.SkirmishWhitelistTagSet))
            {
                __result = true;
            }
            else
            {
                __result = false;
            }
            return false;
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }

        return true;
    }

    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        // fix hardcoded LosTech filter in skirmish mechlab, should be using blacklisted anyway!
        return instructions.StringReplacer(MechValidationRules.Tag_Blacklisted, MechValidationRules.ComponentTag_LosTech);
    }

    private static IEnumerable<CodeInstruction> StringReplacer(this IEnumerable<CodeInstruction> instructions, string newString, params string[] oldStrings)
    {
        foreach (var instruction in instructions)
        {
            if (instruction.opcode == OpCodes.Ldstr && instruction.operand is string text && oldStrings.Contains(text))
            {
                instruction.operand = newString;
            }
            yield return instruction;
        }
    }
}