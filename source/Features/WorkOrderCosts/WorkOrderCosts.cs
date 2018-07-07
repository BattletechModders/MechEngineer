using CustomComponents;

namespace MechEngineer
{
    [CustomComponent("WorkOrderCosts")]
    public class WorkOrderCosts : SimpleCustomComponent
    {
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
    }
}