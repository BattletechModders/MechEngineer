using System.Collections.Generic;
using System.Reflection.Emit;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.ComponentExplosions.Patches
{
    [HarmonyPatch(typeof(AmmunitionBox), nameof(AmmunitionBox.DamageComponent))]
    public static class AmmunitionBox_DamageComponent_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var count = 0;
            foreach (var instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldarg_3)
                {
                    count++;
                }

                if (count == 2)
                {
                    instruction.operand = null;
                    instruction.opcode = OpCodes.Ret;
                    yield return instruction;
                    yield break;
                }
                yield return instruction;
            }
        }
    }
}