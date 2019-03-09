using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using BattleTech.UI.Tooltips;
using CustomComponents;

namespace MechEngineer
{
    public class OverrideDescriptionsHandler: IAdjustSlotElement, IAdjustTooltip, IAdjustInventoryElement
    {
        public static OverrideDescriptionsHandler Shared = new OverrideDescriptionsHandler();

        public void AdjustSlotElement(MechLabItemSlotElement element, MechLabPanel panel)
        {
            foreach (var cc in element.ComponentRef.Def.GetComponents<IAdjustSlotElement>())
            {
                cc.AdjustSlotElement(element, panel);
            }
        }

        public void RefreshData(MechLabPanel panel)
        {
            foreach (var element in panel.Elements())
            {
                AdjustSlotElement(element, panel);
            }
        }

        public void AdjustTooltip(TooltipPrefab_Equipment tooltip, MechComponentDef componentDef)
        {
            foreach (var cc in componentDef.GetComponents<IAdjustTooltip>())
            {
                cc.AdjustTooltip(tooltip, componentDef);
            }
        }

        public void AdjustInventoryElement(ListElementController_BASE_NotListView element)
        {
            var componentDef = element?.componentDef;
            if (componentDef == null)
            {
                return;
            }

            foreach (var cc in componentDef.GetComponents<IAdjustInventoryElement>())
            {
                cc.AdjustInventoryElement(element);
            }
        }
    }

    public static class MechLabExtensions
    {
        public static IEnumerable<MechLabItemSlotElement> Elements(this MechLabPanel panel)
        {
            return MechDefBuilder.Locations
                .Select(location => panel.GetLocationWidget((ArmorLocation) location))
                .Select(widget => new MechLabLocationWidgetAdapter(widget))
                .SelectMany(adapter => adapter.LocalInventory);
        }
    }
}
