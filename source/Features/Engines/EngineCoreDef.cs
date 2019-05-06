using BattleTech;
using BattleTech.UI;
using BattleTech.UI.Tooltips;
using CustomComponents;
using fastJSON;
using MechEngineer.Features.Engines.Helper;
using MechEngineer.Features.Globals;
using MechEngineer.Features.OverrideDescriptions;
using UnityEngine;

namespace MechEngineer.Features.Engines
{
    [CustomComponent("EngineCore")]
    public class EngineCoreDef : SimpleCustom<HeatSinkDef>, IAdjustTooltip, IAdjustSlotElement
    {
        [JsonIgnore]
        private int _rating;

        public int Rating
        {
            get => _rating;
            set
            {
                _rating = value;
                CalcHeatSinks();
                CalcTonnages();
            }
        }

        private void CalcHeatSinks()
        {
            var free = 10;
            var total = Rating / 25;
            InternalHeatSinks = Mathf.Min(free, total);
            InternalHeatSinkAdditionalMaxCount = Mathf.Max(0, total - free);
            ExternalHeatSinksFreeMaxCount = free - InternalHeatSinks;
        }

        [JsonIgnore]
        internal int InternalHeatSinks { get; private set; }
        [JsonIgnore]
        internal int InternalHeatSinkAdditionalMaxCount { get; private set; }
        [JsonIgnore]
        internal int ExternalHeatSinksFreeMaxCount { get; private set; }

        private void CalcTonnages()
        {
            StandardGyroTonnage = Mathf.Ceil(Rating / 100f);
        }

        [JsonIgnore]
        internal float StandardGyroTonnage { get; private set; }

        internal float StandardEngineTonnage => Def.Tonnage - StandardGyroTonnage;

        internal EngineMovement GetMovement(float tonnage)
        {
            return new EngineMovement(Rating, tonnage);
        }

        public override string ToString()
        {
            return Def.Description.Id + " Rating=" + Rating;
        }

        public void AdjustTooltip(TooltipPrefab_Equipment tooltipInstance, MechComponentDef mechComponentDef)
        {
            var coreDef = mechComponentDef.GetComponent<EngineCoreDef>();
            if (coreDef == null)
            {
                return;
            }

            var panel = Global.ActiveMechLabPanel;
            if (panel == null)
            {
                return;
            }

            var engine = panel.activeMechInventory.GetEngine();
            if (engine == null)
            {
                return;
            }

            engine.CoreDef = coreDef;

            var movement = coreDef.GetMovement(panel.activeMechDef.Chassis.Tonnage);

            var tooltip = new TooltipPrefab_EquipmentAdapter(tooltipInstance);
            var originalText = tooltip.detailText.text;
            tooltip.detailText.text = "";

            tooltip.detailText.text += "<i>Speeds</i>" +
                                       "   Cruise <b>" + movement.WalkSpeed + "</b>" +
                                       " / Top <b>" + movement.RunSpeed + "</b>";

            tooltip.detailText.text += "\r\n" +
                                       "<i>Weights [Ton]</i>" +
                                       "   Engine: <b>" + engine.EngineTonnage + "</b>" +
                                       "   Gyro: <b>" + engine.GyroTonnage + "</b>" +
                                       "   Sinks: <b>" + engine.HeatSinkTonnage + "</b>";

            tooltip.tonnageText.text = $"{engine.TotalTonnage}";

            tooltip.detailText.text += "\r\n";
            tooltip.detailText.text += "\r\n";
            tooltip.detailText.text += originalText;
            tooltip.detailText.SetAllDirty();
        }

        public void AdjustSlotElement(MechLabItemSlotElement instance, MechLabPanel panel)
        {
            var def = instance.ComponentRef.GetComponent<CoolingDef>();
            if (def == null)
            {
                return;
            }

            var mechDef = panel.activeMechDef;
            if (mechDef == null)
            {
                return;
            }

            var engine = mechDef.Inventory.GetEngine();

            var adapter = new MechLabItemSlotElementAdapter(instance);
            adapter.bonusTextB.text = BonusValueEngineHeatSinkCounts(engine);
        }

        private static string BonusValueEngineHeatSinkCounts(Engine engine)
        {
            return $"+ {engine.EngineHeatSinkDef.Abbreviation} {engine.CoreDef.InternalHeatSinks}";
        }
    }
}