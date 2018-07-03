using CustomComponents;

namespace MechEngineer.Test
{
    [Custom("TestStructure")]
    public class TestStructureDef : CustomUpgradeDef<TestStructureDef>, ICategory, IDynamicSlots
    {
        public int ReserverdSlots { get; set; }

        public string CategoryID { get; set; }

        public string Tag { get; set; }

        public CategoryDescriptor CategoryDescriptor { get; set; }
    }
}

