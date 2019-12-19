using BattleTech;

namespace MechEngineer.Features.DynamicSlots
{
    public class DynamicSlotsSettings : ISettings
    {
        public bool Enabled { get; set; } = true;
        public string EnabledDescription => "Allows components to take up space dynamically on a mech.";

        public bool DynamicSlotsValidateDropEnabled = true;
        public string DynamicSlotsValidateDropEnabledDescription => "Don't allow dropping of items that would exceed the available slots.";

        public string ReservedSlotText = "reserved slot";
        public string MovableSlotText = "movable slot";
    }
}