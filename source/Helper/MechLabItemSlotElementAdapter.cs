using BattleTech.UI;
using TMPro;

namespace MechEngineer
{
    public class MechLabItemSlotElementAdapter : Adapter<MechLabItemSlotElement>
    {
        public MechLabItemSlotElementAdapter(MechLabItemSlotElement instance) : base(instance)
        {
        }

        public TextMeshProUGUI bonusTextA => traverse.Field("bonusTextA").GetValue<TextMeshProUGUI>();

        public TextMeshProUGUI bonusTextB => traverse.Field("bonusTextB").GetValue<TextMeshProUGUI>();

        public TextMeshProUGUI nameText => traverse.Field("nameText").GetValue<TextMeshProUGUI>();
    }
}