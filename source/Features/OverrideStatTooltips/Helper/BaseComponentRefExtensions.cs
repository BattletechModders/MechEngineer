using BattleTech;

namespace MechEngineer.Features.OverrideStatTooltips.Helper;

internal static class BaseComponentRefExtensions
{
    internal static WeaponRefDataHelper WeaponRefHelper(this BaseComponentRef componentRef)
    {
        return new(componentRef);
    }
}