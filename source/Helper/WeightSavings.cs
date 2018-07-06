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
                }
            }

            if (savings.SlotType == null)
            {
                return savings;
            }

            {
                var tonnageSaved = tonnage * savings.SlotType.WeightSavingsFactor;
                savings.TonnageSaved = tonnageSaved.RoundStandard();
            }

            if (mixed)
            {
                savings.ErrorMessages.Add(savings.Name + ": Cannot mix-match different critical slot types");
            }

            return savings;
        }
    }
}