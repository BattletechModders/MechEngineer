using BattleTech;
using BattleTech.UI;
using BattleTech.UI.Tooltips;
using CustomComponents;
using MechEngineer.Features.OverrideDescriptions;
using MechEngineer.Misc;

namespace MechEngineer.Features.OverrideTonnage;

[CustomComponent("Weights")]
[UsedBy(User.BattleValue)]
public class WeightFactors : SimpleCustomComponent, IAdjustSlotElement, IAdjustTooltipEquipment, IAdjustTooltipWeapon
{
    // factors are additive with other factors of the same name (2.0,0.5->1.5, not 2.0,0.5->1.0)
    public float ArmorFactor { get; set; } = 1;
    public float StructureFactor { get; set; } = 1;
    public float EngineFactor { get; set; } = 1; // XL, compact engines etc..
    // this breaks tooltips/item descriptions on mechs where EngineFactor is not default
    // mainly since an empty Weight() is being used to compare the weight changes a component does
    // we would need to hook into the replace logic of CC to work with a full Weight object, removing any competing items
    public float Engine2Factor { get; set; } = 1; // supercharger
    public float GyroFactor { get; set; } = 1;
    public float ChassisCapacityFactor { get; set; } = 1;

    // not factors
    public int ReservedSlots { get; set; } = 0; // TODO move to own feature... SlotsHandler or SizeHandler
    public float ComponentByChassisFactor { get; set; } = 0; // TODO move to something more elaborate (see CustomCapacities / HBS statistics)

    public BonusSlot? BonusSlot { get; set; }

    public void Combine(WeightFactors savings)
    {
        ArmorFactor += savings.ArmorFactor - 1;
        StructureFactor += savings.StructureFactor - 1;
        EngineFactor += savings.EngineFactor - 1;
        Engine2Factor += savings.Engine2Factor - 1;
        GyroFactor += savings.GyroFactor - 1;
        ChassisCapacityFactor += savings.ChassisCapacityFactor - 1;

        ReservedSlots += savings.ReservedSlots;
        ComponentByChassisFactor += savings.ComponentByChassisFactor;
    }

    public void AdjustSlotElement(MechLabItemSlotElement element, MechLabPanel panel)
    {
        WeightsHandler.Shared.AdjustSlotElement(element, panel);
    }

    public void AdjustTooltipEquipment(TooltipPrefab_Equipment tooltip, MechComponentDef componentDef)
    {
        WeightsHandler.Shared.AdjustTooltipEquipment(tooltip, componentDef);
    }

    public void AdjustTooltipWeapon(TooltipPrefab_Weapon tooltip, MechComponentDef componentDef)
    {
        WeightsHandler.Shared.AdjustTooltipWeapon(tooltip, componentDef);
    }
}