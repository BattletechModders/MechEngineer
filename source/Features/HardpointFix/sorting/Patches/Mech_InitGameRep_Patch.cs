using System;
using System.Linq;
using BattleTech;
using Harmony;
using UnityEngine;

namespace MechEngineer.Features.HardpointFix.sorting.Patches
{
    [HarmonyPatch(typeof(Mech), "InitGameRep")]
    public static class Mech_InitGameRep_Patch
    {
        public static void Prefix(Mech __instance, Transform parentTransform)
        {
            try
            {
                var componentRefs = __instance.Weapons.Union(__instance.supportComponents)
                    .Select(w => w.baseComponentRef as MechComponentRef)
                    .Where(c => c != null)
                    .ToList();

                MechHardpointRules_GetComponentPrefabName_Patch.SetupCalculator(__instance.MechDef.Chassis, componentRefs);
            }
            catch (Exception e)
            {
                Control.Logger.Error.Log(e);
            }
        }

        public static void Postfix(Mech __instance, Transform parentTransform)
        {
            MechHardpointRules_GetComponentPrefabName_Patch.ResetCalculator();
        }
    }
}