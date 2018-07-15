using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(MechHardpointRules), "GetComponentPrefabName")]
    public static class MechHardpointRulesGetComponentPrefabNamePatch
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

        // ReSharper disable once RedundantAssignment
        public static bool Prefix(HardpointDataDef hardpointDataDef, BaseComponentRef componentRef, string prefabBase, string location, ref List<string> usedPrefabNames, ref string __result)
        {
            try
            {
                if (componentRef is MechComponentRef mechComponentRef && mechComponentRef.ComponentDefType == ComponentType.Weapon)
                {
                    __result = calculator?.GetPrefabName(mechComponentRef);
                }

                return __result == null;
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
                return true;
            }
        }
    }
}