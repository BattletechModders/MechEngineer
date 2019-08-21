using System.Collections.Generic;
using BattleTech.UI;
using SVGImporter;
using TMPro;
using UnityEngine;

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

        public List<GameObject> spacers => traverse.Field("spacers").GetValue<List<GameObject>>();

        public GameObject fixedEquipmentOverlay => traverse.Field("fixedEquipmentOverlay").GetValue<GameObject>();

        public UIColorRefTracker iconColor => traverse.Field("iconColor").GetValue<UIColorRefTracker>();

        public UIColorRefTracker nameTextColor => traverse.Field("nameTextColor").GetValue<UIColorRefTracker>();
    }
}