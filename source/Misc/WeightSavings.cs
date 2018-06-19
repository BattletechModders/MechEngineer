using System.Collections.Generic;
using System.Linq;
using BattleTech;

namespace MechEngineMod
{
    internal class WeightSavings
    {
        internal WeightSavingSlotType SlotType { get; private set; }
            
        internal int Count { get; private set; }
        internal int RequiredCount
        {
            get { return SlotType == null ? 0 : SlotType.RequiredCriticalSlotCount; }
        }

        internal float TonnageSaved { get; private set; }

        internal List<string> ErrorMessages { get; private set; }

        private WeightSavings()
        {
            ErrorMessages = new List<string>();
        }

        internal static WeightSavings Create(float tonnage, IEnumerable<MechComponentDef> slots, WeightSavingSlotType[] slotTypes, MechComponentDef def)
        {
            var savings = new WeightSavings();

            string slotTypeId = null;
            string slotName = null;

            if (def != null)
            {
                slotTypeId = def.Description.Id;
                slotName = def.Description.UIName.ToUpper();
                savings.SlotType = slotTypes.First(x => x.ComponentDefId == slotTypeId);
            }

            var mixed = false;

            foreach (var slot in slots)
            {
                if (slotTypeId == null)
                {
                    slotTypeId = slot.Description.Id;
                    slotName = slot.Description.UIName.ToUpper();
                    savings.SlotType = slotTypes.First(x => x.ComponentDefId == slotTypeId);
                }
                else if (slot.Description.Id != slotTypeId)
                {
                    mixed = true;
                    continue;
                }

                savings.Count++;
            }
            
            if (savings.SlotType == null)
            {
                return savings;
            }

            savings.TonnageSaved = (tonnage * savings.SlotType.WeightSavingsFactor).RoundToHalf();

            if (mixed)
            {
                savings.ErrorMessages.Add(slotName + ": Cannot mix-match different critical slot types");
            }

            if (savings.Count > 0 && savings.Count != savings.RequiredCount)
            {
                savings.ErrorMessages.Add(string.Format(slotName + ": Critical slots count does not match ({0} instead of {1})", savings.Count, savings.RequiredCount));
            }

            return savings;
        }
    }
}