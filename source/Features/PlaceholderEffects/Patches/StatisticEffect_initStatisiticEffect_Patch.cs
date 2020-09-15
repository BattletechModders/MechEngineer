using System;
using System.Linq;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.PlaceholderEffects.Patches
{
    [HarmonyPatch(typeof(StatisticEffect), "initStatisiticEffect")]
    public static class StatisticEffect_initStatisiticEffect_Patch
    {
        public static void Postfix(StatisticEffect __instance, ICombatant target, EffectData effectData)
        {
            try
            {
                if (!(target is Mech mech))
                {
                    return;
                }
                var effectID = __instance.id;
                var parts = effectID.Split(PlaceholderEffectsFeature.Shared.Settings.ComponentEffectStatisticSeparator);
                if (parts.Length < 2 || parts[0] != PlaceholderEffectsFeature.Shared.Settings.ComponentEffectStatisticPrefix)
                {
                    return;
                }

                var uid = parts[1];
                var component = mech.allComponents.FirstOrDefault(x => x.uid == uid);
                if (component == null)
                {
                    return;
                }
                Traverse.Create(__instance).Property<StatCollection>("statCollection").Value = component.StatCollection;
            }
            catch (Exception e)
            {
                Control.Logger.Error.Log(e);
            }
        }
    }
}
