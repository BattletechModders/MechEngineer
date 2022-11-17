using System;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.ArmorStructureChanges.Patches;

[HarmonyPatch(typeof(Mech), nameof(Mech.InitStats))]
public static class Mech_InitStats_Patch
{
    [HarmonyPrefix]
    public static void Prefix(Mech __instance)
    {
        try
        {
            if (!__instance.Combat.IsLoadingFromSave)
            {
                ArmorStructureChangesFeature.Shared.InitStats(__instance);
            }
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
    }
}
