using System;
using BattleTech;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(MechValidationRules), nameof(MechValidationRules.ValidateMechStructureSimple))]
    public static class MechValidationRules_ValidateMechStructureSimple_Patch
    {
        public static void Postfix(MechDef mechDef)
        {
            try
            {
                ArmorStructureRatioValidation.ValidateMechArmorStructureRatio(mechDef);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}