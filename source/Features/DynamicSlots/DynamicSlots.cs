using BattleTech.UI;
using CustomComponents;
using Localize;

namespace MechEngineer.Features.DynamicSlots
{
    [CustomComponent("DynamicSlots")]
    public class DynamicSlots : SimpleCustomComponent
    {
        public int ReservedSlots { get; set; }

        public bool? ShowIcon { get; set; } = false;
        public bool? ShowFixedEquipmentOverlay { get; set; } = true;

        public string NameText { get; set; } = ""; // null: use component name
        public string BonusAText { get; set; } = ""; // null: use component bonus, "": dont show
        public string BonusBText { get; set; } = ""; // null: use component bonus, "": dont show
        public string BackgroundColor { get; set; } = null; // null: use component color

        internal void ApplyTo(MechLabItemSlotElement element, bool isReservedSlot)
        {
            var adapter = new MechLabItemSlotElementAdapter(element);

            if (NameText == "")
            {
                adapter.nameText.gameObject.SetActive(false);
            }
            else if (NameText != null)
            {
                adapter.nameText.text = NameText;
                adapter.nameText.gameObject.SetActive(true);
            }

            if (BonusAText == "")
            {
                adapter.bonusTextA.gameObject.SetActive(false);
            }
            else if (BonusAText != null)
            {
                adapter.bonusTextA.text = BonusAText;
                adapter.bonusTextA.gameObject.SetActive(true);
            }
                
            if (BonusBText == "")
            {
                adapter.bonusTextB.gameObject.SetActive(false);
            }
            else if (BonusBText != null)
            {
                adapter.bonusTextB.text = BonusBText;
                adapter.bonusTextB.gameObject.SetActive(true);
            }

            if (!string.IsNullOrEmpty(BackgroundColor))
            {
                adapter.backgroundColor.SetColorFromString(BackgroundColor);
            }

            if (ShowIcon.HasValue)
            {
                adapter.icon.gameObject.SetActive(ShowIcon.Value);
            }

            if (ShowFixedEquipmentOverlay.HasValue)
            {
                adapter.fixedEquipmentOverlay.gameObject.SetActive(ShowFixedEquipmentOverlay.Value);
            }
            
            var text = isReservedSlot ? DynamicSlotsFeature.settings.ReservedSlotText : DynamicSlotsFeature.settings.MovableSlotText;
            adapter.bonusTextA.SetText(new Text(text).ToString());
            adapter.bonusTextA.gameObject.SetActive(true);
        }
    }
}