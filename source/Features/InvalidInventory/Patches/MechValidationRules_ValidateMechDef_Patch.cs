using System.Collections.Generic;
using BattleTech;
using BattleTech.Data;
using Harmony;
using Localize;

namespace MechEngineer.Features.InvalidInventory.Patches;

[HarmonyPatch(typeof(MechValidationRules), nameof(MechValidationRules.ValidateMechDef))]
internal static class MechValidationRules_ValidateMechDef_Patch
{
    [HarmonyPostfix]
    internal static void Postfix(MechValidationLevel validationLevel, DataManager dataManager, MechDef mechDef, ref Dictionary<MechValidationType, List<Text>> __result)
    {
        if (validationLevel == MechValidationLevel.MechLab)
        {
            MechValidationRules.ValidateMechInventorySlots(dataManager, mechDef, ref __result);
        }
    }
}