using System;
using System.Collections.Generic;
using BattleTech;
using BattleTech.UI;
using Harmony;
using Localize;

namespace MechEngineer.Features.InvalidInventory.Patches;

[HarmonyPatch(typeof(MechLabPanel), nameof(MechLabPanel.GetCantSaveErrorString))]
public static class MechLabPanel_GetCantSaveErrorString_Patch
{
    private static bool _isSimGame;

    public static void Prefix(MechLabPanel __instance)
    {
        _isSimGame = __instance.IsSimGame;
    }

    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return instructions.MethodReplacer(
            AccessTools.Method(typeof(MechValidationRules), "GetValidationErrors"),
            AccessTools.Method(typeof(MechLabPanel_GetCantSaveErrorString_Patch), "GetValidationErrors")
        );
    }

    public static List<Text> GetValidationErrors(Dictionary<MechValidationType, List<Text>> errorMessages, List<MechValidationType> validationTypes)
    {
        try
        {
            if (_isSimGame)
            {
                validationTypes.Remove(MechValidationType.InvalidInventorySlots);
            }
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }

        return MechValidationRules.GetValidationErrors(errorMessages, validationTypes);
    }
}