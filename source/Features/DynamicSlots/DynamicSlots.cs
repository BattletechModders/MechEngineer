using System.Collections.Generic;
using BattleTech.UI;
using CustomComponents;
using fastJSON;
using UnityEngine;

namespace MechEngineer.Features.DynamicSlots
{
    [CustomComponent("DynamicSlots")]
    public class DynamicSlots : SimpleCustomComponent, IAfterLoad
    {
        public int ReservedSlots { get; set; }

        public bool? ShowIcon { get; set; } = false;
        public bool? ShowFixedEquipmentOverlay { get; set; } = true;

        public string NameText { get; set; } = ""; // null: use component name
        public string BonusAText { get; set; } = "dynamic slot"; // null: use component bonus, "": dont show
        public string BonusBText { get; set; } = ""; // null: use component bonus, "": dont show
        public UIColor? BackgroundColor { get; set; } = null; // null: use component color
        public string BacgroundRGBColor { get; set; } = null;
        [JsonIgnore]
        public Color CustomColor { get; set; }

        public void OnLoaded(Dictionary<string, object> values)
        {
            if (ColorUtility.TryParseHtmlString(BacgroundRGBColor, out var color))
            {
                CustomColor = color;
                BackgroundColor = UIColor.Custom;
            }
            else
                CustomColor = UnityEngine.Color.magenta;
        }
    }
}