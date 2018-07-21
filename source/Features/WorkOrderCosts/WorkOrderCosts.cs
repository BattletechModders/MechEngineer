using System.Collections.Generic;
using CustomComponents;

namespace MechEngineer
{
    [CustomComponent("WorkOrderCosts")]
    public class WorkOrderCosts : SimpleCustomComponent, IAfterLoad
    {
        public Costs Default { get; set; }
        public Costs Install { get; set; }
        public Costs Repair { get; set; }
        public Costs RepairDestroyed { get; set; }
        public Costs Remove { get; set; }
        public Costs RemoveDestroyed { get; set; }

        public class Costs
        {
            public string TechCost { get; set; }
            public string CBillCost { get; set; }
        }

        public void OnLoaded(Dictionary<string, object> values)
        {
            Install = Install ?? Default;
            Repair = Repair ?? Default;
            RepairDestroyed = RepairDestroyed ?? Default;
            Remove = Remove ?? Default;
            RemoveDestroyed = RemoveDestroyed ?? Default;
        }
    }
}