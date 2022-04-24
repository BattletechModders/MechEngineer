using System;
using System.Linq;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.OverrideGhostVFX.Patches;

[HarmonyPatch(typeof(GameRepresentation), nameof(GameRepresentation.PlayVFXAt))]
public static class GameRepresentation_PlayVFXAt_Patch
{
    public static bool Prefix(string vfxName)
    {
        try
        {
            if (Control.settings.OverrideGhostVFX.Blacklisted.Contains(vfxName))
            {
                Control.Logger.Debug?.Log($"skipped {vfxName}");
                return false;
            }
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }

        return true;
    }
}