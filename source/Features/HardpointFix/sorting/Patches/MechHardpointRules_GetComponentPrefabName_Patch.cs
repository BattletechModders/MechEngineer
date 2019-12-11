using System;
using System.Collections.Generic;
using System.Globalization;
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

        public static bool Prefix(BaseComponentRef componentRef, ref List<string> usedPrefabNames, ref string __result)
        {
            try
            {
                if (componentRef is MechComponentRef mechComponentRef)
                {
                    if (usedPrefabNames.Count == 0)
                    {
                        // make sure no other iteration can take away already reserved prefabs
                        usedPrefabNames.AddRange(calculator.GetUsedPrefabNamesInLocation(mechComponentRef.MountedLocation));
                    }

                    if (mechComponentRef.ComponentDefType == ComponentType.Weapon)
                    {
                        __result = calculator.GetPrefabName(mechComponentRef);

                        if (__result != null)
                        {
                            return false;
                        }

                        if (!HardpointFixFeature.Shared.Settings.CreateVanillaFallbackPrefabs)
                        {
                            return false;
                        }
                    }
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
                if (componentRef is MechComponentRef mechComponentRef)
                {
                    if (mechComponentRef.ComponentDefType == ComponentType.Weapon)
                    {
                        if (HardpointFixFeature.Shared.Settings.CreateVanillaFallbackPrefabs && string.IsNullOrEmpty(__result))
                        {
                            Control.mod.Logger.LogWarning($"no prefabName found for weapon defId={mechComponentRef.ComponentDefID} even with vanilla fallback code");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}