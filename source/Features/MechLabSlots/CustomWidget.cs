using CustomComponents;

namespace MechEngineer.Features.MechLabSlots;

[CustomComponent("CustomWidget")]
public class CustomWidget : SimpleCustomComponent, IValueComponent<CustomWidget.MechLabWidgetLocation>
{
    public MechLabWidgetLocation Location { get; set; } = MechLabWidgetLocation.TopLeft;

    public void LoadValue(MechLabWidgetLocation location)
    {
        Location = location;
    }

    public enum MechLabWidgetLocation
    {
        TopLeft,
        TopRight
    }
}