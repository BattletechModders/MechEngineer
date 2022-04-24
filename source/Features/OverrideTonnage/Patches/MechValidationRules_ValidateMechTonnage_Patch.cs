using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using BattleTech;
using Harmony;
using UnityEngine;

namespace MechEngineer.Features.OverrideTonnage.Patches;

[HarmonyPatch(typeof(MechValidationRules), nameof(MechValidationRules.ValidateMechTonnage))]
public static class MechValidationRules_ValidateMechTonnage_Patch
{
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        foreach (var instruction in instructions)
        {
            if (instruction.opcode == OpCodes.Ldc_R8 && instruction.operand is double epsilon)
            {
                if (Math.Abs(epsilon - 0.0001) < Mathf.Epsilon)
                {
                    instruction.operand = (double)OverrideTonnageFeature.settings.PrecisionEpsilon;
                }
            }
            else if (instruction.opcode == OpCodes.Ldc_R4 && instruction.operand is float underweightWarningThreshold)
            {
                if (Math.Abs(underweightWarningThreshold - 0.5f) < Mathf.Epsilon)
                {
                    instruction.operand = (float)OverrideTonnageFeature.settings.UnderweightWarningThreshold;
                }
            }

            yield return instruction;
        }
    }
}