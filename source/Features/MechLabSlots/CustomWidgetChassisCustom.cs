using CustomComponents;

namespace MechEngineer.Features.MechLabSlots;

[CustomComponent("CustomWidgetChassis")]
public class CustomWidgetChassisCustom : SimpleCustomChassis
{
    public bool? TopLeftWidgetEnabled { get; set; }
    public bool? TopRightWidgetEnabled { get; set; }
}
