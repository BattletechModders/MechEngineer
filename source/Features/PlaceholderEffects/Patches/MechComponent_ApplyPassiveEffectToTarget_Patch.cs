using System;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.PlaceholderEffects.Patches;

[HarmonyPatch(typeof(MechComponent), nameof(MechComponent.ApplyPassiveEffectToTarget))]
public static class MechComponent_ApplyPassiveEffectToTarget_Patch
{
    [HarmonyPrefix]
    public static void Prefix(MechComponent __instance, ref EffectData effect)
    {
        try
        {
            PlaceholderEffectsFeature.ProcessLocationalEffectData(ref effect, __instance);
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
    }
}
