using BattleTech;

namespace MechEngineer.Features.CriticalEffects;

public static class CriticalEffectsMechComponentExtensions
{
    internal static ComponentCriticals Criticals(this MechComponent mechComponent)
    {
        return new(mechComponent);
    }

    internal static CriticalEffects CriticalEffects(this MechComponent mechComponent)
    {
        return new(mechComponent);
    }
}