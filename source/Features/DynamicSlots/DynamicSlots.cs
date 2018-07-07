
using BattleTech.UI;
using CustomComponents;

namespace MechEngineer
{
    [CustomComponent("DynamicSlots")]
    public class DynamicSlots : SimpleCustomComponent
    {
        public int ReservedSlots { get; set; }

        public UIColor ReservedSlotColor { get; set; }
    }
}