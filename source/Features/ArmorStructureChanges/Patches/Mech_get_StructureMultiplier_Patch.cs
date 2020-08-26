using System;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.ArmorStructureChanges.Patches
{
    [HarmonyPatch(typeof(Mech), "get_StructureMultiplier")]
    public static class Mech_get_StructureMultiplier_Patch
    {
        public static void Postfix(Mech __instance, ref float __result)
        {
            try
            {
                __result = __result * ArmorStructureChangesFeature.GetStructureFactorForMech(__instance);
            }
            catch (Exception e)
            {
                Control.Logger.Error.Log(e);
            }
        }
    }
}
