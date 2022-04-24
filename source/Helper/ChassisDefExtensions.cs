using BattleTech;

namespace MechEngineer.Helper;

public static class ChassisDefExtensions
{
    public static bool IgnoreAutofix(this ChassisDef def)
    {
        return def.ChassisTags.IgnoreAutofix();
    }
}