using BattleTech;
using BattleTech.UI;
using BattleTech.UI.Tooltips;
using CustomComponents;
using MechEngineer.Features.OverrideDescriptions;

namespace MechEngineer
{
    [CustomComponent("Weights")]
    public class Weights : SimpleCustomComponent, IAdjustSlotElement, IAdjustTooltip
    {
        public int ReservedSlots { get; set; } = 0; // TODO move to own feature... SlotsHandler or SizeHandler
        public float ArmorFactor { get; set; } = 1;
        public float StructureFactor { get; set; } = 1;
        public float EngineFactor { get; set; } = 1;
        public float GyroFactor { get; set; } = 1;
        public float ChassisFactor { get; set; } = 1;

        public void Combine(Weights savings)
        {
            ReservedSlots += savings.ReservedSlots;
            ArmorFactor += savings.ArmorFactor - 1;
            StructureFactor += savings.StructureFactor - 1;
            EngineFactor += savings.EngineFactor - 1;
            GyroFactor += savings.GyroFactor - 1;
            ChassisFactor += savings.ChassisFactor - 1;
        }

        public void AdjustSlotElement(MechLabItemSlotElement element, MechLabPanel panel)
        {
            WeightsHandler.Shared.AdjustSlotElement(element, panel);
        }

        public void AdjustTooltip(TooltipPrefab_Equipment tooltip, MechComponentDef componentDef)
        {
            WeightsHandler.Shared.AdjustTooltip(tooltip, componentDef);
        }
    }
}