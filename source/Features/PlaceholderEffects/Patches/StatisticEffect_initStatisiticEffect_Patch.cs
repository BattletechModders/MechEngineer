using System;
using System.Linq;
using System.Text.RegularExpressions;
using BattleTech;
using Harmony;

namespace MechEngineer.Features.PlaceholderEffects.Patches;

[HarmonyPatch(typeof(StatisticEffect), nameof(StatisticEffect.initStatisiticEffect))]
public static class StatisticEffect_initStatisiticEffect_Patch
{
    public static void Postfix(StatisticEffect __instance, ICombatant target)
    {
        try
        {
            if (!(target is Mech mech))
            {
                return;
            }
            var effectID = __instance.id;

            var sep = PlaceholderEffectsFeature.Shared.Settings.ComponentEffectStatisticSeparator;
            var prefix = PlaceholderEffectsFeature.Shared.Settings.ComponentEffectStatisticPrefix;

            // effectID={EffectSource}_{effectData.Description.Id}_{guid}
            // descID={Prefix}-{uid}-label
            var match = Regex.Match(effectID, $"{prefix}{sep}([^{sep}]+){sep}", RegexOptions.Compiled);
            if (!match.Success)
            {
                return;
            }

            var uid = match.Groups[1].Value;
            var component = mech.allComponents.FirstOrDefault(x => x.uid == uid);
            if (component == null)
            {
                return;
            }
            __instance.statCollection = component.StatCollection;
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
    }
}