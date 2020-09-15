using System;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using BattleTech.UI.Tooltips;
using CustomComponents;
using MechEngineer.Features.Engines;
using MechEngineer.Features.Engines.Helper;
using MechEngineer.Features.Globals;
using MechEngineer.Features.OverrideDescriptions;
using UnityEngine;

namespace MechEngineer.Features.OverrideTonnage
{
    internal class WeightsHandler : ITonnageChanges, IAdjustTooltipEquipment, IAdjustTooltipWeapon, IAdjustSlotElement
    {
        internal static readonly WeightsHandler Shared = new WeightsHandler();

        // shared between Weights and DynamicSlots
        public void AdjustTooltipEquipment(TooltipPrefab_Equipment tooltipInstance, MechComponentDef mechComponentDef)
        {
            var tooltip = new TooltipPrefab_EquipmentAdapter(tooltipInstance);
            var reservedSlots = 0;

            if (mechComponentDef.Is<Weights>(out var weights))
            {
                reservedSlots += weights.ReservedSlots;
                var mechDef = Global.ActiveMechDef;
                if (mechDef != null)
                {
                    var tonnageChanges = CalculateWeightChanges(mechDef, weights);
                    tooltip.tonnageText.text = FormatChanges(mechComponentDef.Tonnage, tonnageChanges);
                }
            }

            if (mechComponentDef.Is<DynamicSlots.DynamicSlots>(out var dynamicSlots))
            {
                reservedSlots += dynamicSlots.ReservedSlots;
            }

            if (reservedSlots > 0)
            {
                tooltip.slotsText.text = $"{mechComponentDef.InventorySize} + {reservedSlots}";
            }
        }

        public void AdjustTooltipWeapon(TooltipPrefab_Weapon tooltipInstance, MechComponentDef mechComponentDef)
        {
            var tooltip = new TooltipPrefab_WeaponAdapter(tooltipInstance);
            var reservedSlots = 0;

            if (mechComponentDef.Is<Weights>(out var weights))
            {
                reservedSlots += weights.ReservedSlots;
                var mechDef = Global.ActiveMechDef;
                if (mechDef != null)
                {
                    var tonnageChanges = CalculateWeightChanges(mechDef, weights);
                    tooltip.tonnage.text = FormatChanges(mechComponentDef.Tonnage, tonnageChanges);
                }
            }

            if (mechComponentDef.Is<DynamicSlots.DynamicSlots>(out var dynamicSlots))
            {
                reservedSlots += dynamicSlots.ReservedSlots;
            }

            if (reservedSlots > 0)
            {
                tooltip.slots.text = $"{mechComponentDef.InventorySize} + {reservedSlots}";
            }
        }

        private string FormatChanges(float tonnage, float tonnageChanges)
        {
            if (Mathf.Approximately(tonnage, 0))
            {
                return FloatToText(tonnageChanges);
            }
            else if (Mathf.Approximately(tonnageChanges, 0))
            {
                return tonnage.ToString();
            }
            else
            {
                return $"{tonnage} {FloatToText(tonnageChanges, true)}";
            }
        }

        public float TonnageChanges(MechDef mechDef)
        {
            return CalculateWeightChanges(mechDef);
        }

        public void AdjustSlotElement(MechLabItemSlotElement instance, MechLabPanel panel)
        {
            var weights = instance.ComponentRef?.Def?.GetComponent<Weights>();
            if (weights == null)
            {
                return;
            }

            var mechDef = panel.CreateMechDef();
            if (mechDef == null)
            {
                return;
            }

            var tonnageChanges = CalculateWeightChanges(mechDef, weights);
            var adapter = new MechLabItemSlotElementAdapter(instance);

            if (!Mathf.Approximately(tonnageChanges, 0))
            {
                adapter.bonusTextA.text = $"{FloatToText(tonnageChanges)} ton";
            }
            else if (adapter.bonusTextA.text.EndsWith("ton"))
            {
                adapter.bonusTextA.text = instance.ComponentRef.Def.BonusValueA;
            }
        }

