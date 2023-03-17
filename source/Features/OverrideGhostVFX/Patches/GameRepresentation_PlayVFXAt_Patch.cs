using System;
using System.Linq;
using BattleTech;

namespace MechEngineer.Features.OverrideGhostVFX.Patches;

[HarmonyPatch(typeof(GameRepresentation), nameof(GameRepresentation.PlayVFXAt))]
public static class GameRepresentation_PlayVFXAt_Patch
{
    [HarmonyPrefix]
    public static void Prefix(ref bool __runOriginal, string vfxName)
    {
        if (!__runOriginal)
        {
            return;
        }

        try
        {
            if (Control.Settings.OverrideGhostVFX.Blacklisted.Contains(vfxName))
            {
                Log.Main.Debug?.Log($"skipped {vfxName}");
                __runOriginal = false;
            }
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
    }
}
