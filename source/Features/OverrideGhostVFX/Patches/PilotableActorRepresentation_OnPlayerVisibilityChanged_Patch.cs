using System;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.OverrideGhostVFX.Patches;

[HarmonyPatch(typeof(PilotableActorRepresentation), nameof(PilotableActorRepresentation.OnPlayerVisibilityChanged))]
public static class PilotableActorRepresentation_OnPlayerVisibilityChanged_Patch
{
    public static void Postfix(PilotableActorRepresentation __instance)
    {
        try
        {
            var rep = __instance;
            OverrideGhostVFXSettings.BlipGhostType blip;
            if (rep.BlipObjectGhostWeak.activeSelf)
            {
                blip = Control.settings.OverrideGhostVFX.BlipWeak;
            }
            else if (rep.BlipObjectGhostStrong.activeSelf)
            {
                blip = Control.settings.OverrideGhostVFX.BlipStrong;
            }
            else
            {
                return;
            }

            rep.BlipObjectGhostWeak.SetActive(false);
            rep.BlipObjectGhostStrong.SetActive(false);
            if (blip == OverrideGhostVFXSettings.BlipGhostType.Weak)
            {
                rep.BlipObjectGhostWeak.SetActive(true);
            }
            else if (blip == OverrideGhostVFXSettings.BlipGhostType.Strong)
            {
                rep.BlipObjectGhostStrong.SetActive(true);
            }
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
    }
}