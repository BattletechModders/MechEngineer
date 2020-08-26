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
                .Select(c => {
                    if (c.DataManager == null)
                    {
                        c.DataManager = UnityGameInstance.BattleTechGame.DataManager;
                        c.RefreshComponentDef();
                    }
                    return c;
                })
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
                Control.Logger.Error.Log(e);
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
                Control.Logger.Error.Log(e);
            }
            return true;
        }

        public static void Postfix(BaseComponentRef componentRef, ref string __result)
        {
            try
            {
                if (HardpointFixFeature.Shared.Settings.TraceLogDebugMappings || (componentRef.ComponentDefType == ComponentType.Weapon && string.IsNullOrEmpty(__result)))
                {
                    Control.Logger.Debug?.Log($"GetComponentPrefabName prefabName={__result} ComponentDefID={componentRef.ComponentDefID} PrefabIdentifier={componentRef.Def.PrefabIdentifier}");
                }
            }
            catch (Exception e)
            {
                Control.Logger.Error.Log(e);
            }
        }
    }
}