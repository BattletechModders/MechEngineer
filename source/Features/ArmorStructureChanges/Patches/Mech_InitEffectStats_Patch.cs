using System;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.ArmorStructureChanges.Patches
{
    [HarmonyPatch(typeof(Mech), "InitEffectStats")]
    public static class Mech_InitEffectStats_Patch
    {
        public static void Prefix(Mech __instance)
        {
            try
            {
                ArmorStructureChangesFeature.Shared.InitEffectStats(__instance);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}