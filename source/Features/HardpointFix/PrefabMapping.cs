using BattleTech;

namespace MechEngineer.Features.HardpointFix;

internal class PrefabMapping
{
    internal Prefab Prefab { get; }
    internal MechComponentRef MechComponentRef { get; }

    public PrefabMapping(Prefab prefab, MechComponentRef mechComponentRef)
    {
        Prefab = prefab;
        MechComponentRef = mechComponentRef;
    }

    public override string ToString()
    {
        return $"{nameof(PrefabMapping)}[Prefab={Prefab}, MechComponentRef={MechComponentRef.ComponentDefID}]";
    }
}