using CustomComponents;

namespace MechEngineer.Features.MechLabSlots;

[CustomComponent("CustomWidgetChassis")]
public class CustomWidgetChassisCustom : SimpleCustomChassis
{
    internal MechLabSlotsSettings.WidgetOverrideSettings TopLeftWidget { get; set; } = new();
    internal MechLabSlotsSettings.WidgetOverrideSettings TopRightWidget { get; set; } = new();
}
