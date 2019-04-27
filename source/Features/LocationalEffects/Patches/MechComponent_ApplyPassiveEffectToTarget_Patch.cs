using System;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.LocationalEffects.Patches
{
    [HarmonyPatch(typeof(MechComponent), "ApplyPassiveEffectToTarget")]
    public static class MechComponent_ApplyPassiveEffectToTarget_Patch
    {
        public static void Prefix(MechComponent __instance, ref EffectData effect)
        {
            try
            {
                LocationalEffectsHandler.ProcessLocationalEffectData(ref effect, __instance);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}
