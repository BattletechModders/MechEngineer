using BattleTech.UI.Tooltips;

namespace MechEngineer.Features.OverrideDescriptions
{
    internal static class TooltipPrefab_EquipmentExtensions
    {
        internal static void ShowBonuses(this TooltipPrefab_Equipment instance, bool value)
        {
            var text = instance.bonusesText.transform.parent.parent.parent;
            text.gameObject.SetActive(false);
        }
    }
}