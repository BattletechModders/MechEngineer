using BattleTech;
using BattleTech.UI.Tooltips;

namespace MechEngineer.Features.OverrideDescriptions
{
    internal interface IAdjustTooltipEquipment
    {
        void AdjustTooltipEquipment(TooltipPrefab_Equipment tooltip, MechComponentDef componentDef);
    }
}