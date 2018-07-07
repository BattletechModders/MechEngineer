using BattleTech;

namespace MechEngineer
{
    internal class WorkOrderEntry_MechLabAdapter : Adapter<WorkOrderEntry_MechLab>
    {
        internal WorkOrderEntry_MechLabAdapter(WorkOrderEntry_MechLab instance) : base(instance)
        {
        }

        internal int Cost
        {
            get => instance.GetCost();
            set => traverse.Field("Cost").SetValue(value);
        }

        internal int CBillCost
        {
            get => traverse.Field("CBillCost").GetValue<int>();
            set => traverse.Field("CBillCost").SetValue(value);
        }
    }
}