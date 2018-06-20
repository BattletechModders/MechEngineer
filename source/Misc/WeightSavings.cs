using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using UnityEngine;

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

            {
                var tonnageSaved = tonnage * savings.SlotType.WeightSavingsFactor;
                if (Control.settings.AllowPartialWeightSavings)
                {
                    var factor = (float)Mathf.Min(savings.Count, savings.RequiredCount) / savings.RequiredCount;
                    tonnageSaved = tonnageSaved * factor;
                }
                savings.TonnageSaved = tonnageSaved.RoundStandard();
            }

            if (mixed)
            {
                savings.ErrorMessages.Add(slotName + ": Cannot mix-match different critical slot types");
            }

            // AllowPartialWeightSavings

            if (savings.Count > 0)
            {
                if (Control.settings.AllowPartialWeightSavings)
                {
                    if (savings.Count > savings.RequiredCount)
                    {
                        savings.ErrorMessages.Add(string.Format(slotName + ": Critical slots count exceeded ({0} / {1})", savings.Count, savings.RequiredCount));
                    }
                }
                else
                {
                    if (savings.Count != savings.RequiredCount)
                    {
                        savings.ErrorMessages.Add(string.Format(slotName + ": Critical slots count does not match ({0} / {1})", savings.Count, savings.RequiredCount));
                    }
                }
            }

            return savings;
        }
    }
}