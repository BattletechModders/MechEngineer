using BattleTech;
using BattleTech.UI;
using BattleTech.UI.Tooltips;
using CustomComponents;

namespace MechEngineer
{
    [CustomComponent("Weights")]
    public class Weights : SimpleCustomComponent, IAdjustSlotElement, IAdjustTooltip
    {
        public int ReservedSlots { get; set; } = 0;
        public float ArmorFactor { get; set; } = 1;
        public float StructureFactor { get; set; } = 1;
        public float EngineFactor { get; set; } = 1;
        public float GyroFactor { get; set; } = 1;

        public void Combine(Weights savings)
        {
            ReservedSlots *= savings.ReservedSlots;
            ArmorFactor *= savings.ArmorFactor;
            StructureFactor *= savings.StructureFactor;
            EngineFactor *= savings.EngineFactor;
            GyroFactor *= savings.GyroFactor;
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