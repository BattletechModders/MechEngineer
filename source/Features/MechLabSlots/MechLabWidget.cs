using System;
using CustomComponents;

namespace MechEngineer.Features.MechLabSlots
{
    [CustomComponent("MechLabWidget")]
    public class MechLabWidget : SimpleCustomComponent, IValueComponent<MechLabWidget.MechLabWidgetLocation>
    {
        public MechLabWidgetLocation Location { get; set; } = MechLabWidgetLocation.TopLeft;

        public enum MechLabWidgetLocation
        {
            TopLeft, TopRight
        }

        public void LoadValue(MechLabWidgetLocation location)
        {
            Location = location;
        }
    }
}