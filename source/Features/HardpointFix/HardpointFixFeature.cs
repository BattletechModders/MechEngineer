using System;
using System.Collections.Generic;
using System.Linq;

namespace MechEngineer.Features.HardpointFix;

internal class HardpointFixFeature : Feature<HardpointFixSettings>
{
    internal static readonly HardpointFixFeature Shared = new();

    internal override HardpointFixSettings Settings => Control.Settings.HardpointFix;

    private readonly Dictionary<string, string[]> _cachedCompatibleTerms = new();
    internal string[] GetCompatiblePrefabTerms(string prefabIdentifier)
    {
        prefabIdentifier = Prefab.NormIdentifier(prefabIdentifier);
        if (!_cachedCompatibleTerms.TryGetValue(prefabIdentifier, out var compatibleTerms))
        {
            compatibleTerms = Settings.WeaponPrefabMappings
                .Where(x => string.Equals(x.PrefabIdentifier, prefabIdentifier, StringComparison.OrdinalIgnoreCase))
                .Select(x => x.HardpointCandidates)
                .SingleOrDefault();

            if (compatibleTerms == null)
            {
                compatibleTerms = new[] {prefabIdentifier};
            }

            _cachedCompatibleTerms[prefabIdentifier] = compatibleTerms;
        }

        return compatibleTerms;
    }
}