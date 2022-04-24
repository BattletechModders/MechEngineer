using BattleTech;
using MechEngineer.Features.AccuracyEffects;
using MechEngineer.Features.CriticalEffects;

namespace MechEngineer.Features.PlaceholderEffects;

internal class PlaceholderEffectsFeature : Feature<PlaceholderEffectsSettings>
{
    internal static readonly PlaceholderEffectsFeature Shared = new();

    // TODO introduce nice dependency resolver
    internal override bool Enabled => Settings.Enabled && (AccuracyEffectsFeature.Shared.Settings?.Enabled ?? false) || (CriticalEffectsFeature.Shared.Settings?.Enabled ?? false);

    internal override PlaceholderEffectsSettings Settings => Control.settings.PlaceholderEffects;

    internal static bool ProcessLocationalEffectData(ref EffectData effect, MechComponent mechComponent)
    {
        if (effect.effectType == EffectType.StatisticEffect
            && PlaceholderInterpolation.Create(effect.Description.Id, mechComponent, out var naming))
        {
            var data = effect.ToJSON();
            effect = new EffectData();
            effect.FromJSON(data);

            Control.Logger.Debug?.Log($"Replacing placeholders in {effect.Description.Id} with {naming.LocationId}");

            effect.statisticData.statName = naming.InterpolateStatisticName(effect.statisticData.statName);

            effect.Description = new BaseDescriptionDef(
                naming.InterpolateEffectId(effect.Description.Id),
                naming.InterpolateText(effect.Description.Name),
                naming.InterpolateText(effect.Description.Details),
                naming.InterpolateText(effect.Description.Icon)
            );

            return true;
        }

        return false;
    }
}