using BattleTech;

namespace MechEngineer.Features.DynamicSlots;

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
        ChassisLocations.RightArm
    };
    public string LocationPriorityOrderDescription = "From highest to lowest priority where to add dynamic slots too, relevant if locations have same amount of free slots. Visual impact only.";

    public bool? DefaultShowIcon = false;
    public string DefaultShowIconDescription => DefaultValueDescription;

    public bool? DefaultShowFixedEquipmentOverlay = true;
    public string DefaultShowFixedEquipmentOverlayDescription => DefaultValueDescription;

    public string? DefaultNameText = "";
    public string DefaultNameTextDescription => DefaultTextDescription;

    public string? DefaultBonusATextIfReservedSlot = "reserved slot";
    public string DefaultBonusATextIfReservedSlotDescription => DefaultTextDescription;

    public string? DefaultBonusATextIfMovableSlot = "movable slot";
    public string DefaultBonusATextIfMovableSlotDescription => DefaultTextDescription;

    public string? DefaultBonusBText = "";
    public string DefaultBonusBTextDescription => DefaultTextDescription;

    public string? DefaultBackgroundColor = null;
    public string DefaultBackgroundColorDescription => DefaultValueDescription;

    private static string DefaultValueDescription => @"null: use component value, value: overwrite component value";
    private static string DefaultTextDescription => @"null: use component value, """": dont show, ""something"": show as ""something""";
}