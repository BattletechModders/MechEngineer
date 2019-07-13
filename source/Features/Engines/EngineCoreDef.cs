using BattleTech;
using BattleTech.UI;
using BattleTech.UI.Tooltips;
using CustomComponents;
using fastJSON;
using MechEngineer.Features.Engines.Helper;
using MechEngineer.Features.Globals;
using MechEngineer.Features.OverrideDescriptions;
using MechEngineer.Features.OverrideTonnage;
using UnityEngine;

namespace MechEngineer.Features.Engines
{
    [CustomComponent("EngineCore")]
    public class EngineCoreDef : SimpleCustom<HeatSinkDef>, IAdjustTooltip, IAdjustSlotElement, EngineCoreDef.ICalculator, IMechLabFilter
    {
        public int Rating { get; set; }

        public EngineCoreDef()
        {
            Calculator = new MechCalculator(this);
        }

        internal EngineMovement GetMovement(float tonnage)
        {
            return new EngineMovement(Rating, tonnage);
        }

        public override string ToString()
        {
            return Def.Description.Id + " Rating=" + Rating;
        }

        public bool CheckFilter(MechLabPanel panel)
        {
            return GetMovement(panel.activeMechDef.Chassis.Tonnage).Mountable;
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
            return $"+ {engine.EngineHeatSinkDef.Abbreviation} {engine.CoreDef.Calculator.InternalHeatSinkCount}";
        }

        [JsonIgnore]
        internal ICalculator Calculator { get; set; }

        public int InternalHeatSinkCount => Calculator.InternalHeatSinkCount;
        public int InternalHeatSinkAdditionalMaxCount => Calculator.InternalHeatSinkAdditionalMaxCount;
        public int ExternalHeatSinkFreeMaxCount => Calculator.ExternalHeatSinkFreeMaxCount;

        public int TotalHeatSinkMinCount => Calculator.TotalHeatSinkMinCount;

        public float StandardGyroTonnage => Calculator.StandardGyroTonnage;
        public float StandardEngineTonnage => Calculator.StandardEngineTonnage;
        public float EngineWeightPrecision => Calculator.EngineWeightPrecision;
        
        internal interface ICalculator
        {
            int InternalHeatSinkCount { get; }
            int InternalHeatSinkAdditionalMaxCount { get; }
            int ExternalHeatSinkFreeMaxCount { get; }

            int TotalHeatSinkMinCount { get; }

            float StandardGyroTonnage { get; }
            float StandardEngineTonnage { get; }

            float EngineWeightPrecision { get; }
        }

        internal class MechCalculator : ICalculator
        {
            public MechCalculator(EngineCoreDef coreDef)
            {
                CoreDef = coreDef;
            }

            internal EngineCoreDef CoreDef { get; }

            private int FreeHeatSinks => EngineFeature.settings.MinimumHeatSinksOnMech;
            private int InternalHeatSinksMaxCount => CoreDef.Rating / 25;

            public int TotalHeatSinkMinCount => FreeHeatSinks;

            public int InternalHeatSinkCount => Mathf.Min(FreeHeatSinks, InternalHeatSinksMaxCount);
            public int InternalHeatSinkAdditionalMaxCount => Mathf.Max(0, InternalHeatSinksMaxCount - FreeHeatSinks);
            public int ExternalHeatSinkFreeMaxCount => FreeHeatSinks - InternalHeatSinkCount;

            public float StandardGyroTonnage => PrecisionUtils.RoundUpOverridableDefault(CoreDef.Rating / 100f, 1f);

            public float StandardEngineTonnage => CoreDef.Def.Tonnage - StandardGyroTonnage;
            public float EngineWeightPrecision => 0.5f;
        }
    }

    [CustomComponent("ProtoEngineCore")]
    public class ProtoEngineCoreDef : EngineCoreDef
    {
        public ProtoEngineCoreDef()
        {
            Calculator = new ProtoMechCalculator(this);
        }

        internal class ProtoMechCalculator : ICalculator
        {
            public ProtoMechCalculator(EngineCoreDef coreDef)
            {
                CoreDef = coreDef;
            }

            internal EngineCoreDef CoreDef { get; }

            public int TotalHeatSinkMinCount => 0;

            public int InternalHeatSinkCount => 0;
            public int InternalHeatSinkAdditionalMaxCount => 0;
            public int ExternalHeatSinkFreeMaxCount => 0;

            public float StandardGyroTonnage => 0;

            public float StandardEngineTonnage => CoreDef.Def.Tonnage;
            public float EngineWeightPrecision =>  0.025f;
        }
    }
}