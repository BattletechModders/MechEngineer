using CustomComponents;

namespace MechEngineer.Features.DynamicSlots
{
    [CustomComponent("DynamicSlots")]
    public class DynamicSlots : SimpleCustomComponent
    {
        public int ReservedSlots { get; set; }
    }
}