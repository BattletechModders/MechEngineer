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
                OverrideGhostVFXFeatureSettings.BlipGhostType blip;
                if (rep.BlipObjectGhostWeak.activeSelf)
                {
                    blip = Control.settings.FeatureOverrideGhostVFX.BlipWeak;
                }
                else if (rep.BlipObjectGhostStrong.activeSelf)
                {
                    blip = Control.settings.FeatureOverrideGhostVFX.BlipStrong;
                }
                else
                {
                    return;
                }

                rep.BlipObjectGhostWeak.SetActive(false);
                rep.BlipObjectGhostStrong.SetActive(false);
                if (blip == OverrideGhostVFXFeatureSettings.BlipGhostType.Weak)
                {
                    rep.BlipObjectGhostWeak.SetActive(true);
                }
                else if (blip == OverrideGhostVFXFeatureSettings.BlipGhostType.Strong)
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