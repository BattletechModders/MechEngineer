using BattleTech;

namespace MechEngineer.Helper;

public static class MechDefExtensions
{
    public static bool IgnoreAutofix(this MechDef def)
    {
        return def.MechTags.IgnoreAutofix();
    }
}