using BattleTech;

namespace MechEngineer.Features.OverrideDescriptions
{
    public class MechComponentDefAdapter : Adapter<MechComponentDef>
    {
        public MechComponentDefAdapter(MechComponentDef instance) : base(instance)
        {
        }
        
        public string BonusValueA
        {
            set => traverse.Property<string>(nameof(MechComponentDef.BonusValueA)).Value = value;
            get => traverse.Property<string>(nameof(MechComponentDef.BonusValueA)).Value;
        }
        
        public string BonusValueB
        {
            set => traverse.Property<string>(nameof(MechComponentDef.BonusValueB)).Value = value;
            get => traverse.Property<string>(nameof(MechComponentDef.BonusValueB)).Value;
        }
    }
}