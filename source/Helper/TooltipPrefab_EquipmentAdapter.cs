using BattleTech.UI.Tooltips;
using TMPro;

namespace MechEngineer
{
    public class TooltipPrefab_EquipmentAdapter : Adapter<TooltipPrefab_Equipment>
    {
        public TooltipPrefab_EquipmentAdapter(TooltipPrefab_Equipment instance) : base(instance)
        {
        }

        public TextMeshProUGUI bonusesText
        {
            get { return traverse.Field("bonusesText").GetValue<TextMeshProUGUI>(); }
        }

        public TextMeshProUGUI detailText
        {
            get { return traverse.Field("detailText").GetValue<TextMeshProUGUI>(); }
        }

        public TextMeshProUGUI tonnageText
        {
            get { return traverse.Field("tonnageText").GetValue<TextMeshProUGUI>(); }
        }
    }
}