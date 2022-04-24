using BattleTech;

namespace MechEngineer.Helper;

internal static class MechComponentRefExtensions
{
    internal static bool IsFunctionalORInstalling(this MechComponentRef componentRef)
    {
        return componentRef.DamageLevel < ComponentDamageLevel.NonFunctional || componentRef.DamageLevel == ComponentDamageLevel.Installing;
    }
}