using BattleTech.UI.Tooltips;
using TMPro;

namespace MechEngineer
{
    public class TooltipPrefab_WeaponAdapter : Adapter<TooltipPrefab_Weapon>
    {
        public TooltipPrefab_WeaponAdapter(TooltipPrefab_Weapon instance) : base(instance)
        {
        }
        
        public TextMeshProUGUI bonus => traverse.Field("bonus").GetValue<TextMeshProUGUI>();

        public TextMeshProUGUI tonnage => traverse.Field("tonnage").GetValue<TextMeshProUGUI>();

        public TextMeshProUGUI slots => traverse.Field("slots").GetValue<TextMeshProUGUI>();
    }
}