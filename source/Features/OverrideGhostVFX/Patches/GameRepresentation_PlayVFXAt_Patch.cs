using System.Linq;
using BattleTech;

namespace MechEngineer.Features.OverrideGhostVFX.Patches;

[HarmonyPatch(typeof(GameRepresentation), nameof(GameRepresentation.PlayVFXAt))]
public static class GameRepresentation_PlayVFXAt_Patch
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, string vfxName)
    {
        if (!__runOriginal)
        {
            return;
        }

        if (Control.Settings.OverrideGhostVFX.Blacklisted.Contains(vfxName))
        {
            Log.Main.Debug?.Log($"skipped {vfxName}");
            __runOriginal = false;
        }
    }
}
