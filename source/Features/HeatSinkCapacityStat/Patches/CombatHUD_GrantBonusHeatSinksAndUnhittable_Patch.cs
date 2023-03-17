using System.Collections.Generic;
using System.Reflection.Emit;
using BattleTech.UI;

namespace MechEngineer.Features.HeatSinkCapacityStat.Patches;

[HarmonyPatch(typeof(CombatHUD), nameof(CombatHUD.GrantBonusHeatSinksAndUnhittable))]
public static class CombatHUD_GrantBonusHeatSinksAndUnhittable_Patch
{
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        foreach (var codeInstruction in instructions)
        {
            if (codeInstruction.opcode == OpCodes.Isinst)
            {
                // force isInst to always false
                codeInstruction.operand = typeof(CombatHUD_GrantBonusHeatSinksAndUnhittable_Patch);
            }
            yield return codeInstruction;
        }
    }
}