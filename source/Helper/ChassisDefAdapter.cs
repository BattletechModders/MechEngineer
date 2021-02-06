using BattleTech;

namespace MechEngineer.Helper
{
    internal class ChassisDefAdapter : Adapter<ChassisDef>
    {
        internal ChassisDefAdapter(ChassisDef instance) : base(instance)
        {
        }

        internal LocationDef[] Locations
        {
            get => traverse.Field("Locations").GetValue<LocationDef[]>();
            set => traverse.Field("Locations").SetValue(value);
        }

        internal void refreshLocationReferences()
        {
            traverse.Method("refreshLocationReferences").GetValue();
        }
    }
}