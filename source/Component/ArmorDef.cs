using BattleTech.UI;
using CustomComponents;
using UnityEngine;

namespace MechEngineer
{
    [Custom("ArmorDef")]
    public class ArmorDef : CustomUpgradeDef, IWeightSavingSlotType, IDynamicSlots
    {
        public int RequiredCriticalSlotCount { get; set; }
        public float WeightSavingsFactor { get; set; }
        
        //public UIColor Color { get; } = UIColor.ArmorDamaged;

        public int ReservedSlots => RequiredCriticalSlotCount - 1;
        public UIColor ReservedSlotColor => UIColor.ArmorDamaged;
    }
}