using BattleTech.UI.Tooltips;
using TMPro;

namespace MechEngineer
{
    public class TooltipPrefab_EquipmentAdapter : Adapter<TooltipPrefab_Equipment>
    {
        public TooltipPrefab_EquipmentAdapter(TooltipPrefab_Equipment instance) : base(instance)
        {
        }

        public TextMeshProUGUI bonusesText => traverse.Field("bonusesText").GetValue<TextMeshProUGUI>();

        public TextMeshProUGUI detailText => traverse.Field("detailText").GetValue<TextMeshProUGUI>();

        public TextMeshProUGUI tonnageText => traverse.Field("tonnageText").GetValue<TextMeshProUGUI>();

        public TextMeshProUGUI slotsText => traverse.Field("slotsText").GetValue<TextMeshProUGUI>();
    }
}