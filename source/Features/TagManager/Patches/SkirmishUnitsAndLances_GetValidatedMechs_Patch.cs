#nullable disable
using System.Collections.Generic;
using BattleTech;
using BattleTech.Save;
using Harmony;
using Localize;

namespace MechEngineer.Features.TagManager.Patches;

// ignore archive / delete mech message
[HarmonyPatch(typeof(SkirmishUnitsAndLances), nameof(SkirmishUnitsAndLances.GetValidatedMechs))]
public static class SkirmishUnitsAndLances_GetValidatedMechs_Patch
{
    [HarmonyPostfix]
    public static void Postfix(ref Dictionary<string, Text> invalidMechErrors)
    {
        invalidMechErrors = null;
    }

    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return instructions
            .MethodReplacer(
                AccessTools.Method(typeof(SkirmishUnitsAndLances), nameof(RemoveMechDef), new[]{typeof(MechDef)}),
                AccessTools.Method(typeof(SkirmishUnitsAndLances_GetValidatedMechs_Patch), nameof(RemoveMechDef))
            )
            .MethodReplacer(
                AccessTools.Method(typeof(SkirmishUnitsAndLances), nameof(SendToArchive), new[]{typeof(MechDef)}),
                AccessTools.Method(typeof(SkirmishUnitsAndLances_GetValidatedMechs_Patch), nameof(SendToArchive))
            );
    }

    public static bool RemoveMechDef(this SkirmishUnitsAndLances @this, MechDef mechDef)
    {
        // don't do anything
        return false;
    }

    public static void SendToArchive(this SkirmishUnitsAndLances @this, MechDef mech)
    {
        // don't do anything
    }
}