using System.Linq;
using BattleTech;
using CustomComponents;

namespace MechEngineer.Features.ArmorStructureChanges
{
    internal class ArmorStructureChangesFeature : Feature<ArmorStructureChangesSettings>
    {
        internal static ArmorStructureChangesFeature Shared = new ArmorStructureChangesFeature();

        internal override ArmorStructureChangesSettings Settings => Control.settings.ArmorStructureChanges;

        internal void InitEffectStats(Mech mech)
        {
            mech.StatCollection.ArmorMultiplier().Create(GetArmorFactorForMechDef(mech.MechDef));
            mech.StatCollection.StructureMultiplier().Create(GetStructureFactorForMechDef(mech.MechDef));
        }

        internal static float GetArmorFactorForMech(Mech mech)
        {
            return mech.StatCollection.ArmorMultiplier().Get();
        }

        internal static float GetStructureFactorForMech(Mech mech)
        {
            return mech.StatCollection.StructureMultiplier().Get();
        }

        private static float GetArmorFactorForMechDef(MechDef mechDef)
        {
            return mechDef.Inventory
                .Select(r => r.GetComponent<ArmorStructureChanges>())
                .Where(c => c != null)
                .Select(c => c.ArmorFactor)
                .Aggregate(1.0f, (acc, val) => acc * val);
        }

        private static float GetStructureFactorForMechDef(MechDef mechDef)
        {
            return mechDef.Inventory
                .Select(r => r.GetComponent<ArmorStructureChanges>())
                .Where(c => c != null)
                .Select(c => c.StructureFactor)
                .Aggregate(1.0f, (acc, val) => acc * val);
        }
    }
}
