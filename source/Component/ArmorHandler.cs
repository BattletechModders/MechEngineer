using System.Linq;
using BattleTech;

namespace MechEngineer
{
    internal class ArmorHandler : ArmorStructureBase
    {
        internal static ArmorHandler Shared = new ArmorHandler();

        public override bool IsComponentDef(MechComponentDef def)
        {
            return def.CheckComponentDef(ComponentType.Upgrade, Control.settings.ArmorPrefix);
        }

        protected override WeightSavings CalculateWeightSavings(MechDef mechDef, MechComponentDef def = null)
        {
            var num = 0f;
            num += mechDef.Head.AssignedArmor;
            num += mechDef.CenterTorso.AssignedArmor;
            num += mechDef.CenterTorso.AssignedRearArmor;
            num += mechDef.LeftTorso.AssignedArmor;
            num += mechDef.LeftTorso.AssignedRearArmor;
            num += mechDef.RightTorso.AssignedArmor;
            num += mechDef.RightTorso.AssignedRearArmor;
            num += mechDef.LeftArm.AssignedArmor;
            num += mechDef.RightArm.AssignedArmor;
            num += mechDef.LeftLeg.AssignedArmor;
            num += mechDef.RightLeg.AssignedArmor;
            var tonnage = num / (UnityGameInstance.BattleTechGame.MechStatisticsConstants.ARMOR_PER_TENTH_TON * 10f);

            var slots = mechDef.Inventory.Select(c => c.Def).Where(IsComponentDef).ToList();

            return WeightSavings.Create(tonnage, slots, Control.settings.ArmorTypes, def);
        }
    }
}