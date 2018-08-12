using BattleTech.UI;
using SVGImporter;
using TMPro;

namespace MechEngineer
{
    public class MechLabItemSlotElementAdapter : Adapter<MechLabItemSlotElement>
    {
        public MechLabItemSlotElementAdapter(MechLabItemSlotElement instance) : base(instance)
        {
        }
         
        public SVGImage icon => traverse.Field("icon").GetValue<SVGImage>();

        public TextMeshProUGUI bonusTextA => traverse.Field("bonusTextA").GetValue<TextMeshProUGUI>();

        public TextMeshProUGUI bonusTextB => traverse.Field("bonusTextB").GetValue<TextMeshProUGUI>();

        public TextMeshProUGUI nameText => traverse.Field("nameText").GetValue<TextMeshProUGUI>();

        public UIColorRefTracker backgroundColor => traverse.Field("backgroundColor").GetValue<UIColorRefTracker>();
    }
}