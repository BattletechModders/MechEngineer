using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using Harmony;
using MechEngineer.Features.HardpointFix.utils;

namespace MechEngineer.Features.HardpointFix.sorting.Patches
{
    [HarmonyPatch(typeof(MechHardpointRules), "GetComponentPrefabName")]
    public static class MechHardpointRules_GetComponentPrefabName_Patch
    {
        private static WeaponComponentPrefabCalculator calculator;

        internal static void SetupCalculator(ChassisDef chassisDef, List<MechComponentRef> componentRefs)
        {
            if (chassisDef?.HardpointDataDef?.HardpointData == null)
            {
                return;
            }

            if (componentRefs == null || componentRefs.Count == 0)
            {
                return;
            }

            componentRefs = componentRefs
                .Where(c => c != null)
                .Where(c => c.ComponentDefType == ComponentType.Weapon)
                .Where(c => c.Def is WeaponDef)
                .ToList();

            if (componentRefs.Count == 0)
            {
                return;
            }

            try
            {
                calculator = new WeaponComponentPrefabCalculator(chassisDef, componentRefs);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }

        internal static void ResetCalculator()
        {
            calculator = null;
        }

        public static bool Prefix(HardpointDataDef hardpointDataDef, BaseComponentRef componentRef, ref List<string> usedPrefabNames, ref string __result)
        {
            try
            {
                if (calculator != null && componentRef is MechComponentRef mechComponentRef && mechComponentRef.ComponentDefType == ComponentType.Weapon)
                {
                    __result = calculator.GetPrefabName(mechComponentRef) ?? hardpointDataDef.HardpointData[0].weapons[0][0];
                    usedPrefabNames.Add(__result);
                    return false;
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
            return true;
        }

        //public static void Postfix(BaseComponentRef componentRef, ref string __result)
        //{
        //    Control.mod.Logger.Log($"selected {__result} for {componentRef.ComponentDefID}");
        //}
    }
}