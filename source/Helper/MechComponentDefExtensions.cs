using BattleTech;
using CustomComponents;

namespace MechEngineer
{
    internal static class MechComponentDefExtensions
    {
        internal static bool HasCustomFlag(this MechComponentDef def, string flag)
        {
            return def.Is<Flags>(out var f) && f.IsSet(flag);
        }

        internal static bool HasComponentTag(this MechComponentDef def, string tag)
        {
            return def.ComponentTags.Contains(tag);
        }
    }
}