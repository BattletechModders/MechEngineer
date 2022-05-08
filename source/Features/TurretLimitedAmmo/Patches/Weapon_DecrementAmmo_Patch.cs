using System.Collections.Generic;
using System.Reflection.Emit;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.TurretLimitedAmmo.Patches;

[HarmonyPatch(typeof(Weapon), nameof(Weapon.DecrementAmmo))]
public static class Weapon_DecrementAmmo_Patch
{
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        foreach (var instruction in instructions)
        {
            if (instruction.opcode == OpCodes.Isinst && Equals(typeof(Turret), instruction.operand))
            {
                instruction.operand = typeof(string);
            }

            yield return instruction;
        }
    }
}