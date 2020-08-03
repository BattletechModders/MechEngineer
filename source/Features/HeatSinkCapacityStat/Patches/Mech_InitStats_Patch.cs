using System;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.HeatSinkCapacityStat.Patches
{
    [HarmonyPatch(typeof(Mech), "InitStats")]
    public static class Mech_InitStats_Patch
    {
        public static void Postfix(Mech __instance)
        {
            try
            {
                if (!__instance.Combat.IsLoadingFromSave)
                {
                    HeatSinkCapacityStatFeature.Shared.InitStats(__instance);
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}