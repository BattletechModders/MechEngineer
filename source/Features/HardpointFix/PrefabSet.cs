using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MechEngineer.Features.CriticalEffects.Patches;

namespace MechEngineer.Features.HardpointFix;

internal class PrefabSet : IEnumerable<Prefab>
{
    internal int Index { get; }
    private Dictionary<string, Prefab> Prefabs { get; }

    public IEnumerator<Prefab> GetEnumerator()
    {
        return Prefabs.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    internal PrefabSet(int index, IEnumerable<string> prefabs)
        : this(index, prefabs
            .Distinct()
            .Select(x => new Prefab(x))
            .ToDictionary(x => x.Identifier))
    {
    }

    private PrefabSet(int index, Dictionary<string, Prefab> prefabs)
    {
        Index = index;
        Prefabs = prefabs;
    }

    internal PrefabSet CreateWithoutPrefabNames(HashSet<string> blacklistedPrefabNames)
    {
        var copy = Prefabs
            .Where(x => !blacklistedPrefabNames.Contains(x.Value.Name))
            .ToDictionary(x => x.Key, x => x.Value);
        return new PrefabSet(Index, copy);
    }

    internal bool IsEmpty => Prefabs.Count == 0;

    internal bool ContainsPrefab(Prefab prefab)
    {
        return Prefabs.Values.Any(x => x.Name.Equals(prefab.Name));
    }

    internal Prefab? GetPrefabByIdentifier(string prefabIdentifier)
    {
        if (Prefabs.TryGetValue(prefabIdentifier, out var value))
        {
            return value;
        }
        return null;
    }

    public override string ToString()
    {
        return $"{nameof(PrefabSet)}[index ={Index}, Prefabs=[{Prefabs.Values.JoinAsString()}]]";
    }
}