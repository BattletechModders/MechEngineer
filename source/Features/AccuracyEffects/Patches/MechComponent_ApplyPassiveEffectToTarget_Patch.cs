using System;
using System.Collections.Generic;
using BattleTech;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(MechComponent), "ApplyPassiveEffectToTarget")]
    public static class MechComponent_ApplyPassiveEffectToTarget_Patch
    {
        public static void Prefix(MechComponent __instance, ref EffectData effect)
        {
            try
            {
                LocationalEffects.ProcessLocationalEffectData(ref effect, __instance);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}
