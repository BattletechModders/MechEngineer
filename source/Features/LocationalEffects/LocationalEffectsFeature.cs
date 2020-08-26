using BattleTech;
using MechEngineer.Features.AccuracyEffects;
using MechEngineer.Features.CriticalEffects;

namespace MechEngineer.Features.LocationalEffects
{
    internal class LocationalEffectsFeature : Feature<LocationalEffectsSettings>
    {
        internal static LocationalEffectsFeature Shared = new LocationalEffectsFeature();

        // TODO introduce nice dependency resolver
        internal override bool Enabled => Settings.Enabled && (AccuracyEffectsFeature.Shared.Settings?.Enabled ?? false) || (CriticalEffectsFeature.Shared.Settings?.Enabled ?? false);

        internal override LocationalEffectsSettings Settings => Control.settings.LocationalEffects;

        internal static bool ProcessLocationalEffectData(ref EffectData effect, MechComponent mechComponent)
        {
            if (effect.effectType == EffectType.StatisticEffect
                && LocationNaming.IsLocational(effect.Description.Id)
                && LocationNaming.Create(mechComponent, out var naming))
            {
                var data = effect.ToJSON();
                effect = new EffectData();
                effect.FromJSON(data);

                Control.Logger.Debug?.Log($"Replacing location in {effect.Description.Id} with {naming.LocationId}");

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
}
