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

            var tonnageSaved = CalculateWeightSavings(weights, mechDef);

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
            var tonnageSaved = 0f;
            foreach (var savings in mechDef.Inventory.Select(r => r.Def.GetComponent<Weights>()).Where(w => w != null))
            {
                tonnageSaved += CalculateWeightSavings(savings, mechDef);
            }

            return -tonnageSaved;
        }

        private static float CalculateWeightSavings(Weights savings, MechDef mechDef)
        {
            return CalculateArmorWeightSavings(savings, mechDef)
                   + CalculateStructureWeightSavings(savings, mechDef)
                   + CalculateEngineWeightSavings(savings, mechDef);
        }

        private static float CalculateArmorWeightSavings(Weights savings, MechDef mechDef)
        {
            return (mechDef.ArmorTonnage() * (1 - savings.ArmorFactor)).RoundStandard();
        }

        private static float CalculateStructureWeightSavings(Weights savings, MechDef mechDef)
        {
            return (mechDef.Chassis.DefaultStructureTonnage() * (1 - savings.StructureFactor)).RoundStandard();
        }

        private static float CalculateEngineWeightSavings(Weights savings, MechDef mechDef)
        {
            if (Mathf.Approximately(savings.EngineFactor, 1) && Mathf.Approximately(savings.GyroFactor, 1))
            {
                return 0;
            }
            
            var engine = mechDef.GetEngine();
            if (engine == null)
            {
                return 0;
            }

            engine.Weights = savings;
            return - engine.TotalTonnageChanges;
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

            var tonnageSaved = CalculateWeightSavings(weights, mechDef);
            var adapter = new MechLabItemSlotElementAdapter(instance);

            if (!Mathf.Approximately(tonnageSaved, 0))
            {
                adapter.bonusTextA.text = $"- {tonnageSaved} ton";
            }
        }
    }
}