using System.Linq;
using BattleTech;
using BattleTech.UI;
using CustomComponents;
using UnityEngine;

namespace MechEngineer
{
    internal class WeightsHandler : ITonnageChanges, IAdjustTooltip, IRefreshSlotElement
    {
        public static readonly WeightsHandler Shared = new WeightsHandler();

        public void AdjustTooltip(TooltipPrefab_EquipmentAdapter tooltip, MechComponentDef mechComponentDef)
        {
            var weights = mechComponentDef.GetComponent<Weights>();
            if (weights == null)
            {
                return;
            }
            

            var mechDef = Global.ActiveMechDef;
            if (mechDef == null)
            {
                return;
            }

            var tonnageSaved = CalculateWeightSavings(mechDef, weights);
            tonnageSaved -= CalculateEngineTonnageChanges(mechDef, weights);

            tooltip.tonnageText.text = $"- {tonnageSaved}";

            // TODO move to own feature... SlotsHandler or SizeHandler
            var reservedSlots = weights.ReservedSlots;
            if (mechComponentDef.Is<DynamicSlots>(out var dynamicSlots))
            {
                reservedSlots += dynamicSlots.ReservedSlots;
            }

            if (reservedSlots > 0)
            {
                tooltip.slotsText.text = $"{mechComponentDef.InventorySize} + {reservedSlots}";
            }

            tooltip.bonusesText.SetAllDirty();
        }

        public float TonnageChanges(MechDef mechDef)
        {
            var tonnageChanges = 0f;
            foreach (var savings in mechDef.Inventory.Select(r => r.Def.GetComponent<Weights>()).Where(w => w != null))
            {
                tonnageChanges -= CalculateWeightSavings(mechDef, savings);
            }

            tonnageChanges += CalculateEngineTonnageChanges(mechDef);

            return tonnageChanges;
        }

        public void RefreshSlotElement(MechLabItemSlotElement instance, MechLabPanel panel)
        {
            var weights = instance.ComponentRef?.Def?.GetComponent<Weights>();
            if (weights == null)
            {
                return;
            }

            var mechDef = panel.activeMechDef;
            if (mechDef == null)
            {
                return;
            }

            var tonnageSaved = CalculateWeightSavings(mechDef, weights);
            tonnageSaved -= CalculateEngineTonnageChanges(mechDef, weights);
            var adapter = new MechLabItemSlotElementAdapter(instance);

            if (!Mathf.Approximately(tonnageSaved, 0))
            {
                adapter.bonusTextA.text = $"- {tonnageSaved} ton";
            }
        }

        private static float CalculateEngineTonnageChanges(MechDef mechDef, Weights savings = null)
        {
            var engine = mechDef.GetEngine();
            if (engine == null)
            {
                return 0;
            }

            if (savings != null)
            {
                if (!Mathf.Approximately(savings.EngineFactor, 1))
                {
                    engine.Weights.EngineFactor = savings.EngineFactor;
                }
                
                if (!Mathf.Approximately(savings.GyroFactor, 1))
                {
                    engine.Weights.GyroFactor = savings.GyroFactor;
                }
            }

            return engine.TotalTonnageChanges;
        }

        private static float CalculateWeightSavings(MechDef mechDef, Weights savings)
        {
            return CalculateArmorWeightSavings(mechDef, savings)
                   + CalculateStructureWeightSavings(mechDef, savings);
        }

        private static float CalculateArmorWeightSavings(MechDef mechDef, Weights savings)
        {
            return (mechDef.ArmorTonnage() * (1 - savings.ArmorFactor)).RoundStandard();
        }

        private static float CalculateStructureWeightSavings(MechDef mechDef, Weights savings)
        {
            return (mechDef.Chassis.DefaultStructureTonnage() * (1 - savings.StructureFactor)).RoundStandard();
        }
    }
}