using BattleTech;

namespace MechEngineer
{
    public class DescriptionDefAdapter : Adapter<DescriptionDef>
    {
        public DescriptionDefAdapter(DescriptionDef instance) : base(instance)
        {
        }
        
        public string Details
        {
            set => traverse.Property<string>(nameof(DescriptionDef.Details)).Value = value;
            get => traverse.Property<string>(nameof(DescriptionDef.Details)).Value;
        }
    }
}