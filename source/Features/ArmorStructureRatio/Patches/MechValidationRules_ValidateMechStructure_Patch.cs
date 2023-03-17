using System.Collections.Generic;
using BattleTech;
using Localize;

namespace MechEngineer.Features.ArmorStructureRatio.Patches;

[HarmonyPatch(typeof(MechValidationRules), nameof(MechValidationRules.ValidateMechStructure))]
public static class MechValidationRules_ValidateMechStructure_Patch
{
    [HarmonyPostfix]
    [HarmonyWrapSafe]
    public static void Postfix(MechDef mechDef, ref Dictionary<MechValidationType, List<Text>> errorMessages)
    {
        ArmorStructureRatioFeature.ValidateMechArmorStructureRatio(mechDef, errorMessages);
    }
}
