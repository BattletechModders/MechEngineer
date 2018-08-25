
using BattleTech.UI;
using CustomComponents;

namespace MechEngineer
{
    [CustomComponent("DynamicSlots")]
    public class DynamicSlots : SimpleCustomComponent
    {
        public int ReservedSlots { get; set; }

        public bool ShowIcon { get; set; } = false;
        public string NameText { get; set; } = ""; // null: use component name
        public string BonusAText { get; set; } = "dynamic slot"; // null: use component bonus, "": dont show
        public string BonusBText { get; set; } = ""; // null: use component bonus, "": dont show
        public UIColor? BackgroundColor { get; set; } = null; // null: use component color
    }
}