using BattleTech;

namespace MechEngineer.Features.ArmorStructureRatio.Patches;

[HarmonyPatch(typeof(MechValidationRules), nameof(MechValidationRules.ValidateMechStructureSimple))]
public static class MechValidationRules_ValidateMechStructureSimple_Patch
{
    [HarmonyPostfix]
    [HarmonyWrapSafe]
    public static void Postfix(MechDef mechDef)
    {
        ArmorStructureRatioFeature.ValidateMechArmorStructureRatio(mechDef);
    }
}
