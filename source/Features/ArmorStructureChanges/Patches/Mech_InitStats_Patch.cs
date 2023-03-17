using System;
using BattleTech;

namespace MechEngineer.Features.ArmorStructureChanges.Patches;

[HarmonyPatch(typeof(Mech), nameof(Mech.InitStats))]
public static class Mech_InitStats_Patch
{
    [HarmonyPrefix]
    public static void Prefix(ref bool __runOriginal, Mech __instance)
    {
        if (!__runOriginal)
        {
            return;
        }

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
