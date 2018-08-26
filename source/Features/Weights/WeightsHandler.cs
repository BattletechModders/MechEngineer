using System.Linq;
using BattleTech;
using BattleTech.UI;
using BattleTech.UI.Tooltips;
using CustomComponents;
using UnityEngine;

namespace MechEngineer
{
    internal class WeightsHandler : ITonnageChanges, IAdjustTooltip, IAdjustSlotElement
    {
        public static readonly WeightsHandler Shared = new WeightsHandler();

        public void AdjustTooltip(TooltipPrefab_Equipment tooltipInstance, MechComponentDef mechComponentDef)
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
            
            var tooltip = new TooltipPrefab_EquipmentAdapter(tooltipInstance);
            tooltip.tonnageText.text = $"- {tonnageSaved:0.##}";

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

        public void AdjustSlotElement(MechLabItemSlotElement instance, MechLabPanel panel)
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
                var sign = tonnageSaved > 0 ? "- " : "";
                adapter.bonusTextA.text = $"{sign}{tonnageSaved:0.##} ton";
            }
            else if (adapter.bonusTextA.text.EndsWith("ton"))
            {
                adapter.bonusTextA.text = instance.ComponentRef.Def.BonusValueA;
            }
        }

        private static float CalculateEngineTonnageChanges(MechDef mechDef, Weights savings = null)
        {
            var engine = mechDef.GetEngine();
            if (engine == null)
            {
                return 0;
            }
            
            // TODO only way to get rid of this would be to fake replace using category restrictions
            // then get a prepared mechdef and use that
            // unfortunatly CC does not work on fake mechdefs or inventories in that sense yet
            if (savings != null)
            {
                //var originalTonnage = engine.TotalTonnage;

                if (!Mathf.Approximately(savings.EngineFactor, 1))
                {
                    engine.Weights.EngineFactor = 1;
                }
                
                if (!Mathf.Approximately(savings.GyroFactor, 1))
                {
                    engine.Weights.GyroFactor = 1;
                }

                var defaultTonnage = engine.TotalTonnage;

                if (!Mathf.Approximately(savings.EngineFactor, 1))
                {
                    engine.Weights.EngineFactor = savings.EngineFactor;
                }
                
                if (!Mathf.Approximately(savings.GyroFactor, 1))
                {
                    engine.Weights.GyroFactor = savings.GyroFactor;
                }

                var newTonnage = engine.TotalTonnage;

                //Control.mod.Logger.LogDebug($"originalTonnage={originalTonnage} defaultTonnage={defaultTonnage} newTonnage={newTonnage}");

                return newTonnage - defaultTonnage;
            }

            return engine.TotalTonnageChanges;
        }

        private static float CalculateWeightSavings(MechDef mechDef, Weights savings)
        {
            return CalculateArmorWeightSavings(mechDef, savings)
                   + CalculateStructureWeightSavings(mechDef, savings);
        }

        private static float ArmorRoundingPrecision { get; }
            = Control.settings.ArmorRoundingPrecision ??
              UnityGameInstance.BattleTechGame.MechStatisticsConstants.ARMOR_PER_STEP
              * UnityGameInstance.BattleTechGame.MechStatisticsConstants.TONNAGE_PER_ARMOR_POINT;

        private static float CalculateArmorWeightSavings(MechDef mechDef, Weights weights)
        {
            var unmodified = mechDef.ArmorTonnage();
            var modified = unmodified * weights.ArmorFactor;
            var modifiedRounded = modified.RoundUp(ArmorRoundingPrecision);
            var savings = unmodified - modifiedRounded;
            return savings;
        }

        private static float CalculateStructureWeightSavings(MechDef mechDef, Weights weights)
        {
            var unmodified = mechDef.Chassis.DefaultStructureTonnage();
            var modified = unmodified * weights.StructureFactor;
            var modifiedRounded = modified.RoundUp();
            var savings = unmodified - modifiedRounded;
            return savings;
        }
    }
}