        private static string FloatToText(float number, bool positiveSign = false)
        {
            var sign = positiveSign ? "+ " : "";
            if (number < 0)
            {
                sign = "- ";
                number = -number;
            }
            return $"{sign}{number:0.##}";
        }

        private static float CalculateWeightChanges(MechDef mechDef)
        {
            var state = new BaseWeightState(mechDef);

            if (mechDef?.Inventory == null)
            {
                return 0;
            }

            var tonnageChanges = mechDef.Inventory
                .Select(r => r.Def?.GetComponent<Weights>())
                .Where(w => w != null)
                .Sum(weights => CalculateWeightChanges(state, weights));

            if (state.Engine != null)
            {
                // WORKAROUND
                // didn't add free heat sink tonnages to the actual cores, since RT doesn't either we fix it here
                var beforeTonnageChanges = tonnageChanges;
                tonnageChanges -= state.Engine.HeatSinkExternalFreeTonnage;
                Control.Logger.Debug?.Log($"state.Engine.HeatSinkExternalFreeTonnage={state.Engine.HeatSinkExternalFreeTonnage} tonnageChanges={tonnageChanges} beforeTonnageChanges={beforeTonnageChanges}");
            }

            return tonnageChanges;
        }

        private static float CalculateWeightChanges(MechDef mechDef, Weights weights)
        {
            var state = new BaseWeightState(mechDef);

            return CalculateWeightChanges(state, weights);
        }

        private class BaseWeightState
        {
            internal readonly float Armor;
            internal readonly float Structure;
            internal readonly float Chassis;
            internal readonly Engine Engine;

            internal BaseWeightState(MechDef mechDef)
            {
                Armor = mechDef.ArmorTonnage();
                Structure = mechDef.Chassis.Tonnage / 10f;
                Chassis = mechDef.Chassis.Tonnage;
                Engine = mechDef.GetEngine();
            }
        }

        private static float CalculateWeightChanges(BaseWeightState state, Weights weights)
        {
            var tonnageChanges = 0.0f;

            tonnageChanges += CalculateEngineTonnageChanges(state.Engine, weights);
            tonnageChanges += CalcChanges(state.Armor, weights.ArmorFactor, ArmorRoundingPrecision);
            tonnageChanges += CalcChanges(state.Structure, weights.StructureFactor);
            tonnageChanges += CalcChanges(state.Chassis, weights.ChassisFactor);

            return tonnageChanges;
        }

        private static float CalculateEngineTonnageChanges(Engine engine, Weights weights)
        {
            if (engine == null)
            {
                return 0;
            }
            
            // TODO this and other places don't make EngineFactorFactor work
            // doesn't support multiple components
            // therefore it doesn't track what added/removed weights
            engine.Weights = new Weights();

            var defaultTonnage = engine.TotalTonnage;

            engine.Weights = weights;

            var newTonnage = engine.TotalTonnage;

            return newTonnage - defaultTonnage;
        }

        private static float ArmorRoundingPrecision { get; }
            = OverrideTonnageFeature.settings.ArmorRoundingPrecision ??
              UnityGameInstance.BattleTechGame.MechStatisticsConstants.ARMOR_PER_STEP
              * UnityGameInstance.BattleTechGame.MechStatisticsConstants.TONNAGE_PER_ARMOR_POINT;

        private static float CalcChanges(float unmodified, float factor, float? precision = null)
        {
            var modified = unmodified * factor;
            var modifiedRounded = precision == null
                ? PrecisionUtils.RoundUp(modified, OverrideTonnageFeature.settings.TonnageStandardPrecision)
                : PrecisionUtils.RoundUp(modified, (float)precision);
            var changes = modifiedRounded - unmodified;
            return changes;
        }
    }
}