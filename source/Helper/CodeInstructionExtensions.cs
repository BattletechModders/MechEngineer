using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using Harmony;

namespace MechEngineer.Helper;

internal static class CodeInstructionExtensions
{
    internal static IEnumerable<CodeInstruction> StringReplacer(this IEnumerable<CodeInstruction> instructions, string newString, params string[] oldStrings)
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