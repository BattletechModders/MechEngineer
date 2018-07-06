using BattleTech.UI;
using CustomComponents;
using UnityEngine;

namespace MechEngineer
{
    [Custom("StructureDef")]
    public class StructureDef : CustomUpgradeDef, IWeightSavingSlotType, IDynamicSlots
    {
        public int RequiredCriticalSlotCount { get; set; }
        public float WeightSavingsFactor { get; set; }
        
        //public UIColor Color { get; } = UIColor.OrangeHalf;

        public int ReservedSlots => RequiredCriticalSlotCount - 1;
        public UIColor ReservedSlotColor => UIColor.OrangeHalf;
    }
}