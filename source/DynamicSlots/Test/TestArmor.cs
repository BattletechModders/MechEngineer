using CustomComponents;


namespace MechEngineer.Test
{

    [Custom("TestArmor")]
    public class TestArmorDef : CustomUpgradeDef<TestArmorDef>, ICategory, IDynamicSlots
    {
        public int ReservedSlots { get; set; }

        public string CategoryID { get; set; }

        public string Tag { get; set; }

        public CategoryDescriptor CategoryDescriptor { get; set; }
    }

}
