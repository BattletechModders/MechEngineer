using System.Linq;
using BattleTech;
using CustomComponents;

namespace MechEngineer.Features.ArmorStructureChanges;

internal class ArmorStructureChangesFeature : Feature<ArmorStructureChangesSettings>
{
    internal static readonly ArmorStructureChangesFeature Shared = new();

    internal override ArmorStructureChangesSettings Settings => Control.settings.ArmorStructureChanges;

    internal void InitStats(Mech mech)
    {
        // statcollection is only used to undo the multiplier, no interaction with any statusEffects
        mech.StatCollection.ArmorMultiplier().CreateWithDefault(GetArmorFactorForMechDef(mech.MechDef));
        mech.StatCollection.StructureMultiplier().CreateWithDefault(GetStructureFactorForMechDef(mech.MechDef));
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