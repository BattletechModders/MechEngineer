using System.Linq;
using BattleTech;

namespace MechEngineer
{
    internal class StructureHandler : ArmorStructureBase
    {
        internal static StructureHandler Shared = new StructureHandler();

        public override bool IsComponentDef(MechComponentDef def)
        {
            return def.CheckComponentDef(ComponentType.Upgrade, Control.settings.StructurePrefix);
        }

        protected override WeightSavings CalculateWeightSavings(MechDef mechDef, MechComponentDef def = null)
        {
            var tonnage = mechDef.Chassis.Tonnage / 10f;

            var slots = mechDef.Inventory.Select(c => c.Def).Where(IsComponentDef).ToList();

            return WeightSavings.Create(tonnage, slots, Control.settings.StructureTypes, def);
        }
    }
}