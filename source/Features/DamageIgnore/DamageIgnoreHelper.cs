using BattleTech;
using CustomComponents;

namespace MechEngineer.Features.DamageIgnore
{
    internal static class DamageIgnoreHelper
    {
        public static bool IsIgnoreDamage(this MechComponentDef def)
        {
            return def.HasCustomFlag("ignore_damage");
        }

        public static int OverrideLocation(this MechComponent component)
        {
            if (component.componentDef.IsIgnoreDamage())
            {
                return 0;
            }

            return component.Location;
        }
    }
}