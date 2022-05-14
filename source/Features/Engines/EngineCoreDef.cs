using BattleTech;
using BattleTech.UI;
using BattleTech.UI.Tooltips;
using CustomComponents;
using MechEngineer.Features.Engines.Helper;
using MechEngineer.Features.Globals;
using MechEngineer.Features.OverrideDescriptions;

namespace MechEngineer.Features.Engines;

[CustomComponent("EngineCore")]
public class EngineCoreDef : SimpleCustom<HeatSinkDef>, IAdjustTooltipEquipment, IAdjustSlotElement, IMechLabFilter, IValueComponent<int>
{
    public int Rating { get; set; }

    public void LoadValue(int value)
    {
        Rating = value;
    }

    // This methid goes public -- bhtrail
    public EngineMovement GetMovement(float tonnage)
    {
        return new(Rating, tonnage);
    }

    public override string ToString()
    {
        return Def.Description.Id + " Rating=" + Rating;
    }

    public bool CheckFilter(MechLabPanel panel)
    {
        if (Control.Settings.Engine.LimitEngineCoresToTonnage)
        {

            if (!string.IsNullOrEmpty(Control.Settings.Engine.IgnoreLimitEngineChassisTag) &&
                panel.activeMechDef.Chassis.ChassisTags.Contains(
                    Control.Settings.Engine.IgnoreLimitEngineChassisTag))
            {
                return true;
            }

            return GetMovement(panel.activeMechDef.Chassis.Tonnage).Mountable;
        }

        return true;
    }

    public void AdjustTooltipEquipment(TooltipPrefab_Equipment tooltip, MechComponentDef mechComponentDef)
    {
        var coreDef = mechComponentDef.GetComponent<EngineCoreDef>();
        if (coreDef == null)
        {
            return;
        }

        var mechDefNullable = Global.ActiveMechDef;
        var engine = mechDefNullable?.GetEngine();
        if (engine == null)
        {
            return;
        }
        var mechDef = mechDefNullable!;

        engine.CoreDef = coreDef;

        var movement = coreDef.GetMovement(mechDef.Chassis.Tonnage);

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

        instance.bonusTextB.text = BonusValueEngineHeatSinkCounts(engine);
    }

    private static string BonusValueEngineHeatSinkCounts(Engine engine)
    {
        return $"+ {engine.HeatSinkDef.Abbreviation} {engine.HeatSinkInternalFreeMaxCount}";
    }
}
