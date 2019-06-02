using BattleTech;
using MechEngineer.Features.AccuracyEffects;
using MechEngineer.Features.CriticalEffects;

namespace MechEngineer.Features.LocationalEffects
{
    internal class LocationalEffectsFeature : Feature
    {
        internal static LocationalEffectsFeature Shared = new LocationalEffectsFeature();

        internal override bool Enabled => (AccuracyEffectsFeature.settings?.Enabled ?? false) || (CriticalEffectsFeature.settings?.Enabled ?? false);

        internal static bool ProcessLocationalEffectData(ref EffectData effect, MechComponent mechComponent)
        {
            if (effect.effectType == EffectType.StatisticEffect
                && LocationNaming.IsLocational(effect.Description.Id)
                && LocationNaming.Create(mechComponent, out var naming))
            {
                var data = effect.ToJSON();
                effect = new EffectData();
                effect.FromJSON(data);

                Control.mod.Logger.LogDebug($"Replacing location in {effect.Description.Id} with {naming.LocationId}");

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
