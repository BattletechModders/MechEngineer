using System;
using System.Linq;
using BattleTech;
using Harmony;
using UnityEngine;

namespace MechEngineer.Features.OverrideGhostVFX.Patches
{
    [HarmonyPatch(typeof(GameRepresentation), nameof(GameRepresentation.PlayVFXAt))]
    public static class GameRepresentation_PlayVFXAt_Patch
    {
        public static bool Prefix(string vfxName)
        {
            try
            {
                if (Control.settings.FeatureOverrideGhostVFX.Blacklisted.Contains(vfxName))
                {
                    Control.mod.Logger.LogDebug($"skipped {vfxName}");
                    return false;
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }

            return true;
        }
    }
}
