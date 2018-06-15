using System.Collections.Generic;
using System.Linq;
using BattleTech;

namespace MechEngineMod
{
    internal class WeightSavingSlotCalculator
    {
        public WeightSavingSlotType SlotType { get; private set; }
            
        public int Count { get; private set; }
        public int RequiredCount
        {
            get { return SlotType.RequiredCriticalSlotCount; }
        }
        public string ErrorMessage { get; private set; }

        internal WeightSavingSlotCalculator(IEnumerable<MechComponentRef> slots, WeightSavingSlotType[] slotTypes)
        {
            string slotTypeId = null;
            string slotName = null;
            foreach (var slot in slots)
            {
                if (slotTypeId == null)
                {
                    slotTypeId = slot.ComponentDefID;
                    slotName = slot.Def.Description.UIName.ToUpper();
                    SlotType = slotTypes.First(x => x.ComponentDefId == slotTypeId);
                }
                else if (slot.ComponentDefID != slotTypeId)
                {
                    ErrorMessage = slotName + ": Cannot mix-match different critical slot types";
                    return;
                }

                Count++;
            }

            if (Count <= 0 || Count == RequiredCount)
            {
                return;
            }

            ErrorMessage = string.Format(slotName + ": Critical slots count does not match ({0} instead of {1})", Count, RequiredCount);
        }

        internal float WeightSavingForTonnage(float tonnage)
        {
            var savings = tonnage * SlotType.WeightSavingsFactor;
            return savings.RoundToHalf();
        }
    }
}