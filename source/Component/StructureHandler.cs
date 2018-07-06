using System.Linq;
using BattleTech;

namespace MechEngineer
{
    internal class StructureHandler : ArmorStructureBase
    {
        internal static StructureHandler Shared = new StructureHandler();

        public override string CategoryName { get; } = "Structure";

        public override bool IsCustomType(MechComponentDef def)
        {
            return def is StructureDef;
        }

        protected override WeightSavings CalculateWeightSavings(MechDef mechDef, MechComponentDef def = null)
        {
            var tonnage = mechDef.Chassis.Tonnage / 10f;

            var slots = mechDef.Inventory.Select(c => c.Def).Where(IsCustomType).ToList();

            return WeightSavings.Create(tonnage, slots, def);
        }
    }
}