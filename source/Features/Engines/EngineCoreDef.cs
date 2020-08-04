using BattleTech;
using BattleTech.UI;
using BattleTech.UI.Tooltips;
using CustomComponents;
using MechEngineer.Features.Engines.Helper;
using MechEngineer.Features.Globals;
using MechEngineer.Features.OverrideDescriptions;

namespace MechEngineer.Features.Engines
{
    [CustomComponent("EngineCore")]
    public class EngineCoreDef : SimpleCustom<HeatSinkDef>, IAdjustTooltipEquipment, IAdjustSlotElement, IMechLabFilter
    {
        public int Rating { get; set; }

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
            if (Control.settings.Engine.LimitEngineCoresToTonnage)
            {

                if (!string.IsNullOrEmpty(Control.settings.Engine.IgnoreLimitEngineChassisTag) &&
                    panel.activeMechDef.Chassis.ChassisTags.Contains(
                        Control.settings.Engine.IgnoreLimitEngineChassisTag))
                    return true;

                return GetMovement(panel.activeMechDef.Chassis.Tonnage).Mountable;
            }

            return true;
        }

        public void AdjustTooltipEquipment(TooltipPrefab_Equipment tooltipInstance, MechComponentDef mechComponentDef)
        {
            var coreDef = mechComponentDef.GetComponent<EngineCoreDef>();
            if (coreDef == null)
            {
                return;
            }

            var panel = Global.ActiveMechLabPanel;
            var mechDef = panel?.CreateMechDef();
            var engine = mechDef?.GetEngine();
            if (engine == null)
            {
                return;
            }

            engine.CoreDef = coreDef;

            var movement = coreDef.GetMovement(mechDef.Chassis.Tonnage);

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

            var engine = panel.CreateMechDef()?.GetEngine();
            if (engine == null)
            {
                return;
            }

            var adapter = new MechLabItemSlotElementAdapter(instance);
            adapter.bonusTextB.text = BonusValueEngineHeatSinkCounts(engine);
        }

        private static string BonusValueEngineHeatSinkCounts(Engine engine)
        {
            return $"+ {engine.MechHeatSinkDef.Abbreviation} {engine.HeatSinkInternalFreeMaxCount}";
        }
    }
}