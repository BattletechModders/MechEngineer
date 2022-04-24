using System.Collections.Generic;
using System.Linq;
using MechEngineer.Features.CriticalEffects.Patches;

namespace MechEngineer.Features.HardpointFix;

internal class PrefabSelectionCandidate
{
    internal List<PrefabSet> FreeSets { get; }
    internal List<PrefabMapping> Mappings { get; }

    internal PrefabSelectionCandidate(List<PrefabSet> freeSets) : this(freeSets, new List<PrefabMapping>())
    {
    }

    private PrefabSelectionCandidate(List<PrefabSet> freeSets, List<PrefabMapping> prefabsMappings)
    {
        FreeSets = freeSets;
        Mappings = prefabsMappings;
    }

    internal PrefabSelectionCandidate CreateWithoutPrefab(Prefab exclude, PrefabMapping newMapping)
    {
        var blacklistedPrefabNames = FreeSets
            .Where(x => x.ContainsPrefab(exclude))
            .SelectMany(x => x)
            .Select(x => x.Name)
            .ToHashSet();

        var sets = FreeSets
            .Select(x => x.CreateWithoutPrefabNames(blacklistedPrefabNames))
            .Where(x => !x.IsEmpty)
            .ToList();

        var mappings = new List<PrefabMapping>(Mappings) {newMapping};
        return new PrefabSelectionCandidate(sets, mappings);
    }

    public override string ToString()
    {
        return $"{nameof(PrefabSelectionCandidate)}[freeSets={FreeSets.JoinAsString()},mappings=[{Mappings.JoinAsString()}]]";
    }
}