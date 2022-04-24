using System;
using System.Collections.Generic;
using BattleTech;
using Harmony;
using Localize;

namespace MechEngineer.Features.ArmorStructureRatio.Patches;

[HarmonyPatch(typeof(MechValidationRules), nameof(MechValidationRules.ValidateMechStructure))]
public static class MechValidationRules_ValidateMechStructure_Patch
{
    public static void Postfix(MechDef mechDef, ref Dictionary<MechValidationType, List<Text>> errorMessages)
    {
        try
        {
            ArmorStructureRatioFeature.ValidateMechArmorStructureRatio(mechDef, errorMessages);
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
    }
}