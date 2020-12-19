using System;
using System.Linq;
using BattleTech;
using Harmony;
using MechEngineer.Misc;

namespace MechEngineer.Features.HardpointFix.sorting.Patches
{
    [HarmonyPatch(typeof(Mech), "InitGameRep")]
    public static class Mech_InitGameRep_Patch
    {
        [HarmonyAfter(KFix.AC, KFix.CU)]
        public static void Prefix(Mech __instance)
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

        public static void Postfix(Mech __instance)
        {
            MechHardpointRules_GetComponentPrefabName_Patch.ResetCalculator();
        }
    }
}