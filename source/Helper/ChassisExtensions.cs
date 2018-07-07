using BattleTech;

namespace MechEngineer
{
    public static class ChassisDefExtensions
    {
        public static float DefaultStructureTonnage(this ChassisDef chassisDef)
        {
            return chassisDef.Tonnage / 10f;
        }
    }
}