using System.Collections.Generic;
using System.Linq;
using BattleTech;
using CustomComponents;
using MechEngineer.Features.PlaceholderEffects;

namespace MechEngineer.Features.CriticalEffects;

internal class CriticalEffectsFeature : Feature<CriticalEffectsSettings>
{
    internal static readonly CriticalEffectsFeature Shared = new();

    internal override bool Enabled => base.Enabled && PlaceholderEffectsFeature.Shared.Loaded;

    internal override CriticalEffectsSettings Settings => Control.settings.CriticalEffects;

    internal static CriticalEffectsSettings settings => Shared.Settings;

    internal override void SetupResources(Dictionary<string, Dictionary<string, VersionManifestEntry>> customResources)
    {
        Resources = SettingsResourcesTools.Enumerate<EffectData>("MECriticalEffects", customResources)
            .ToDictionary(entry => entry.Description.Id);
    }

    private static Dictionary<string, EffectData> Resources { get; set; } = new();

    internal static EffectData GetEffectData(string effectId)
    {
        if (Resources.TryGetValue(effectId, out var effectData))
        {
            return effectData;
        }

        Control.Logger.Error.Log($"Can't find critical effect id '{effectId}'");
        return null;
    }
}