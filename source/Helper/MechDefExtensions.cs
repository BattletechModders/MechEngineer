using BattleTech;

namespace MechEngineer
{
    public static class MechDefExtensions
    {
        public static float ArmorTonnage(this MechDef mechDef)
        {
            return mechDef.MechDefAssignedArmor / (UnityGameInstance.BattleTechGame.MechStatisticsConstants.ARMOR_PER_TENTH_TON * 10f);
        }
    }
}