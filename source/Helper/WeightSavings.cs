using System.Collections.Generic;
using System.Linq;
using BattleTech;
using UnityEngine;

namespace MechEngineer
{
    internal class WeightSavings
    {
        private WeightSavings()
        {
            ErrorMessages = new List<string>();
        }

        internal IWeightSavingSlotType SlotType => Def as IWeightSavingSlotType;

        internal MechComponentDef Def { get; private set; }

        internal string Name => Def?.Description.UIName.ToUpperInvariant();

        internal int Count { get; private set; }

        internal int RequiredCount => SlotType?.RequiredCriticalSlotCount ?? 0;

        internal float TonnageSaved { get; private set; }

        internal List<string> ErrorMessages { get; private set; }

        internal static WeightSavings Create(float tonnage, IEnumerable<MechComponentDef> slots, MechComponentDef def)
        {
            var savings = new WeightSavings {Def = def};

            var mixed = false;

            foreach (var slot in slots)
            {
                if (savings.Def == null)
                {
                    savings.Def = slot;
                }
                else if (slot.Description.Id != savings.Def.Description.Id)
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
                    var factor = (float) Mathf.Min(savings.Count, savings.RequiredCount) / savings.RequiredCount;
                    tonnageSaved = tonnageSaved * factor;
                }

                savings.TonnageSaved = tonnageSaved.RoundStandard();
            }

            if (mixed)
            {
                savings.ErrorMessages.Add(savings.Name + ": Cannot mix-match different critical slot types");
            }

            // AllowPartialWeightSavings

            if (savings.Count > 0)
            {
                if (Control.settings.AllowPartialWeightSavings)
                {
                    if (savings.Count > savings.RequiredCount)
                    {
                        savings.ErrorMessages.Add(string.Format(savings.Name + ": Critical slots count exceeded ({0} / {1})", savings.Count, savings.RequiredCount));
                    }
                }
                else
                {
                    if (savings.Count != savings.RequiredCount)
                    {
                        savings.ErrorMessages.Add(string.Format(savings.Name + ": Critical slots count does not match ({0} / {1})", savings.Count, savings.RequiredCount));
                    }
                }
            }

            return savings;
        }
    }
}