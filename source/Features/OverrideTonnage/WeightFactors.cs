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
    // TODO rename to BaseMultiplier, all factors are additive and therefore only a factor against the base
    public float ArmorFactor { get; set; } = 1;
    public float StructureFactor { get; set; } = 1;
    public float EngineFactor { get; set; } = 1;
    public float GyroFactor { get; set; } = 1;
    public float ChassisCapacityFactor { get; set; } = 1;

    // Multipliers are multiplied with each other and the base
    public float EngineMultiplier { get; set; } = 1;

    // not factors
    public int ReservedSlots { get; set; } = 0; // TODO move to own feature... SlotsHandler or SizeHandler
    public float ComponentByChassisFactor { get; set; } = 0; // TODO move to something more elaborate (see CustomCapacities / HBS statistics)

    public BonusSlot? BonusSlot { get; set; }

    public void Combine(WeightFactors savings)
    {
        ArmorFactor += savings.ArmorFactor - 1;
        StructureFactor += savings.StructureFactor - 1;
        EngineFactor += savings.EngineFactor - 1;
        GyroFactor += savings.GyroFactor - 1;
        ChassisCapacityFactor += savings.ChassisCapacityFactor - 1;

        EngineMultiplier *= savings.EngineMultiplier;

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