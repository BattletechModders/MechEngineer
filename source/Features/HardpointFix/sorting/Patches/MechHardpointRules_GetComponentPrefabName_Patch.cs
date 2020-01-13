using BattleTech;
using Harmony;
using MechEngineer.Features.HardpointFix.utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MechEngineer.Features.HardpointFix.sorting.Patches
{
    [HarmonyPatch(typeof(MechHardpointRules), nameof(MechHardpointRules.GetComponentPrefabName))]
    public static class MechHardpointRules_GetComponentPrefabName_Patch
    {
        internal static WeaponComponentPrefabCalculator calculator;

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

        public static bool Prefix(BaseComponentRef componentRef, ref string __result)
        {
            try
            {
                if (calculator != null)
                {
                    __result = calculator.GetPrefabName(componentRef);
                    return false;
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
            return true;
        }

        public static void Postfix(BaseComponentRef componentRef, ref string __result)
        {
            try
            {
                if (componentRef is MechComponentRef mechComponentRef
                    && mechComponentRef.ComponentDefType == ComponentType.Weapon
                    && string.IsNullOrEmpty(__result))
                {
                    Control.mod.Logger.LogDebug($"no prefabName mapped for weapon ComponentDefID={mechComponentRef.ComponentDefID} PrefabIdentifier={mechComponentRef.Def.PrefabIdentifier}");
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}