using System;
using CustomComponents;

namespace MechEngineer.Features.MechLabSlots
{
    [CustomComponent("MechLabWidget")]
    public class MechLabWidget : SimpleCustomComponent, IValueComponent
    {
        public MechLabWidgetLocation Location { get; set; } = MechLabWidgetLocation.TopLeft;

        public enum MechLabWidgetLocation
        {
            TopLeft, TopRight
        }

        public void LoadValue(object value)
        {
            if (value is string str && Enum.TryParse(str, true, out MechLabWidgetLocation location))
            {
                Location = location;
            }
        }
    }
}