using BattleTech;
using BattleTech.UI;
using BattleTech.UI.Tooltips;
using CustomComponents;
using MechEngineer.Features.Engines.Helper;
using MechEngineer.Features.Globals;
using MechEngineer.Features.OverrideDescriptions;

namespace MechEngineer.Features.Engines;

[CustomComponent("EngineHeatBlock")]
public class EngineHeatBlockDef : SimpleCustom<HeatSinkDef>, IAdjustTooltipEquipment, IAdjustSlotElement
{
    public int HeatSinkCount { get; set; }

    public void AdjustTooltipEquipment(TooltipPrefab_Equipment tooltip, MechComponentDef mechComponentDef)
    {
        var def = mechComponentDef.GetComponent<EngineHeatBlockDef>();
        if (def == null)
        {
            return;
        }


        var panel = Global.ActiveMechLabPanel;

        var engine = panel?.CreateMechDef()?.GetEngine();
        if (engine == null)
        {
            return;
        }

        engine.HeatBlockDef = def;

        var originalText = tooltip.detailText.text;
        tooltip.detailText.text = "";

        tooltip.detailText.text += $"<i>{engine.HeatSinkDef.FullName}</i>" +
                                   $"\r\n   Internal" +
                                   $"   Free: <b>{engine.HeatSinkInternalFreeMaxCount}</b> " +
                                   $"   Additional: <b>{engine.HeatBlockDef.HeatSinkCount} / {engine.HeatSinkInternalAdditionalMaxCount}</b>" +
                                   $"\r\n   External" +
                                   $"   Free: <b>{engine.HeatSinkExternalFreeCount} / {engine.HeatSinkExternalFreeMaxCount}</b> " +
                                   $"   Additional: <b>{engine.HeatSinkExternalAdditionalCount}</b>" +
                                   "\r\n";

        tooltip.detailText.text += "\r\n";
        tooltip.detailText.text += "\r\n";
        tooltip.detailText.text += originalText;
        tooltip.detailText.SetAllDirty();

        tooltip.bonusesText.text = BonusValueEngineHeatDissipation(engine) + ", " + BonusValueEngineHeatSinkCounts(engine);
        tooltip.bonusesText.SetAllDirty();
    }

    public void AdjustSlotElement(MechLabItemSlotElement instance, MechLabPanel panel)
    {
        var def = instance.ComponentRef.GetComponent<EngineHeatBlockDef>();
        if (def == null)
        {
            return;
        }

        var mechDef = panel?.CreateMechDef();
        var engine = mechDef?.GetEngine();
        if (engine == null)
        {
            return;
        }

        instance.bonusTextA.text = BonusValueEngineHeatDissipation(engine);
        instance.bonusTextB.text = BonusValueEngineHeatSinkCounts(engine);
    }

    private static string BonusValueEngineHeatDissipation(Engine engine)
    {
        return $"- {engine.EngineHeatDissipation} Heat";
    }

    private static string BonusValueEngineHeatSinkCounts(Engine engine)
    {
        return $"{engine.HeatSinkDef.Abbreviation} {engine.HeatSinkInternalCount}";
    }
}