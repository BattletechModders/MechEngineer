using System;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.OverrideGhostVFX.Patches
{
    [HarmonyPatch(typeof(PilotableActorRepresentation), nameof(PilotableActorRepresentation.OnPlayerVisibilityChanged))]
    public static class PilotableActorRepresentation_OnPlayerVisibilityChanged_Patch
    {
        public static void Postfix(PilotableActorRepresentation __instance)
        {
            try
            {
                var rep = __instance;
                OverrideGhostVFXFeature.Settings.BlipGhostType blip;
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
                if (blip == OverrideGhostVFXFeature.Settings.BlipGhostType.Weak)
                {
                    rep.BlipObjectGhostWeak.SetActive(true);
                }
                else if (blip == OverrideGhostVFXFeature.Settings.BlipGhostType.Strong)
                {
                    rep.BlipObjectGhostStrong.SetActive(true);
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}