using BattleTech;

namespace MechEngineer.Features.CriticalEffects;

public static class CriticalEffectsMechComponentExtensions
{
    internal static Criticals Criticals(this MechComponent mechComponent)
    {
        return new(mechComponent);
    }
}