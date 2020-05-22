using BattleTech;
using BattleTech.UI.Tooltips;

namespace MechEngineer.Features.OverrideDescriptions
{
    internal interface IAdjustTooltipWeapon
    {
        void AdjustTooltipWeapon(TooltipPrefab_Weapon tooltip, MechComponentDef componentDef);
    }
}