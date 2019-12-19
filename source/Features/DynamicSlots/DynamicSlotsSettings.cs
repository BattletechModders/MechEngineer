using BattleTech;

namespace MechEngineer.Features.DynamicSlots
{
    public class DynamicSlotsSettings : ISettings
    {
        public bool Enabled { get; set; } = true;
        public string EnabledDescription => "Allows components to take up space dynamically on a mech.";

        public bool DynamicSlotsValidateDropEnabled = true;
        public string DynamicSlotsValidateDropEnabledDescription => "Don't allow dropping of items that would exceed the available slots.";

        public ChassisLocations[] LocationPriorityOrder =
        {
            ChassisLocations.CenterTorso,
            ChassisLocations.Head,
            ChassisLocations.LeftTorso,
            ChassisLocations.LeftLeg,
            ChassisLocations.RightTorso,
            ChassisLocations.RightLeg,
            ChassisLocations.LeftArm,
            ChassisLocations.RightArm,
        };
        public string LocationPriorityOrderDescription = "From highest to lowest priority where to add dynamic slots too, relevant if locations have same amount of free slots. Visual impact only.";

        public string ReservedSlotText = "reserved slot";
        public string MovableSlotText = "movable slot";
    }
}