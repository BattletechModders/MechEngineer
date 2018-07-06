
using BattleTech.UI;

namespace MechEngineer
{
    public interface IDynamicSlots
    {
        //DescriptionDef Description { get; }
        int ReservedSlots { get; }
        UIColor ReservedSlotColor { get; }
    }
}