using System;
using System.Linq;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.OverrideGhostVFX.Patches;

[HarmonyPatch(typeof(GameRepresentation), nameof(GameRepresentation.PlayVFXAt))]
public static class GameRepresentation_PlayVFXAt_Patch
{
    [HarmonyPrefix]
    public static bool Prefix(string vfxName)
    {
        try
        {
            if (Control.Settings.OverrideGhostVFX.Blacklisted.Contains(vfxName))
            {
                Log.Main.Debug?.Log($"skipped {vfxName}");
                return false;
            }
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }

        return true;
    }
}
