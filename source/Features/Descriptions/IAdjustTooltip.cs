using BattleTech;
using BattleTech.UI.Tooltips;

namespace MechEngineer
{
    internal interface IAdjustTooltip
    {
        void AdjustTooltip(TooltipPrefab_Equipment tooltip, MechComponentDef componentDef);
    }
}